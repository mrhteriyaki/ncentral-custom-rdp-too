using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdp_ncentral_tool_custom
{
    public static class LaunchArgs
    {
        public static Guid clientUuid = Guid.NewGuid();
        public static Guid targetUuid = Guid.NewGuid();
        public static string privateKeyFile;

        public static bool sshAccess = false;
        public static bool httpAccess = false;

        public static string ackToken;
        public static string Version;
        public static bool restarted = false;

        public static string serverID;
        public static string xmlFile;
        public static string protocol;
        public static string taskId;

        public static void ProcessArgs(string[] args)
        {
            if(args.Count() == 0)
            {
                throw new Exception("Invalid launch arguments.");
            }

            string arg0 = args[0];
            if (!arg0.StartsWith("swncrc"))
            {
                throw new Exception("Invalid launch args.");
            }

            string[] argData = arg0.Substring(7).Split('&');

            foreach (string arg in argData)
            {
                if (arg.StartsWith("ackToken="))
                {
                    ackToken = arg.Substring(9);
                }
                else if (arg.StartsWith("version="))
                {
                    Version = arg.Substring(8);
                }
                else if (arg.StartsWith("restarted"))
                {
                    restarted = true;
                }
                else if (arg.StartsWith("parameters="))
                {
                    string param64data = arg.Substring(11);
                    string[] parameters = Encoding.UTF8.GetString(Convert.FromBase64String(param64data)).Split('&');
                    foreach (string parm in parameters)
                    {
                        if(parm.StartsWith("serverID="))
                        {
                            serverID = parm.Substring(9);
                        }
                        else if(parm.StartsWith("xmlFile="))
                        {
                            xmlFile = parm.Substring(31);
                        }
                        else if (parm.StartsWith("protocol="))
                        {
                            protocol = parm.Substring(9);
                        }
                        else if (parm.StartsWith("taskId="))
                        {
                            taskId = parm.Substring(7);

                            privateKeyFile = Path.Join(Path.GetTempPath(), "privateKey-" + taskId + ".key");
                        }
                    }
                }
            }
        }
    }
}
