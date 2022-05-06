using System.Linq;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Components.SerialPort;
using MultiPlug.Ext.SerialPort.Models.Settings.Status;

namespace MultiPlug.Ext.SerialPort.Controllers.Settings.Status
{
    [Route("status")]
    public class StatusController : SettingsApp
    {
        public Response Get(string id)
        {
            SerialPortComponent SerialPortSearch = null;

            if (!string.IsNullOrEmpty(id))
            {
                SerialPortSearch = Core.Instance.SerialPorts.FirstOrDefault(t => t.Guid == id);
            }

            if (SerialPortSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
            else
            {
                return new Response
                {
                    Model = new StatusModel
                    {
                        Guid = SerialPortSearch.Guid,
                        LoggingLevel = SerialPortSearch.LoggingLevel,
                        Log = SerialPortSearch.GetTraceLog()
                    },
                    Subscriptions = new Subscription[]
                    {
                        new Subscription { Guid = "LogEventId", Id = SerialPortSearch.LogEventId },
                    },
                    Template = Templates.SettingsStatus
                };
            }
        }

        public Response Post(StatusModel theModel)
        {
            var SerialPortsSearch = Core.Instance.SerialPorts.FirstOrDefault(Lane => Lane.Guid == theModel.Guid);

            if (SerialPortsSearch == null)
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            SerialPortsSearch.UpdateProperties(theModel.LoggingLevel);

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
