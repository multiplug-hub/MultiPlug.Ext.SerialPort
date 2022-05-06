using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;
using MultiPlug.Ext.SerialPort.Models.Settings.Home;

namespace MultiPlug.Ext.SerialPort.Controllers.Settings.Home
{
    [Route("")]
    public class HomeController : SettingsApp
    {
        public Response Get()
        {
            return new Response
            {
                Model = new HomeModel
                {
                    Guids = Core.Instance.SerialPorts.Select( x => x.Guid).ToArray(),
                    PortNames = Core.Instance.SerialPorts.Select(x => x.PortName).ToArray(),
                    BaudRates = Core.Instance.SerialPorts.Select(x => x.BaudRate).ToArray(),
                    ReadEventIds = Core.Instance.SerialPorts.Select(x => x.ReadEvent.Id).ToArray(),
                    WriteSubscriptionsCounts = Core.Instance.SerialPorts.Select(x => x.WriteSubscriptions.Length).ToArray(),
                    Openeds = Core.Instance.SerialPorts.Select(x => x.Opened).ToArray(),
                    AvailablePortNames = Components.Utils.SerialPort.GetPortNames(),
                    AvailableBaudRates = Components.Utils.SerialPort.GetBaudRates()
                },
                Template = Templates.SettingsHome
            };
        }

        public Response Post(HomeModel theModel)
        {
            if( theModel.PortNames != null && theModel.BaudRates != null && theModel.PortNames.Length > 0)
            {
                var NewSerialPortProperties = new SerialPortProperties[theModel.PortNames.Length];


                for( int i = 0; i < theModel.PortNames.Length; i++)
                {
                    NewSerialPortProperties[i] = new SerialPortProperties();
                    NewSerialPortProperties[i].PortName = theModel.PortNames[i];
                    NewSerialPortProperties[i].BaudRate = theModel.BaudRates[i];
                }

                Core.Instance.Update(NewSerialPortProperties);
            }

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
