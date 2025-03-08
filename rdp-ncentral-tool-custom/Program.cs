using rdp_ncentral_tool_custom;
using Renci.SshNet;
using Renci.SshNet.Messages.Transport;
using System.Diagnostics;
using System.IO.Compression;


string rdpConfigDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\NcentralProtocolHandler");

LoggerClass Logger = new LoggerClass(Path.Join(rdpConfigDir, "log.txt"));
bool rdpMode = false;
if (args.Length == 1 && args[0].EndsWith(".jnlp"))
{
    string jnlpData = File.ReadAllText(args[0]);
    LaunchArgs.privateKeyFile = Path.Join(Path.GetTempPath(),Guid.NewGuid().ToString() + ".key");
    ServerXML.ImportData(jnlpData, "argument");
}
else
{
    LaunchArgs.ProcessArgs(args);
    Acknowledgement.Send(); //Send Ack to /dms/rest/customProtocol/acknowledge
    ServerXML.Download(); //Download XML from /webstart/tmp/123/456/789.xml
    rdpMode = true;
}

Logger.LogMessage("Device name: " + ServerXML.deviceName);

//Wait until connection established on N-Central server, checking status at /remoteControlAction.do?method=getRCStatus
Logger.LogMessage("Waiting for client device to connect with rmm server.");
while (!RemoteControl.GetStatus())
{
    Thread.Sleep(300);
}
Logger.LogMessage("Remote device connected to RMM Server.");

//Regular CPH has a 3 second wait.
//50ms resulted in failed to get peer details.
if(rdpMode)
{
    Thread.Sleep(1500);
}
else
{
    Thread.Sleep(3000);
}


Logger.LogMessage("Getting Peer Details");
try
{
    PeerDataExchange.GetPeerDetails(); // remoteControlAction.do?method=getPierDetails
}
catch
{
    //Need to add user alert here and implement http tunnel failover.
    Logger.LogMessage("Failed to get peer details - Exiting.");
    RemoteControl.Disconnect();
    return;
}

Logger.LogMessage("Starting SSH port forward on local port " + SshConnection.GetLocalSSHPort());
SshConnection.Connect();


DateTime connectionStartTime = DateTime.Now;

if (!RemoteControl.GetStatus())
{
    //Client device has disconnected from N-central server, exit.
    Logger.LogMessage("Client device has disconnected.");
    return;
}


//Failover to HTTP Tunnel if ssh failed.
if (!LaunchArgs.sshAccess)
{
    //Use http tunnel as backup - not complete
    HttpConnection.StartTunnel();
    Logger.LogMessage("Starting failover HTTP Tunnel");
}


Logger.LogMessage(LaunchArgs.sshAccess ? "Details Submit - SSH" : "Details submit - HTTP");

try
{
    RemoteControl.DetailsSubmit(LaunchArgs.sshAccess ? "SSH" : "HTTP", connectionStartTime);
}
catch (Exception ex)
{
    Logger.LogMessage("Error: DetailsSubmit failed with exception " + ex.Message);
    return;
}


//Timer sends keep alive to RMM Server, session will disconnect without it after short time period.
TimerCallback KACB = new TimerCallback(RemoteControl.GetStatusTimer);
Timer KeepaliveTimer = new Timer(KACB, (object)null, 8000, 6000);



if(rdpMode)
{

    //need to cleanup the hostname here, symbols can break it.
    string hostname = ServerXML.GetDeviceCleanName();
    if (hostname.Contains(" "))
    {
        hostname = hostname.Split(" ")[0];
    }

    string rdpFilePath = RdpClientConfig.GenerateRDP(hostname, rdpConfigDir);

    Process rdpProc = new Process();
    rdpProc.StartInfo.FileName = @"C:\Windows\System32\mstsc.exe";
    rdpProc.StartInfo.Arguments = rdpFilePath;
    rdpProc.Start();

    //Wait for RDP to exit and cleanup.
    rdpProc.WaitForExit();
    File.Delete(rdpFilePath);
}
else
{
    Process puttyProc = new Process();
    puttyProc.StartInfo.FileName = @"C:\Program Files\Putty\putty.exe";
    puttyProc.StartInfo.Arguments = "-ssh 127.0.0.1 -P" + SshConnection.GetLocalSSHPort();
    puttyProc.Start();

    puttyProc.WaitForExit();
}

KeepaliveTimer.Dispose();


if (LaunchArgs.sshAccess)
{
    try
    {
        SshConnection.Disconnect();
    }
    catch (Exception ex)
    {
        Logger.LogMessage("Error disconnecting ssh connection: " + ex.Message);
    }
}

try
{
    string disconnectMessage = RemoteControl.Disconnect();
    Logger.LogMessage("Disconnect confirmation: " + disconnectMessage);
}
catch (Exception ex)
{
    Logger.LogMessage("Disconnect request failed: " + ex.Message);
}
//doesnt seem to be working, returning error page.

File.Delete(LaunchArgs.privateKeyFile);

Logger.LogMessage("Finished");

