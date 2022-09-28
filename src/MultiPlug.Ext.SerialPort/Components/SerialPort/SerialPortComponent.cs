using System;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Exchange.API;
using MultiPlug.Ext.SerialPort.Diagnostics;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;
using DotNetSerialPort = System.IO.Ports.SerialPort;

namespace MultiPlug.Ext.SerialPort.Components.SerialPort
{
    public class SerialPortComponent : SerialPortProperties
    {
        public event Action EventsUpdated;
        public event Action SubscriptionsUpdated;

        private DotNetSerialPort m_SerialPort = null;

        private ILoggingService m_LoggingService;

        internal string LogEventId { get { return m_LoggingService.EventId; } }

        private CancellationTokenSource m_ReadCancellationToken = new CancellationTokenSource();
        private string m_ReadTo = string.Empty;
        private string m_WritePrefix = string.Empty;
        private string m_WriteAppend = string.Empty;

        public SerialPortComponent(string theGuid, ILoggingService theLoggingService)
        {
            Guid = theGuid;
            m_LoggingService = theLoggingService;
            m_SerialPort = new DotNetSerialPort();
            // We don't use DataReceived as it's not supported by Mono
            // m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);

            ReadEvent = new Event { Guid = System.Guid.NewGuid().ToString(), Id = "SerialPortRead-" + System.Guid.NewGuid().ToString().Substring(0,7), Description = "Serial Port Read", Subjects = new string[] { "ReadValue" } };
            WriteSubscriptions = new Subscription[0];
        }

        internal void UpdateProperties(SerialPortProperties theNewProperties)
        {
            bool FlagSubscriptionUpdated = false;
            bool FlagEventUpdated = false;
            bool FlagBeginConnect = false;

            if(theNewProperties.LoggingLevel != -1) // Flag for Don't Change
            {
                LoggingLevel = theNewProperties.LoggingLevel;
            }

            if(PortName != theNewProperties.PortName)
            {
                PortName = theNewProperties.PortName;
                FlagBeginConnect = true;
            }

            if(BaudRate != theNewProperties.BaudRate )
            {
                BaudRate = theNewProperties.BaudRate;
                FlagBeginConnect = true;
            }

            if(Parity != theNewProperties.Parity)
            {
                Parity = theNewProperties.Parity;
                FlagBeginConnect = true;
            }

            if (DataBits != theNewProperties.DataBits)
            {
                DataBits = theNewProperties.DataBits;
                FlagBeginConnect = true;
            }

            if (StopBits != theNewProperties.StopBits)
            {
                StopBits = theNewProperties.StopBits;
                FlagBeginConnect = true;
            }

            if( ReadTo != theNewProperties.ReadTo )
            {
                ReadTo = theNewProperties.ReadTo;
                m_ReadTo = ReadTo != null ? Regex.Unescape(ReadTo) : string.Empty;
                FlagBeginConnect = true;
            }

            if(ReadTimeout != theNewProperties.ReadTimeout)
            {
                ReadTimeout = theNewProperties.ReadTimeout;
                FlagBeginConnect = true;
            }

            if(WriteTimeout != theNewProperties.WriteTimeout)
            {
                WriteTimeout = theNewProperties.WriteTimeout;
                FlagBeginConnect = true;
            }

            if(ReadStrategy != theNewProperties.ReadStrategy)
            {
                ReadStrategy = theNewProperties.ReadStrategy;
                FlagBeginConnect = true;
            }

            if(WritePrefix != theNewProperties.WritePrefix)
            {
                WritePrefix = theNewProperties.WritePrefix;
                m_WritePrefix = WritePrefix != null ? Regex.Unescape(WritePrefix) : string.Empty;
            }

            if(WriteAppend != theNewProperties.WriteAppend)
            {
                WriteAppend = theNewProperties.WriteAppend;
                m_WriteAppend = WriteAppend != null ? Regex.Unescape(WriteAppend) : string.Empty;
            }

            ReadTrim = theNewProperties.ReadTrim;
            ReadPrefix = theNewProperties.ReadPrefix;
            ReadAppend = theNewProperties.ReadAppend;

            if ( theNewProperties.ReadEvent != null)
            {
                if( Event.Merge(ReadEvent, theNewProperties.ReadEvent) ) { FlagEventUpdated = true; }
            }

            if (theNewProperties.WriteSubscriptions != null && UpdateMachineReadySubscriptions(theNewProperties.WriteSubscriptions))
            {
                FlagSubscriptionUpdated = true;
            }

            // Keep this last
            if (Enabled != theNewProperties.Enabled)
            {
                Enabled = theNewProperties.Enabled;

                if (Enabled)
                {
                    FlagBeginConnect = true;
                }
                else
                {
                    Close();
                }
            }

            if (FlagSubscriptionUpdated) { SubscriptionsUpdated?.Invoke(); }
            if (FlagEventUpdated) { EventsUpdated?.Invoke(); }

            if(Enabled && FlagBeginConnect)
            {
                Open();
            }
        }
        internal void UpdateProperties(int theLoggingLevel)
        {
            LoggingLevel = theLoggingLevel;
        }

