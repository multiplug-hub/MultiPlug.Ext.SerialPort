using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Exchange.API;
using MultiPlug.Ext.SerialPort.Diagnostics;
using MultiPlug.Ext.SerialPort.Components.Utils;
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
            LoggingLevel = 0;
            LoggingShowControlCharacters = false;
            BaudRate = 0;
            Parity = 0;
            DataBits = 0;
            StopBits = 0;
            ReadTimeout = 0;
            WriteTimeout = 0;
            ReadStrategy = 0;
            ReadTrim = false;
            Enabled = false;

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

            if(theNewProperties.LoggingLevel != null && theNewProperties.LoggingLevel != LoggingLevel)
            {
                LoggingLevel = theNewProperties.LoggingLevel;
            }

            if(theNewProperties.LoggingShowControlCharacters != null && theNewProperties.LoggingShowControlCharacters != LoggingShowControlCharacters)
            {
                LoggingShowControlCharacters = theNewProperties.LoggingShowControlCharacters;
            }

            if (theNewProperties.PortName != null && PortName != theNewProperties.PortName)
            {
                PortName = theNewProperties.PortName;
                FlagBeginConnect = true;
            }

            if(theNewProperties.BaudRate != null && BaudRate != theNewProperties.BaudRate )
            {
                BaudRate = theNewProperties.BaudRate;
                FlagBeginConnect = true;
            }

            if(theNewProperties.Parity != null && Parity != theNewProperties.Parity)
            {
                Parity = theNewProperties.Parity;
                FlagBeginConnect = true;
            }

            if (theNewProperties.DataBits != null && DataBits != theNewProperties.DataBits)
            {
                DataBits = theNewProperties.DataBits;
                FlagBeginConnect = true;
            }

            if (theNewProperties.StopBits != null && StopBits != theNewProperties.StopBits)
            {
                StopBits = theNewProperties.StopBits;
                FlagBeginConnect = true;
            }

            if(theNewProperties.ReadTo != null && ReadTo != theNewProperties.ReadTo )
            {
                ReadTo = theNewProperties.ReadTo;
                m_ReadTo = ReadTo != null ? Regex.Unescape(ReadTo) : string.Empty;
                FlagBeginConnect = true;
            }

            if(theNewProperties.ReadTimeout != null && ReadTimeout != theNewProperties.ReadTimeout)
            {
                ReadTimeout = theNewProperties.ReadTimeout;
                FlagBeginConnect = true;
            }

            if(theNewProperties.WriteTimeout != null && WriteTimeout != theNewProperties.WriteTimeout)
            {
                WriteTimeout = theNewProperties.WriteTimeout;
                FlagBeginConnect = true;
            }

            if(theNewProperties.ReadStrategy != null && ReadStrategy != theNewProperties.ReadStrategy)
            {
                ReadStrategy = theNewProperties.ReadStrategy;
                FlagBeginConnect = true;
            }

            if(theNewProperties.WritePrefix != null && WritePrefix != theNewProperties.WritePrefix)
            {
                WritePrefix = theNewProperties.WritePrefix;
                m_WritePrefix = WritePrefix != null ? Regex.Unescape(WritePrefix) : string.Empty;
            }

            if(theNewProperties.WriteAppend != null && WriteAppend != theNewProperties.WriteAppend)
            {
                WriteAppend = theNewProperties.WriteAppend;
                m_WriteAppend = WriteAppend != null ? Regex.Unescape(WriteAppend) : string.Empty;
            }

            if (theNewProperties.ReadTrim != null && ReadTrim != theNewProperties.ReadTrim)
            {
                ReadTrim = theNewProperties.ReadTrim;
            }

            if (theNewProperties.ReadPrefix != null && ReadPrefix != theNewProperties.ReadPrefix)
            {
                ReadPrefix = theNewProperties.ReadPrefix;
            }

            if (theNewProperties.ReadAppend != null && ReadAppend != theNewProperties.ReadAppend)
            {
                ReadAppend = theNewProperties.ReadAppend;
            }

            if ( theNewProperties.ReadEvent != null)
            {
                if( Event.Merge(ReadEvent, theNewProperties.ReadEvent) ) { FlagEventUpdated = true; }
            }

            if (theNewProperties.WriteSubscriptions != null && UpdateMachineReadySubscriptions(theNewProperties.WriteSubscriptions))
            {
                FlagSubscriptionUpdated = true;
            }

            // Keep this last
            if (theNewProperties.Enabled != null && Enabled != theNewProperties.Enabled)
            {
                Enabled = theNewProperties.Enabled;

                if (Enabled.Value)
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

            if(Enabled.Value && FlagBeginConnect)
            {
                Open();
            }
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

            m_SerialPort = new DotNetSerialPort();

            try
            {
                PortExists(PortName);

                m_SerialPort.PortName = PortName;
                m_SerialPort.BaudRate = BaudRate.Value;

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

                m_SerialPort.WriteTimeout = WriteTimeout.Value == 0 ? DotNetSerialPort.InfiniteTimeout : WriteTimeout.Value;
                m_SerialPort.ReadTimeout = ReadTimeout.Value == 0 ? DotNetSerialPort.InfiniteTimeout : ReadTimeout.Value;
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

                        Close();

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
            lock (this) // Read thread and User Action will call this.
            {
                if (m_SerialPort == null)
                {
                    Opened = false;
                    return;
                }

                if (m_SerialPort.IsOpen)
                {
                    try
                    {
                        m_SerialPort.Close();
                    }
                    catch (System.IO.IOException theException)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
                    }
                }

                m_SerialPort.Dispose();
                m_SerialPort = null;
                GC.Collect(); // Updates the Available Serial ports on Windows if a Port was removed.
                GC.WaitForPendingFinalizers();

                try
                {
                    PortExists(PortName);
                }
                catch (InvalidOperationException theException)
                {
                    m_LoggingService.WriteEntry((uint)EventLogEntryCodes.Exception, new string[] { theException.Message });
                }

                if(Opened)
                {
                    if (LoggingLevel > 1)
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.PortClosed);
                    }
                }


                m_ReadCancellationToken.Cancel();
                m_ReadCancellationToken = new CancellationTokenSource();

                Opened = false;
            }
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

            switch ( ReadStrategy )
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
                    catch (System.IO.IOException theException)
                    {
                        throw (theException);
                    }
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
                    catch(System.IO.IOException theException)
                    {
                        throw (theException);
                    }
                    break;
            }

            if (string.IsNullOrEmpty(Read)) // Linux/Mono will read empty on the physical removal of a Serial Port
            {
                try
                {
                    PortExists(PortName);
                }
                catch(InvalidOperationException theException)
                {
                    // We record the error on Close()
                    throw (theException);
                }
            }
            else
            {
                if(ReadTrim.Value)
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
                    if(LoggingShowControlCharacters.Value == true)
                    {
                        StringBuilder SB = new StringBuilder();

                        foreach (char aChar in Read)
                        {
                            if (char.IsControl(aChar))
                            {
                                SB.AppendFormat(ControlCharacters.Lookup(Convert.ToUInt32(aChar)));
                            }
                            else
                            {
                                SB.Append(aChar);
                            }
                        }

                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.DataRead, new string[] { SB.ToString() });
                    }
                    else
                    {
                        m_LoggingService.WriteEntry((uint)EventLogEntryCodes.DataRead, new string[] { Read });
                    }
                }

                ReadEvent.Invoke(new Payload(ReadEvent.Id, new PayloadSubject[] { new PayloadSubject(ReadEvent.Subjects[0], Read) }));
            }
        }
        /// <summary>
        /// Checks if a Port Name exists.
        /// </summary>
        /// <param name="thePortName">
        /// The Port Name
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The specified port doesn't exists. 
        /// </exception>
        private static void PortExists( string thePortName )
        {
            if (!Utils.SerialPort.GetPortNames().Contains(thePortName))
            {
                throw (new InvalidOperationException("The port '" + thePortName + "' does not exist."));
            }
        }

        internal new void Dispose()
        {
            try
            {
                // Bug Created https://github.com/British-Systems/MultiPlug/issues/52
                m_LoggingService.Delete();
            }
            catch (NotImplementedException)
            {
            }

            base.Dispose();
        }
    }
}
