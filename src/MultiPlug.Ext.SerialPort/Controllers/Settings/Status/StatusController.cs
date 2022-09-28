using System.Linq;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Components.SerialPort;
using MultiPlug.Ext.SerialPort.Models.Settings.Status;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;

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
                        LoggingLevel = SerialPortSearch.LoggingLevel.Value,
                        LoggingShowControlCharacters = SerialPortSearch.LoggingShowControlCharacters.Value,
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
            Core.Instance.Update(new SerialPortProperties[]
            {
                new SerialPortProperties
                {
                    Guid = theModel.Guid,
                    LoggingLevel = theModel.LoggingLevel,
                    LoggingShowControlCharacters = theModel.LoggingShowControlCharacters
                }
            });

            return new Response
            {
                StatusCode = System.Net.HttpStatusCode.Moved,
                Location = Context.Referrer
            };
        }
    }
}
