
namespace MultiPlug.Ext.SerialPort.Models.Settings.SerialPort
{
    public class SerialPortModel
    {
        public string Guid { get; set; }
        public bool Enabled { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int Parity { get; set; }
        public int DataBits { get; set; }
        public int StopBits { get; set; }
        public string ReadEventId { get; set; }
        public string ReadEventDescription { get; set; }
        public string ReadEventSubject { get; set; }
        public int ReadStrategy { get; set; }
        public string ReadTo { get; set; }
        public int ReadTimeout { get; set; }
        public bool ReadTrim { get; set; }
        public string ReadPrefix { get; set; }
        public string ReadAppend { get; set; }
        public bool WriteLine { get; set; }
        public int WriteTimeout { get; set; }
        public string WritePrefix { get; set; }
        public string WriteSeparator { get; set; }
        public string WriteAppend { get; set; }
        public string[] WriteSubscriptionGuids { get; set; }
        public string[] WriteSubscriptionIds { get; set; }
        public string[] WriteSubscriptionWritePrefixs { get; set; }
        public string[] WriteSubscriptionWriteSeparators { get; set; }
        public string[] WriteSubscriptionWriteSuffixs { get; set; }
        public string[] AvailablePortNames { get; set; }
        public string[] AvailableBaudRates { get; set; }
        public int ReadRetryAfter { get; set; }
    }
}
