using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SerialPort.Components.SerialPort;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;
using MultiPlug.Base.Exchange.API;
using MultiPlug.Extension.Core;

namespace MultiPlug.Ext.SerialPort
{
    internal class Core : MultiPlugBase
    {
        private static Core m_Instance = null;

        private IMultiPlugServices m_MultiPlugServices;
        private IMultiPlugActions m_MultiPlugActions;

        internal Subscription[] Subscriptions { get; private set; } = new Subscription[0];
        internal Event[] Events { get; private set; } = new Event[0];

        [DataMember]
        public SerialPortComponent[] SerialPorts { get; set; } = new SerialPortComponent[0];

        internal bool DeleteSubscription(string theSerialPortGuid, Subscription theSubscription)
        {
            var SerialPortsSearch = SerialPorts.FirstOrDefault(Lane => Lane.Guid == theSerialPortGuid);

            if (SerialPortsSearch != null)
            {
                return SerialPortsSearch.DeleteWriteSubscription(theSubscription);
            }

            return false;
        }

        public static Core Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Core();
                }
                return m_Instance;
            }
        }

        internal void Init(IMultiPlugServices theMultiPlugServices, IMultiPlugActions theMultiPlugActions)
        {
            m_MultiPlugServices = theMultiPlugServices;
            m_MultiPlugActions = theMultiPlugActions;
        }

        internal void Update(SerialPortProperties[] theSerialPorts)
        {
            bool NewSerialPort = false;
            List<SerialPortComponent> SerialPortsList = SerialPorts.ToList();

            foreach (var SerialPortProperties in theSerialPorts)
            {


                if (string.IsNullOrEmpty(SerialPortProperties.Guid))
                {
                    SerialPortProperties.Guid = Guid.NewGuid().ToString();
                }

                var SerialPortSearch = Instance.SerialPorts.FirstOrDefault(Lane => Lane.Guid == SerialPortProperties.Guid);

                if (SerialPortSearch == null)
                {
                    NewSerialPort = true;

                    ILoggingService LoggingService = m_MultiPlugServices.Logging.New(SerialPortProperties.Guid, Diagnostics.EventLogDefinitions.DefinitionsId);

                    SerialPortSearch = new SerialPortComponent(SerialPortProperties.Guid, LoggingService);
                    SerialPortSearch.EventsUpdated += OnEventsUpdated;
                    SerialPortSearch.SubscriptionsUpdated += OnSubscriptionsUpdated;
                    SerialPortsList.Add(SerialPortSearch);
                }

                SerialPortSearch.UpdateProperties(SerialPortProperties);
            }

            SerialPorts = SerialPortsList.ToArray();

            if(NewSerialPort)
            {
                AggregateSubscriptions();
                AggregateEvents();
            }
        }

        internal void Shutdown()
        {
            foreach (SerialPortComponent SerialPort in SerialPorts)
            {
                SerialPort.Close();
            }
        }

        internal bool Remove(SerialPortProperties theSerialPorts)
        {
            var SerialPortSearch = Instance.SerialPorts.FirstOrDefault(Lane => Lane.Guid == theSerialPorts.Guid);

            if(SerialPortSearch == null)
            {
                return false;
            }
            else
            {
                SerialPortSearch.Close();
                SerialPortSearch.EventsUpdated -= OnEventsUpdated;
                SerialPortSearch.SubscriptionsUpdated -= OnSubscriptionsUpdated;

                List<SerialPortComponent> SerialPortsList = SerialPorts.ToList();
                SerialPortsList.Remove(SerialPortSearch);
                SerialPorts = SerialPortsList.ToArray();

                AggregateSubscriptions();
                AggregateEvents();

                return true;
            }
        }

        private void OnSubscriptionsUpdated()
        {
            AggregateSubscriptions();
        }

        private void OnEventsUpdated()
        {
            AggregateEvents();
        }

        private void AggregateEvents()
        {
            var EventsList = new List<Event>();

            foreach (var SerialPort in SerialPorts)
            {
                EventsList.Add(SerialPort.ReadEvent);
            }

            Events = EventsList.ToArray();
            m_MultiPlugActions.Extension.Updates.Events();
        }

        private void AggregateSubscriptions()
        {
            var SubscriptionsList = new List<Subscription>();

            foreach (var SerialPort in SerialPorts)
            {
                SubscriptionsList.AddRange(SerialPort.WriteSubscriptions);
            }

            Subscriptions = SubscriptionsList.ToArray();
            m_MultiPlugActions.Extension.Updates.Subscriptions();
        }
    }
}
