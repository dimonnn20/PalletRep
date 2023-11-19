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
        private string RemotePathSFTP = ConfigurationManager.AppSettings["RemotePathSFTP"];
        private readonly string RemotePath = ConfigurationManager.AppSettings["RemotePathForTest"];
        private readonly LeapLogParser LeapLogParser;
        private List<string> lines;

        public SFTPConnection(LeapLogParser leap)
        {
            LeapLogParser = leap;
        }

        public async Task CheckAndProceedFile()
        {
            if (ConfigurationManager.AppSettings["Test"].Equals("Yes"))
            {
                try
                {
                    if (File.Exists(RemotePath))
                    {
                        Logger.Logger.Log.Info("File leap.log detected");
                        lines = new List<string>();
                        using (var stream = new FileStream(RemotePath, FileMode.OpenOrCreate))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string line;
                                while ((line = await reader.ReadLineAsync()) != null)
                                {
                                    lines.Add(line);
                                }
                                Logger.Logger.Log.Info("Information from file leap.log readed successfuly");
                                await LeapLogParser.Proceed(lines);
                            }
                        }
                        File.Delete(RemotePath);
                        Logger.Logger.Log.Info("File leap.log is deleted");
                    }
                } catch (Exception ex)
                {
                    Logger.Logger.Log.Error("Exception during file folder connection, details below:\n" + ex.ToString());
                }

            }
            else
            {
                try
                {
                    using (SftpClient sftp = new SftpClient(Host, Username, Password))
                    {
                        sftp.Connect();
                        if (sftp.Exists(RemotePathSFTP))
                        {
                            lines = new List<string>();
                            using (SftpFileStream fileStream = sftp.OpenRead(RemotePathSFTP))
                            {
                                using (StreamReader reader = new StreamReader(fileStream))
                                {
                                    string line;
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        lines.Add(line);
                                    }
                                    Logger.Logger.Log.Info("Information from file leap.log readed successfuly");
                                    await LeapLogParser.Proceed(lines);
                                    sftp.Delete(RemotePathSFTP);
                                    Logger.Logger.Log.Info("File leap.log is deleted");
                                    sftp.Disconnect();
                                }
                            }
                        }
                    }
                } catch (Exception ex) 
                {
                    Logger.Logger.Log.Error("Exception during SFTP connection, details below:\n"+ex.ToString());
                }
                
            }
        }
    }
}
