using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class LoggerHelper
    {
        public void WriteLog(Exception ex, string message)
        {
            var filePath = ConfigurationManager.AppSettings["LogFile"] + "\\Logs.txt";
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
                fileInfo.Create();

            using (StreamWriter file = new StreamWriter(filePath, true))
            {
                file.WriteLine("-----------START LOG MESSAGE-------------");
                file.WriteLine("LOG DATE: " + DateTime.Now);
                if (ex != null)
                    file.Write(ex);
                else if (message != "")
                    file.Write(message);
                else
                    file.Write("empty message");
                file.Write(Environment.NewLine);
                file.WriteLine("---------------END LOG MESSAGE----------------");
            }
        }
    }
}
