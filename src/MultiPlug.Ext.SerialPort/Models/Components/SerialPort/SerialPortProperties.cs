using System.Runtime.Serialization;
using MultiPlug.Base;
using MultiPlug.Base.Exchange;

namespace MultiPlug.Ext.SerialPort.Models.Components.SerialPort
{
    public class SerialPortProperties : MultiPlugBase
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public int LoggingLevel { get; set; }

        [DataMember]
        public Event ReadEvent { get; set; }

        [DataMember]
        public Subscription[] WriteSubscriptions { get; set; }

        [DataMember]
        public string PortName { get; set; }

        [DataMember]
        public int BaudRate { get; set; }

        [DataMember]
        public int Parity { get; set; }

        [DataMember]
        public int DataBits { get; set; }

        [DataMember]
        public int StopBits { get; set; }

        [DataMember]
        public int WriteTimeout { get; set; }

        [DataMember]
        public int ReadTimeout { get; set; }

        /// <summary>
        /// 0 = ReadExisting, 1 = ReadTo
        /// </summary>
        [DataMember]
        public int ReadStrategy { get; set; }

        [DataMember]
        public string ReadTo { get; set; }
        [DataMember]
        public bool ReadTrim { get; set; }
        [DataMember]
        public string ReadPrefix { get; set; }
        [DataMember]
        public string ReadAppend { get; set; }

        [DataMember]
        public bool Enabled { get; set; }
        public bool Opened { get; set; }

        [DataMember]
        public string WritePrefix { get; set; }

        [DataMember]
        public string WriteAppend { get; set; }
    }
}
