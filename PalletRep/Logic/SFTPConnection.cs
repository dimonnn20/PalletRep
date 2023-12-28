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
        private readonly string _host;
        private readonly string _username = "***";
        private readonly string _password = "***";
        //private string _remotePath = "/userapp/software/draw/log/leap.log";
        private string _remotePath = ConfigurationManager.AppSettings["RemotePath"];

        private List<string> lines;

        public SFTPConnection()
        {
            try
            {
                _host = ConfigurationManager.AppSettings["IP"];
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Debug("Exception to pars IP from config ",ex);
            }
        }

        public async Task CheckAndProceedFile()
        {
            Logger.Logger.Log.Debug("CheckAndProceedFile started working");
            await Task.Run(() =>
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
                //});

                Logger.Logger.Log.Debug("Check for file");
                if (File.Exists(_remotePath))
                {
                    Logger.Logger.Log.Debug("File exists");
                    lines = new List<string>();
                    using (var stream = new FileStream(_remotePath, FileMode.OpenOrCreate))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                lines.Add(line);
                            }
                            Logger.Logger.Log.Debug("Read from file is successful");
                            LeapLogParser leapLogParser = new LeapLogParser();
                            leapLogParser.Proceed(lines);
                        }
                    }
                    File.Delete(_remotePath);
                    Logger.Logger.Log.Debug("File leap.log is deleted");
                }
            });
        }

       
    }
}
