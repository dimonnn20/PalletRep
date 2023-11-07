using log4net.Repository.Hierarchy;
using PalletRep.Model;
using PalletRep.Logger;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Logic
{
    internal class SFTPConnection
    {
        private readonly string Host;
        private readonly string Username = ConfigurationManager.AppSettings["Username"];
        private readonly string Password = ConfigurationManager.AppSettings["Password"];
        //private string _remotePath = "/userapp/software/draw/log/leap.log";
        private readonly string RemotePath;
        private readonly LeapLogParser LeapLogParser;

        private List<string> lines;

        public SFTPConnection(LeapLogParser leap)
        {
            try
            {
                Host = ConfigurationManager.AppSettings["IP"];
                RemotePath = ConfigurationManager.AppSettings["RemotePath"];
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Exception to pars IP or connectionString from App.config ", ex);
            }
            LeapLogParser = leap;
        }

        public void CheckAndProceedFile()
        {
                //    using (SftpClient sftp = new SftpClient(_host, _username, _password))
                //    {
                //        sftp.Connect();
                //        if (sftp.Exists(_remotePath))
                //        {
                //            lines = new List<string>();
                //            using (SftpFileStream fileStream = sftp.OpenRead(_remotePath))
                //            {
                //                using (StreamReader reader = new StreamReader(fileStream))
                //                {
                //                    string line;
                //                    while ((line = reader.ReadLine()) != null)
                //                    {
                //                        lines.Add(line);
                //                    }
                //                    Proceed(lines);
                //                    sftp.Delete(_remotePath);
                //                    sftp.Disconnect();
                //                }

                //            }
                //        }
                //    }

                if (File.Exists(RemotePath))
                {
                Logger.Logger.Log.Info("File leap.log exists");
                lines = new List<string>();
                    using (var stream = new FileStream(RemotePath, FileMode.OpenOrCreate))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                lines.Add(line);
                            }
                            Logger.Logger.Log.Info("Infromation from file leap.log readed successfuly");
                            LeapLogParser.Proceed(lines);
                        }
                    }
                    File.Delete(RemotePath);
                    Logger.Logger.Log.Info("File leap.log is deleted");
                }
        }

       
    }
}
