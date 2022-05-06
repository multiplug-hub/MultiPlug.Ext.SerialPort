using MultiPlug.Base.Http;
using MultiPlug.Extension.Core.Attribute;

namespace MultiPlug.Ext.SerialPort.Controllers.Settings
{
    [Name("Serial Port")]
    [ViewAs(ViewAs.Partial)]
    [HttpEndpointType(HttpEndpointType.Settings)]
    public class SettingsApp : Controller
    {
    }
}
