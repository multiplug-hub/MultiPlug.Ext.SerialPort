using System.Linq;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Exchange;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Models.Components.SerialPort;
using MultiPlug.Ext.SerialPort.Models.Settings.SerialPort;
using MultiPlug.Ext.SerialPort.Models.Exchange;

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
                        Enabled = SerialPortSearch.Enabled.Value,
                        PortName = SerialPortSearch.PortName,
                        BaudRate = SerialPortSearch.BaudRate.Value,
                        Parity = SerialPortSearch.Parity.Value,
                        DataBits = SerialPortSearch.DataBits.Value,
                        StopBits = SerialPortSearch.StopBits.Value,
                        ReadEventId = SerialPortSearch.ReadEvent.Id,
                        ReadEventDescription = SerialPortSearch.ReadEvent.Description,
                        ReadEventSubject = SerialPortSearch.ReadEvent.Subjects[0],
                        ReadStrategy = SerialPortSearch.ReadStrategy.Value,
                        ReadTo = SerialPortSearch.ReadTo,
                        ReadTimeout = SerialPortSearch.ReadTimeout.Value,
                        ReadTrim = SerialPortSearch.ReadTrim.Value,
                        ReadPrefix = SerialPortSearch.ReadPrefix,
                        ReadAppend = SerialPortSearch.ReadAppend,
                        WriteTimeout = SerialPortSearch.WriteTimeout.Value,
                        WritePrefix = SerialPortSearch.WritePrefix,
                        WriteSeparator = SerialPortSearch.WriteSeparator,
                        WriteAppend = SerialPortSearch.WriteAppend,
                        WriteSubscriptionGuids = SerialPortSearch.WriteSubscriptions.Select( x => x.Guid).ToArray(),
                        WriteSubscriptionIds = SerialPortSearch.WriteSubscriptions.Select(x => x.Id).ToArray(),
                        WriteSubscriptionWritePrefixs = SerialPortSearch.WriteSubscriptions.Select(x => x.WritePrefix).ToArray(),
                        WriteSubscriptionWriteSeparators = SerialPortSearch.WriteSubscriptions.Select(x => x.WriteSeparator).ToArray(),
                        WriteSubscriptionWriteSuffixs = SerialPortSearch.WriteSubscriptions.Select(x => x.WriteAppend).ToArray(),
                        AvailablePortNames = Components.Utils.SerialPort.GetPortNames(),
                        AvailableBaudRates = Components.Utils.SerialPort.GetBaudRates(),
                        ReadRetryAfter = SerialPortSearch.ReadRetryAfter.Value
                    },
                    Template = Templates.SettingsSerialPort
                };
            }
        }

        public Response Post(SerialPortModel theModel)
        {
            WriteSubscription[] Subscriptions;

            if (theModel.WriteSubscriptionGuids != null)
            {
                Subscriptions = new WriteSubscription[theModel.WriteSubscriptionGuids.Length];

                for (int i = 0; i< theModel.WriteSubscriptionGuids.Length; i++)
                {
                    Subscriptions[i] = new WriteSubscription
                    {
                        Guid = theModel.WriteSubscriptionGuids[i],
                        Id = theModel.WriteSubscriptionIds[i],
                        WritePrefix = theModel.WriteSubscriptionWritePrefixs[i],
                        WriteSeparator = theModel.WriteSubscriptionWriteSeparators[i],
                        WriteAppend = theModel.WriteSubscriptionWriteSuffixs[i]
                    };
                }
            }
            else
            {
                Subscriptions = new WriteSubscription[0];
            }

            Core.Instance.Update(new SerialPortProperties[]
            {
                new SerialPortProperties
                {
                    Guid = theModel.Guid,
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
                    WriteSeparator = theModel.WriteSeparator,
                    WriteAppend = theModel.WriteAppend,
                    WriteSubscriptions = Subscriptions,
                    ReadRetryAfter = theModel.ReadRetryAfter
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
