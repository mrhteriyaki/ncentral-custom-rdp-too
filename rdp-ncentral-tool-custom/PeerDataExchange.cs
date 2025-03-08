using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace rdp_ncentral_tool_custom
{
    public static class PeerDataExchange
    {
        //Sample response from n-central server:
        //PierDetails||<PeerDataExchange><ACTIVE_TUNNELS>SSH:HTTPS</ACTIVE_TUNNELS></PeerDataExchange>||


        public static string GetPeerDetails()
        {
            string peerXml = @"<PeerDataExchange>
    <REMOTE-CONTROL-TASK-ID>{0}</REMOTE-CONTROL-TASK-ID>
    <ACTIVE_TUNNELS>SSH:HTTPS</ACTIVE_TUNNELS>
    <HTTP_INFO>
        <HTTP_CLIENT_UUID>{1}</HTTP_CLIENT_UUID>
        <HTTP_TARGET_UUID>{2}</HTTP_TARGET_UUID>
    </HTTP_INFO>
</PeerDataExchange>";
            peerXml = string.Format(peerXml, ServerXML.remotecontroltaskid, LaunchArgs.clientUuid, LaunchArgs.targetUuid);

            string UrlQuery = "https://" + LaunchArgs.serverID + "/remoteControlAction.do?method=getPierDetails&applianceID=" + ServerXML.applianceId.ToString() + "&body=" + HttpUtility.UrlEncode(peerXml) + "&randomId=" + Guid.NewGuid().ToString();
            string peerDetails = WebClient.GetRequest(UrlQuery, 2000);

            if (peerDetails.Contains("<ACTIVE_TUNNELS>SSH:HTTPS</ACTIVE_TUNNELS>"))
            {
                LaunchArgs.sshAccess = true;
                LaunchArgs.httpAccess = true;
            }
            else if (peerDetails.Contains("<ACTIVE_TUNNELS>SSH</ACTIVE_TUNNELS>"))
            {
                LaunchArgs.sshAccess = true;
            }
            else if (peerDetails.Contains("<ACTIVE_TUNNELS>HTTPS</ACTIVE_TUNNELS>"))
            {
                LaunchArgs.httpAccess = true;
            }
            return peerDetails;
        }



    }
}
