using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;

namespace MultiPlug.Ext.SerialPort.Controllers.Settings.SerialPort
{
    [Route("serialport/delete")]
    public class SerialPortDeleteController : SettingsApp
    {
        public Response Post(string id)
        {
            if (Core.Instance.Remove(new SerialPortProperties { Guid = id }))
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.OK };
            }
            else
            {
                return new Response { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
        }
    }
}
