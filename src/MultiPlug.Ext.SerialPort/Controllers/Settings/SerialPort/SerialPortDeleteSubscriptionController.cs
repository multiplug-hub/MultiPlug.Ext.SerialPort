using System.Net;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Models.Exchange;

namespace MultiPlug.Ext.SerialPort.Controllers.Settings.SerialPort
{
    [Route("serialport/delete/subscription")]
    public class SerialPortDeleteSubscriptionController : SettingsApp
    {
        public Response Post(string id, string subguid)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(subguid))
            {
                return new Response
                {
                    StatusCode = (HttpStatusCode)422
                };
            }

            if(Core.Instance.DeleteSubscription(id, new WriteSubscription
            {
                Guid = subguid
            }))
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.OK
                };
            }
            else
            {
                return new Response
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }
        }
    }
}
