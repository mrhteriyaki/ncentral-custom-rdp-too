using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace rdp_ncentral_tool_custom
{
    public static class ServerXML
    {
        static List<string> xmlArguments;

        public static int targetport;
        public static int timeout;
        public static int remotecontroltaskid;
        public static string centralserverip;
        public static int userid;
        public static int localport;
        public static string centralserverpassword;
        public static string centralserverusername;
        public static int applianceId;
        public static string rcsessionuuid;
        public static string deviceId;
        public static string deviceName;
        public static string stunServers;

        public static void Download()
        {
            string uriPath = "https://" + LaunchArgs.serverID + LaunchArgs.xmlFile;
            string xmlData = WebClient.GetRequest(uriPath);
            string tempDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"\Temp");

            ImportData(xmlData, "Argument");
        }

        public static void ImportData(string xmlData, string nodeName)
        {
            if (string.IsNullOrEmpty(LaunchArgs.privateKeyFile))
            {
                throw new Exception("privateKeyFile variable is not set");
            }

            XDocument xmlDoc = XDocument.Parse(xmlData);
            xmlArguments = xmlDoc.Descendants(nodeName).Select(arg => arg.Value).ToList();

            StreamWriter SW = new StreamWriter(LaunchArgs.privateKeyFile, false);
            string privateKey = GetValue("privatekey");

            foreach (string privateKeyLine in privateKey.Split("|||"))
            {
                SW.WriteLine(privateKeyLine);
            }
            SW.Close();

            
            remotecontroltaskid = int.Parse(GetValue("remotecontrolTaskId"));
            centralserverip = GetValue("centralserverip");   
            if(string.IsNullOrEmpty(LaunchArgs.serverID))
            {
                LaunchArgs.serverID = centralserverip;
            }
            targetport = int.Parse(GetValue("targetport"));
            timeout = int.Parse(GetValue("timeout"));
            userid = int.Parse(GetValue("userid"));
            localport = int.Parse(GetValue("localport"));
            centralserverpassword = GetValue("centralserverpassword");
            centralserverusername = GetValue("centralserverusername");
            applianceId = int.Parse(GetValue("applianceid"));
            rcsessionuuid = GetValue("rcsessionuuid");
            deviceId = GetValue("deviceId");
            deviceName = GetValue("deviceName");
            stunServers = GetValue("stunServers");
        }

        static string GetValue(string key)
        {
            try
            {
                foreach (string xArg in xmlArguments)
                {
                    if (xArg.ToLower().StartsWith(key.ToLower()))
                    {
                        return xArg.Split(':')[1];
                    }
                }
                return "";

            }
            catch
            {
                Console.WriteLine("Error getting xml value: " + key);
                return "";
            }

        }

        public static string GetDeviceCleanName()
        {
            string cleanName = deviceName;
            if (cleanName.Contains("("))
            {
                cleanName = deviceName.Substring(0, deviceName.IndexOf("(")).Trim();
            }

            return Regex.Replace(cleanName, "[^a-zA-Z0-9-]", "");
        }


    }
}
