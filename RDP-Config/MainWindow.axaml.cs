using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RDP_Config
{
    public partial class MainWindow : Window
    {
        static string rdpConfigDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\NcentralProtocolHandler");


        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(rdpConfigDir))
            {
                Directory.CreateDirectory(rdpConfigDir);
            }


            string configFilePath = Path.Join(rdpConfigDir, "config.ini");
            if (File.Exists(configFilePath))
            {
                foreach (string line in File.ReadAllLines(configFilePath))
                {
                    if (line.StartsWith("suffix="))
                    {
                        txtSuffix.Text = line.Split("=")[1];
                    }
                }
            }

            string rdpFilePath = Path.Join(rdpConfigDir, "Default.rdp");
            if (File.Exists(rdpFilePath))
            {
                foreach (string line in File.ReadAllLines(rdpFilePath))
                {
                    if (line.StartsWith("desktopwidth"))
                    {
                        txtWidthInput.Text = line.Split(":")[2];
                    }
                    else if (line.StartsWith("desktopheight"))
                    {
                        txtHeightInput.Text = line.Split(":")[2];
                    }
                }
            }



        }

        class Rdpc
        {
            public string name;
            public string datatype;
            public string value;
            public Rdpc(string newname, string newdatatype, string newvalue)
            {
                name = newname;
                datatype = newdatatype;
                value = newvalue;
            }
        }


        public void OnSaveClicked(object sender, RoutedEventArgs args)
        {

            if (!Directory.Exists(rdpConfigDir))
            {
                Directory.CreateDirectory(rdpConfigDir);
            }

            List<Rdpc> configList = [];

            int width = int.Parse(txtWidthInput.Text);
            configList.Add(new Rdpc("desktopwidth", "i", width.ToString()));

            int height = int.Parse(txtHeightInput.Text);
            configList.Add(new Rdpc("desktopheight", "i", height.ToString()));

            bool fullScreen = (bool)chkFullscreen.IsChecked;
            configList.Add(new Rdpc("screen mode id", "i", fullScreen ? "2" : "1")); // 1= Windowed, 2 = FullScreen

            bool multiMon = (bool)chkMultimonitor.IsChecked;
            configList.Add(new Rdpc("use multimon", "i", multiMon ? "1" : "0")); //multi-monitor 0 = off, 1 = on.

            bool RedirectPrinters = (bool)chkRedirectPrinters.IsChecked;
            configList.Add(new Rdpc("redirectprinters", "i", RedirectPrinters ? "1" : "0"));

            bool RedirectComPorts = (bool)chkRedirectComports.IsChecked;
            configList.Add(new Rdpc("redirectcomports", "i", RedirectComPorts ? "1" : "0"));
            configList.Add(new Rdpc("redirectsmartcards", "i", "0"));
            configList.Add(new Rdpc("redirectwebauthn", "i", "0"));

            bool RedirectClipboards = (bool)chkRedirectClipboard.IsChecked;
            configList.Add(new Rdpc("redirectclipboard", "i", RedirectClipboards ? "1" : "0")); //Clipboard

            bool RedirectDrives = (bool)chkRedirectDrives.IsChecked;
            configList.Add(new Rdpc("drivestoredirect", "s", RedirectDrives ? "*" : "")); //Drive redirection.


            configList.Add(new Rdpc("connection type", "i", "5"));//Connection Type WAN
            configList.Add(new Rdpc("disable menu anims", "i", "1"));

            string configFile = Path.Join(rdpConfigDir, "config.ini");
            if (string.IsNullOrWhiteSpace(txtSuffix.Text))
            {
                if (File.Exists(configFile))
                {
                    File.Delete(configFile);
                }
            }
            else
            {
                StreamWriter CSW = new StreamWriter(configFile);
                CSW.WriteLine("suffix=" + txtSuffix.Text);
                CSW.Close();
            }

            string rdpConfigFile = Path.Join(rdpConfigDir, "Default.rdp");
            if (!File.Exists(rdpConfigFile))
            {
                var fs = File.Create(rdpConfigFile);
                fs.Close();
            }
            string[] existingFile = File.ReadAllLines(rdpConfigFile);

            StreamWriter SW = new StreamWriter(rdpConfigFile);
            foreach (string line in existingFile)
            {
                if (!line.Contains(":"))
                {
                    continue;//empty line.
                }

                var parts = line.Split(':');
                if (configList.Any(cf => cf.name.Equals(parts[0])))
                {
                    continue; //Skip properties updated by config tool.
                }

                SW.WriteLine(line);
            }



            foreach (Rdpc rdpc in configList)
            {
                SW.Write(rdpc.name);
                SW.Write(":");
                SW.Write(rdpc.datatype);
                SW.Write(":");
                SW.WriteLine(rdpc.value);
            }

            SW.Close();




        }
    }
}