        internal string GetTraceLog()
        {
            return string.Join(Environment.NewLine, m_LoggingService.Read());
        }

        internal bool DeleteWriteSubscription(Subscription theSubscription)
        {
            var Subscriptions = new List<Subscription>(WriteSubscriptions);

            var SubscriptionSearch = Subscriptions.FirstOrDefault(Sub => Sub.Guid == theSubscription.Guid);

            if (SubscriptionSearch != null)
            {
                Subscriptions.Remove(SubscriptionSearch);
                WriteSubscriptions = Subscriptions.ToArray();
                SubscriptionsUpdated?.Invoke();
                return true;
            }

            return false;
        }

        private bool UpdateMachineReadySubscriptions(Subscription[] theMachineReadySubscriptions)
        {
            bool Result = false;

            var list = new List<Subscription>(WriteSubscriptions);

            foreach (var UpdatedSubscription in theMachineReadySubscriptions)
            {
                if (string.IsNullOrEmpty(UpdatedSubscription.Guid))
                {
                    if (!string.IsNullOrEmpty(UpdatedSubscription.Id))
                    {
                        UpdatedSubscription.Guid = System.Guid.NewGuid().ToString();
                        UpdatedSubscription.Event += OnSubscriptionEvent;
                        list.Add(UpdatedSubscription);
                        Result = true;
                    }
                }
                else
                {
                    var ExistingSubSearch = list.FirstOrDefault(s => s.Guid == UpdatedSubscription.Guid);

                    if (ExistingSubSearch == null)
                    {
                        if (!string.IsNullOrEmpty(UpdatedSubscription.Id))
                        {
                            UpdatedSubscription.Event += OnSubscriptionEvent;
                            list.Add(UpdatedSubscription);
                            Result = true;
                        }
                    }
                    else
                    {
                        if (Subscription.Merge(ExistingSubSearch, UpdatedSubscription)) { Result = true; }
                    }
                }
            }

            WriteSubscriptions = list.ToArray();

            return Result;
        }

