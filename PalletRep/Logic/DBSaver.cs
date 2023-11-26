using CliWrap;
using log4net.Layout;
using PalletRep.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PalletRep.Logic
{
    internal class DBSaver
    {
        private readonly string QueueToDB = @"C:/folderToDel/writeToDB.txt";
        private static DBSaver _instance;
        private readonly string ConnectionString = ConfigurationManager.AppSettings["connectionString"];
        private DBSaver()
        {

        }

        public static DBSaver GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DBSaver();
                return _instance;
            }
            else
            {
                return _instance;
            }

        }

        public async Task Save(List<Layout> layouts)
        {
            // Save all lines to temp file
            await SaveToQueue(layouts);

        }

        private async Task SaveToQueue(List<Layout> layouts)
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
            Queue<string> QueueToWrite = new Queue<string>();
            foreach (Layout layout in layouts)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("('").Append(date).Append("','").Append(layout.Sscc).Append("','").Append(layout.Date.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"))).Append("')");
                QueueToWrite.Enqueue(stringBuilder.ToString());

            }
            try
            {
                if (QueueToWrite.Count > 0) 
                {
                    using (StreamWriter writer = new StreamWriter(QueueToDB, true))
                    {
                        foreach (string line in QueueToWrite)
                        {
                            await writer.WriteLineAsync(line);
                        }
                        Logger.Logger.Log.Info($"Successfully added {QueueToWrite.Count()} line(s) to queue file {Path.GetFileName(QueueToDB)}");
                        QueueToWrite.Clear();

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error($"Exception to add line(s) to queue file" + ex.ToString());
            }
        }

        public async Task StartSavingToDBAsync()
        {
            while (true)
            {
                int numberOfLinesInQueue;
                if (CheckForFile())
                {
                    List<string> lines = new List<string>();
                    if ((numberOfLinesInQueue = CheckLines()) <= 100)
                    {
                        try
                        {
                            using (StreamReader reader = new StreamReader(QueueToDB))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        lines.Add(line);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Logger.Log.Error("Exception to read from queue file", ex);
                        }
                        if (await WriteToDB(lines))
                        {
                            if (File.Exists(QueueToDB)) { File.Delete(QueueToDB); }
                        }
                    }
                    else
                    {
                        try
                        {
                            using (StreamReader reader = new StreamReader(QueueToDB))
                            {
                                string line;
                                for (int i = 0; i < 100; i++)
                                {
                                    line = reader.ReadLine();
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        lines.Add(line);
                                    }
                                }
                            }

                            if (await WriteToDB(lines))
                            {
                                string[] tempLines = File.ReadAllLines(QueueToDB);
                                string[] newLines = tempLines.Skip(100).ToArray();
                                File.WriteAllLines(QueueToDB, newLines);
                            }
                            else
                            {
                                Logger.Logger.Log.Error("Unsuccess to write to database when lines in temp file is more than 100");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Logger.Log.Error("Exception to read from queue file and add to database", ex);
                        }
                    }
                }
                //***********************************************************************
                await Task.Delay(Convert.ToInt32(ConfigurationManager.AppSettings["TimeoutToAccessBD"]));
            }
        }

        private int CheckLines()
        {
            int numberOfLines = 0;
            try
            {
                using (StreamReader reader = new StreamReader(QueueToDB))
                {
                    string str;
                    while ((str = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(str))
                            numberOfLines++;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Exception to read queue file ", ex);
            }

            return numberOfLines;
        }

        private bool CheckForFile()
        {
            bool fileExists = false;
            if (File.Exists(QueueToDB))
            {
                if (new FileInfo(QueueToDB).Length != 0)
                {
                    fileExists = true;

                }
            }
            return fileExists;
        }

        private async Task<bool> WriteToDB(List<string> lines)
        {
            bool success = false;
            // formatting request for insert
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"INSERT INTO Layout (dateOfEntry,sscc,date) VALUES ");
            for (int i = 0; i < lines.Count - 1; i++)
            {
                stringBuilder.Append(lines[i]);
                stringBuilder.Append(",");
            }
            stringBuilder.Append(lines[lines.Count - 1]);
            stringBuilder.Append(";");
            // inserting to DB
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = stringBuilder.ToString();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        Logger.Logger.Log.Info($"Successfully added {command.ExecuteNonQuery()} line(s) to database");
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Logger.Log.Error("Exception to access database ", ex);
                }
            }
            return success;
        }
    }
}
