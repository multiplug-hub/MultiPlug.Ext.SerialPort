
namespace MultiPlug.Ext.SerialPort.Models.Settings.Home
{
    public class HomeModel
    {
        public string Guid { get; set; }    // Used for Navigation
        public string[] Guids { get; internal set; }
        public int[] BaudRates { get; internal set; }
        public string[] ReadEventIds { get; internal set; }
        public int[] WriteSubscriptionsCounts { get; internal set; }
        public bool[] Openeds { get; internal set; }
        public string[] PortNames { get; internal set; }

        public string[] AvailableBaudRates { get; internal set; }
        public string[] AvailablePortNames { get; internal set; }
    }
}
