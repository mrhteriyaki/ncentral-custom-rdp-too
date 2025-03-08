using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace rdp_ncentral_tool_custom
{
    public static class SshConnection
    {
        public static SshClient sshClient;
        static int sshPort = new Random().Next(15000, 30001); //The local listen port that the mstsc rdp client will connect to.

        public static string GetLocalSSHPort()
        {
            return sshPort.ToString();
        }

        public static void Connect()
        {
            PrivateKeyFile[] privateKeyFileArray = new PrivateKeyFile[1];
            privateKeyFileArray[0] = new PrivateKeyFile(LaunchArgs.privateKeyFile);

            PrivateKeyAuthenticationMethod certAuthMethod = new PrivateKeyAuthenticationMethod(ServerXML.centralserverusername, privateKeyFileArray);
            PasswordAuthenticationMethod passwordAuthMethod = new PasswordAuthenticationMethod(ServerXML.centralserverusername, ServerXML.centralserverpassword);

            AuthenticationMethod[] authMethods = new AuthenticationMethod[2]
            {
                (AuthenticationMethod) certAuthMethod,
                (AuthenticationMethod) passwordAuthMethod
            };

            sshClient = new SshClient(new ConnectionInfo(ServerXML.centralserverip, 22, ServerXML.centralserverusername, authMethods));

            ForwardedPort port = (ForwardedPort)new ForwardedPortLocal("127.0.0.1", (uint)sshPort, "localhost", (uint)ServerXML.targetport);

            try
            {
                sshClient.ErrorOccurred += new EventHandler<ExceptionEventArgs>(ErrorOccurred);
                sshClient.HostKeyReceived += new EventHandler<HostKeyEventArgs>(HostKeyEvent);
                sshClient.Connect();
                sshClient.AddForwardedPort(port);
                port.RequestReceived += new EventHandler<PortForwardEventArgs>(RequestReceived);
                port.Exception += new EventHandler<ExceptionEventArgs>(ExceptiontReceived);
                port.Start();               
            }
            catch (Exception ex)
            {
                LaunchArgs.sshAccess = false;
                throw ex;
            }
        }

        private static void RequestReceived(object sender, PortForwardEventArgs e)
        {

        }

        private static void ExceptiontReceived(object sender, ExceptionEventArgs e)
        {

            
        }

        private static void ErrorOccurred(object sender, ExceptionEventArgs e)
        {
            //Need to call RemoteControl.GetStatus() to repair connection.
        }

        private static void HostKeyEvent(object sender, HostKeyEventArgs hkea)
        {
        }

        public static void Disconnect()
        {
            sshClient.Disconnect();
        }



    }
}
