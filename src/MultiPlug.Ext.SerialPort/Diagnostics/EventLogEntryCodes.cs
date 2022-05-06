
namespace MultiPlug.Ext.SerialPort.Diagnostics
{
    internal enum EventLogEntryCodes
    {
        Reserved = 0,
        SourceSerialPort = 1,
        
        // Setup 21-99
        PortOpened = 21,
        PortClosed = 22,


        // Data 100+
        DataRead = 100,
        DataWrite = 101,

        // Exceptions
        Exception = 200
    }
}
