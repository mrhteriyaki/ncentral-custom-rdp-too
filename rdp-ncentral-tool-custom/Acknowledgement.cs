using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace rdp_ncentral_tool_custom
{
    public static class Acknowledgement
    {
        public static void Send()
        {
            if (!LaunchArgs.restarted)
            {
                string response = WebClient.PutRequest("https://" + LaunchArgs.serverID + "/dms/rest/customProtocol/acknowledge?tokenId=" + LaunchArgs.ackToken, "", "application/json");
                Console.WriteLine(response);
            }


        }

    }
}
