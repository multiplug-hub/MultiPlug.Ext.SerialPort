
using System.Collections.Generic;
using System.IO;

namespace MultiPlug.Ext.SerialPort.Components.Utils
{
    static internal class SerialPort
    {

        static internal string[] GetPortNames()
        {
            List<string> PortNames = new List<string>();

            if( Directory.Exists("/dev/"))
            {
                DirectoryInfo d = new DirectoryInfo("/dev/");
                FileInfo[] Files = d.GetFiles("ttyAMA*", SearchOption.TopDirectoryOnly);

                foreach (FileInfo file in Files)
                {
                    PortNames.Add(file.FullName);
                }
            }

            PortNames.AddRange(System.IO.Ports.SerialPort.GetPortNames());

            return PortNames.ToArray();
        }

        static internal string[] GetBaudRates()
        {
            return new string[] { "4800", "9600", "19200", "38400", "57600", "115200", "230400" };
        }
    }
}