        private void Open()
        {
            if (Opened)
            {
                Close();
            }

            try
            {
                m_SerialPort.PortName = PortName;
                m_SerialPort.BaudRate = BaudRate;

                switch(Parity)
                {
                    case 0:
                        m_SerialPort.Parity = System.IO.Ports.Parity.None;
                        break;
                    case 1:
                        m_SerialPort.Parity = System.IO.Ports.Parity.Odd;
                        break;
                    case 2:
                        m_SerialPort.Parity = System.IO.Ports.Parity.Even;
                        break;
                    case 3:
                        m_SerialPort.Parity = System.IO.Ports.Parity.Mark;
                        break;
                    case 4:
                        m_SerialPort.Parity = System.IO.Ports.Parity.Space;
                        break;
                }

                switch(DataBits)
                {
                    case 0:
                        m_SerialPort.DataBits = 8;
                        break;
                    case 1:
                        m_SerialPort.DataBits = 7;
                        break;
                    case 2:
                        m_SerialPort.DataBits = 6;
                        break;
                    case 3:
                        m_SerialPort.DataBits = 5;
                        break;
                }

                switch (StopBits)
                {
                    case 0:
                        m_SerialPort.StopBits = System.IO.Ports.StopBits.One;
                        break;
                    case 1:
                        m_SerialPort.StopBits = System.IO.Ports.StopBits.OnePointFive;
                        break;
                    case 2:
                        m_SerialPort.StopBits = System.IO.Ports.StopBits.Two;
                        break;
                }

                m_SerialPort.WriteTimeout = WriteTimeout == 0 ? DotNetSerialPort.InfiniteTimeout : WriteTimeout;
                m_SerialPort.ReadTimeout = ReadTimeout == 0 ? DotNetSerialPort.InfiniteTimeout : ReadTimeout;
                m_SerialPort.Open();
                Opened = m_SerialPort.IsOpen;

                if(Opened)
                {
                    if ( LoggingLevel > 1)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.PortOpened);
                    }

                    Task ReadTask = Task.Run(() =>
                    {
                        CancellationTokenSource ReadCancellationToken = m_ReadCancellationToken;

                        while ( !ReadCancellationToken.IsCancellationRequested )
                        {
                            try
                            {
                                ReadLoop(ReadCancellationToken);
                            }
                            catch
                            {
                                break;
                            }

                        }
                    }, m_ReadCancellationToken.Token);
                }
            }
            catch (Exception theException)
            {
                m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
            }
        }

        internal void Close()
        {
            if (m_SerialPort.IsOpen)
            {
                try
                {
                    m_SerialPort.Close();
                    if (LoggingLevel > 1)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.PortClosed);
                    }
                }
                catch (System.IO.IOException theException)
                {
                    m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
                }
            }

            m_ReadCancellationToken.Cancel();
            m_ReadCancellationToken = new CancellationTokenSource();

            Opened = false;
        }

        private void OnSubscriptionEvent(SubscriptionEvent theSubscriptionEvent)
        {
            foreach(PayloadSubject PayloadSubject in theSubscriptionEvent.PayloadSubjects)
            {
                if (m_SerialPort.IsOpen)
                {
                    m_SerialPort.Write(m_WritePrefix + PayloadSubject.Value + m_WriteAppend);
                }

                if (LoggingLevel == 1)
                {
                    m_LoggingService.WriteEntry((uint)EventLogEntryCodes.DataWrite, new string[] { string.Empty });
                }
                else if (LoggingLevel == 2)
                {
                    m_LoggingService.WriteEntry((uint)EventLogEntryCodes.DataWrite, new string[] { PayloadSubject.Value });
                }
            }
        }

        private void ReadLoop(CancellationTokenSource theReadCancellationToken)
        {
            CancellationTokenSource ReadCancellationToken = theReadCancellationToken;

            string Read = string.Empty;

            switch ( ReadStrategy)
            {
                case 0:
                    try
                    {
                        // We do this to block the Serial Port
                        var OneChar = new char[1];
                        int Chars = m_SerialPort.Read(OneChar, 0, 1);
                        // Now Continue
                        if (m_SerialPort.BytesToRead > 0)
                        {
                            Read = m_SerialPort.ReadExisting();
                        }

                        if(Chars == 1)
                        {
                            Read = new string(OneChar) + Read;
                        }
                    }
                    catch (InvalidOperationException theException)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
                        throw (theException);
                    }
                    catch (TimeoutException) { return; }
                    break;
                case 1:
                    try
                    {
                        Read = m_SerialPort.ReadTo(m_ReadTo);
                    }
                    catch (ArgumentNullException theException)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
                        throw (theException);
                    }
                    catch (ArgumentException theException)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
                        throw (theException);
                    }
                    catch (InvalidOperationException theException)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
                        throw (theException);
                    }
                    catch (TimeoutException) { return; }
                    break;
            }

            if ( ! string.IsNullOrEmpty(Read))
            {
                if(ReadTrim)
                {
                    Read = Read.Trim();
                }

                if( ! string.IsNullOrEmpty( ReadPrefix ))
                {
                    Read = ReadPrefix + Read;
                }

                if (!string.IsNullOrEmpty(ReadAppend))
                {
                    Read = Read + ReadAppend;
                }

                if (LoggingLevel == 1)
                {
                    m_LoggingService.WriteEntry((uint)EventLogEntryCodes.DataRead, new string[] { string.Empty });
                }
                else if (LoggingLevel == 2)
                {
                    m_LoggingService.WriteEntry((uint)EventLogEntryCodes.DataRead, new string[] { Read });
                }

                ReadEvent.Invoke(new Payload(ReadEvent.Id, new PayloadSubject[] { new PayloadSubject(ReadEvent.Subjects[0], Read) }));
            }
        }
    }
}
