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
        private readonly string Host = ConfigurationManager.AppSettings["IP"];
        private readonly string Username = ConfigurationManager.AppSettings["Username"];
        private readonly string Password = ConfigurationManager.AppSettings["Password"];
        private readonly string RemotePath = ConfigurationManager.AppSettings["RemotePath"];
        private readonly LeapLogParser LeapLogParser;

        private List<string> lines;

        public SFTPConnection(LeapLogParser leap)
        {
            LeapLogParser = leap;
        }

        public async Task CheckAndProceedFile()
        {
            //await Task.Run(() =>
            //{
            //    try
            //    {
            //        using (SftpClient sftp = new SftpClient(Host, Username, Password))
            //        {
            //            sftp.Connect();
            //            if (sftp.Exists(RemotePath))
            //            {
            //                lines = new List<string>();
            //                using (SftpFileStream fileStream = sftp.OpenRead(RemotePath))
            //                {
            //                    using (StreamReader reader = new StreamReader(fileStream))
            //                    {
            //                        string line;
            //                        while ((line = reader.ReadLine()) != null)
            //                        {
            //                            lines.Add(line);
            //                        }
            //                        LeapLogParser.Proceed(lines);
            //                        sftp.Delete(RemotePath);
            //                        sftp.Disconnect();
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.Logger.Log.Error("Exception during reading from file " + ex.ToString());
            //    }
            //});


            await Task.Run(() =>
            {
                if (File.Exists(RemotePath))
                {
                    Logger.Logger.Log.Info("File leap.log exists");
                    lines = new List<string>();
                    try
                    {
                        using (var stream = new FileStream(RemotePath, FileMode.OpenOrCreate))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    lines.Add(line);
                                }
                                Logger.Logger.Log.Info("Infromation from file leap.log is readed successfuly");
                                try
                                {
                                    LeapLogParser.Proceed(lines);
                                    File.Delete(RemotePath);
                                    Logger.Logger.Log.Info("File leap.log is deleted");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Logger.Log.Error("Exception during proceeding data from file " + ex.ToString());
                                    throw new Exception();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Logger.Log.Error("Exception during reading from file " + ex.ToString());
                    }
                }
            });

        }


    }
}
