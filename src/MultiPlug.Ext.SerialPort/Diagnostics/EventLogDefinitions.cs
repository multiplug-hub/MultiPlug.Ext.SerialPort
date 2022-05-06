using MultiPlug.Base.Diagnostics;

namespace MultiPlug.Ext.SerialPort.Diagnostics
{
    internal class EventLogDefinitions
    {
        internal const string DefinitionsId = "MultiPlug.Ext.SerialPort.EN";

        internal static EventLogDefinition[] Definitions { get; set; } = new EventLogDefinition[]
        {
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.SourceSerialPort, Source = (uint) EventLogEntryCodes.Reserved,  StringFormat = "Serial Port", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.PortOpened, Source = (uint) EventLogEntryCodes.SourceSerialPort,  StringFormat = "Opened", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.PortClosed, Source = (uint) EventLogEntryCodes.SourceSerialPort,  StringFormat = "Closed", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.DataRead, Source = (uint) EventLogEntryCodes.SourceSerialPort,  StringFormat = "Data Read: {0}", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.DataWrite, Source = (uint) EventLogEntryCodes.SourceSerialPort, StringFormat = "Data Writen: {0}", Type = EventLogEntryType.Information  },
            new EventLogDefinition { Code = (uint) EventLogEntryCodes.Exception, Source = (uint) EventLogEntryCodes.SourceSerialPort, StringFormat = "Exception Message: {0}", Type = EventLogEntryType.Error  },
        };
    }
}
