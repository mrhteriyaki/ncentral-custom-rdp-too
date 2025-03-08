using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdp_ncentral_tool_custom
{
    public class RdpClientConfig
    {

        public static string GenerateRDP(string hostname, string rdpConfigDir)
        {
            string rdpFilePath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp\\" + hostname + ".rdp");

            string[] defaultFile = [];
            string defaultFilePath = Path.Join(rdpConfigDir, "Default.rdp");
            if (File.Exists(defaultFilePath))
            {
                defaultFile = File.ReadAllLines(defaultFilePath);
            }

            string configFilePath = Path.Join(rdpConfigDir, "config.ini");
            string[] configFile = [];
            if (File.Exists(configFilePath))
            {
                File.ReadAllLines(configFilePath);
            }

            StreamWriter rdpSW = new StreamWriter(rdpFilePath);
            foreach (string configLine in defaultFile)
            {
                if (configLine.Contains("full address:s:"))
                {
                    continue;
                }
                rdpSW.WriteLine(configLine);
            }

            string fullAddress = "127.0.0.1:" + SshConnection.GetLocalSSHPort();

            foreach (string configLine in configFile)
            {
                if (configLine.StartsWith("suffix="))
                {
                    string suffix = configLine.Split("=")[1];
                    fullAddress = hostname + "." + suffix + ":" + SshConnection.GetLocalSSHPort();
                }
            }
            rdpSW.WriteLine("full address:s:" + fullAddress);
            rdpSW.Close();
            
            return rdpFilePath;
        }
    }
}
