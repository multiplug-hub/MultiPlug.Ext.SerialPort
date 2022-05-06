
using System.Drawing;
using MultiPlug.Base.Attribute;
using MultiPlug.Base.Http;
using MultiPlug.Ext.SerialPort.Properties;

namespace MultiPlug.Ext.SerialPort.Controllers.Assets
{
    [Route("images/*")]
    public class ImageController : AssetsEndpoint
    {
        public Response Get(string theName)
        {
            if(theName.Equals("serialport.png", System.StringComparison.OrdinalIgnoreCase))
            {
                ImageConverter converter = new ImageConverter();
                return new Response { RawBytes = (byte[])converter.ConvertTo(Resources.SerialPort, typeof(byte[])), MediaType = "image/png" };
            }
            else
            {
                return new Response
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
        }
    }
}
