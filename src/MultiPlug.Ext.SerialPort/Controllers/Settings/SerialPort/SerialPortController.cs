using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;
using MultiPlug.Ext.SerialPort.Models.Settings.SerialPort;

namespace MultiPlug.Ext.SerialPort.Controllers.Settings.SerialPort
{
    [Route("serialport")]
    public class SerialPortController : SettingsApp
    {
        public Response Get(string id)
        {
            SerialPortProperties SerialPortSearch = null;

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
                    Model = new SerialPortModel
                    {
                        Guid = SerialPortSearch.Guid,
                        Enabled = SerialPortSearch.Enabled,
                        PortName = SerialPortSearch.PortName,
                        BaudRate = SerialPortSearch.BaudRate,
                        Parity = SerialPortSearch.Parity,
                        DataBits = SerialPortSearch.DataBits,
                        StopBits = SerialPortSearch.StopBits,
                        ReadEventId = SerialPortSearch.ReadEvent.Id,
                        ReadEventDescription = SerialPortSearch.ReadEvent.Description,
                        ReadEventSubject = SerialPortSearch.ReadEvent.Subjects[0],
                        ReadStrategy = SerialPortSearch.ReadStrategy,
                        ReadTo = SerialPortSearch.ReadTo,
                        ReadTimeout = SerialPortSearch.ReadTimeout,
                        ReadTrim = SerialPortSearch.ReadTrim,
                        ReadPrefix = SerialPortSearch.ReadPrefix,
                        ReadAppend = SerialPortSearch.ReadAppend,
                        WriteTimeout = SerialPortSearch.WriteTimeout,
                        WritePrefix = SerialPortSearch.WritePrefix,
                        WriteAppend = SerialPortSearch.WriteAppend,
                        WriteSubscriptionGuids = SerialPortSearch.WriteSubscriptions.Select( x => x.Guid).ToArray(),
                        WriteSubscriptionIds = SerialPortSearch.WriteSubscriptions.Select(x => x.Id).ToArray(),
                        AvailablePortNames = Components.Utils.SerialPort.GetPortNames(),
                        AvailableBaudRates = Components.Utils.SerialPort.GetBaudRates()
                    },
                    Template = Templates.SettingsSerialPort
                };
            }
        }

        public Response Post(SerialPortModel theModel)
        {
            Subscription[] Subscriptions;

            if (theModel.WriteSubscriptionGuids != null)
            {
                Subscriptions = new Subscription[theModel.WriteSubscriptionGuids.Length];

                for (int i = 0; i< theModel.WriteSubscriptionGuids.Length; i++)
                {
                    Subscriptions[i] = new Subscription { Guid = theModel.WriteSubscriptionGuids[i], Id = theModel.WriteSubscriptionIds[i] };
                }
            }
            else
            {
                Subscriptions = new Subscription[0];
            }

            Core.Instance.Update(new SerialPortProperties[]
            {
                new SerialPortProperties
                {
                    Guid = theModel.Guid,
                    LoggingLevel = -1,
                    Enabled = theModel.Enabled,
                    PortName = theModel.PortName,
                    BaudRate = theModel.BaudRate,
                    Parity = theModel.Parity,
                    DataBits = theModel.DataBits,
                    StopBits = theModel.StopBits,
                    ReadEvent = new Event
                    {
                        Id = theModel.ReadEventId,
                        Description = theModel.ReadEventDescription,
                        Subjects = new string[] { theModel.ReadEventSubject }
                    },
                    ReadStrategy = theModel.ReadStrategy,
                    ReadTo = theModel.ReadTo,
                    ReadTimeout = theModel.ReadTimeout,
                    ReadTrim = theModel.ReadTrim,
                    ReadPrefix = theModel.ReadPrefix,
                    ReadAppend = theModel.ReadAppend,
                    WriteTimeout = theModel.WriteTimeout,
                    WritePrefix = theModel.WritePrefix,
                    WriteAppend = theModel.WriteAppend,
                    WriteSubscriptions = Subscriptions
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
