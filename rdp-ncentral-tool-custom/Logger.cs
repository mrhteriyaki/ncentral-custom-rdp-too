using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdp_ncentral_tool_custom
{
    public class LoggerClass
    {
        string logFile;
        public LoggerClass(string LogPath)
        {
            logFile = LogPath;
            if(File.Exists(logFile))
            {
                File.Delete(logFile);
            }
            
        }
        public void LogMessage(string message)
        {
            StreamWriter logSW = new StreamWriter(logFile, true);
            logSW.WriteLine(message);
            Console.WriteLine(message);
            logSW.Close();
        }
    }
}
