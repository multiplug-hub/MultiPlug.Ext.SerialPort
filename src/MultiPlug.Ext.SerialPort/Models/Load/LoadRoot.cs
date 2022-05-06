using System.Runtime.Serialization;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;

namespace MultiPlug.Ext.SerialPort.Models.Load
{
    public class LoadRoot
    {
        [DataMember]
        public SerialPortProperties[] SerialPorts { get; set; } = new SerialPortProperties[0];
    }
}
