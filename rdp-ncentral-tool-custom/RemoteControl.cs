using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace rdp_ncentral_tool_custom
{
    public class RemoteControl
    {

        public static bool GetStatus()
        {
            string requestUri = "https://" + LaunchArgs.serverID + "/remoteControlAction.do?method=getRCStatus&taskID=" + ServerXML.remotecontroltaskid + "&randomId=" + Guid.NewGuid().ToString();
            string responseMessage = WebClient.GetRequest(requestUri);
            if (responseMessage.Contains("Completed")) //ends with doesnt work.
            {
                return true;
            }
            //Disconnected.
            return false;
        }
      
        public static void GetStatusTimer(object state)
        {
            try
            {
                GetStatus();
            }
            catch(Exception ex)
            {

            }
        }

        public static void DetailsSubmit(string tunnelType, DateTime dtStart)
        {
            string UriString = "https://" + LaunchArgs.serverID + "/remoteControlAction.do?method=rcDetailsSubmit&tunnelType=" + tunnelType +
                "&availableTunnelTypes=SSH+%7C+HTTP&sourceURI=Public+Endpoint%3A+0.0.0.0%3A0+%7C+Private+EndPoint%3A+0.0.0.0%3A0&targetURI=&startTime=" + dtStart.ToString("yyyy-MM-dd+HH:mm:ss").Replace(":", "%3A") +
                "&endTime=" + DateTime.Now.ToString("yyyy-MM-dd+HH:mm:ss").Replace(":", "%3A") +
                "&rcUUID=" + ServerXML.rcsessionuuid +
                "&randomId=" + ServerXML.rcsessionuuid;
            string response = WebClient.GetRequest(UriString);
        }


        public static string Disconnect()
        {
            string DisconnectUri = "https://" + LaunchArgs.serverID + "/remoteControlAction.do?method=disconnectRCC&taskID=" + ServerXML.remotecontroltaskid + "&userID=" + ServerXML.userid + "&deviceID=" + ServerXML.deviceId;
            return WebClient.GetRequest(DisconnectUri);           
        }

    }
}
