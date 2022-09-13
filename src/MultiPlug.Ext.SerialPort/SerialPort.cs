using MultiPlug.Base.Exchange;
using MultiPlug.Ext.SerialPort.Models.Load;
using MultiPlug.Extension.Core;
using MultiPlug.Extension.Core.Http;
using MultiPlug.Ext.SerialPort.Controllers;
using MultiPlug.Ext.SerialPort.Properties;

namespace MultiPlug.Ext.SerialPort
{
    public class SerialPort : MultiPlugExtension
    {
        private LoadRoot m_LoadModel = null;

        public SerialPort()
        {
            MultiPlugServices.Logging.RegisterDefinitions(Diagnostics.EventLogDefinitions.DefinitionsId, Diagnostics.EventLogDefinitions.Definitions, true);

            Core.Instance.Init(MultiPlugServices, MultiPlugActions);
        }

        public override RazorTemplate[] RazorTemplates
        {
            get
            {
                return new RazorTemplate[]
                {
                    new RazorTemplate( Templates.SettingsSerialPortNavigation, Resources.SettingsSerialPortNavigation),
                    new RazorTemplate( Templates.SettingsHome, Resources.SettingsHome ),
                    new RazorTemplate( Templates.SettingsAbout, Resources.SettingsAbout ),
                    new RazorTemplate( Templates.SettingsSerialPort, Resources.SettingsSerialPort ),
                    new RazorTemplate( Templates.SettingsStatus, Resources.SettingsStatus )
                };
            }
        }

        public void Load(LoadRoot theConfiguration)
        {
            m_LoadModel = theConfiguration;
        }

        public override void Initialise()
        {
            if (m_LoadModel != null)
            {
                if(m_LoadModel.SerialPorts != null)
                {
                    Core.Instance.Update(m_LoadModel.SerialPorts);
                }
                m_LoadModel = null;
            }
        }

        public override object Save()
        {
            return Core.Instance;
        }

        public override Subscription[] Subscriptions
        {
            get
            {
                return Core.Instance.Subscriptions;
            }
        }
        public override Event[] Events
        {
            get
            {
                return Core.Instance.Events;
            }
        }

        public override void Shutdown()
        {
            Core.Instance.Shutdown();
        }
    }
}
