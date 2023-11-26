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
        private readonly string TempFileName = @"C:/folderToDel/myTmp.txt";
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
            string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"));
            Queue<string> QueueToWrite = new Queue<string>();
            foreach (Layout layout in layouts)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("('").Append(date).Append("','").Append(layout.Sscc).Append("','").Append(layout.Date.ToString("MM/dd/yyyy HH:mm:ss", new CultureInfo("en-US"))).Append("')");
                QueueToWrite.Enqueue(stringBuilder.ToString());

            }
            try
            {
                using (StreamWriter writer = new StreamWriter(TempFileName, true))
                {
                    foreach (string line in QueueToWrite)
                    {
                        await writer.WriteLineAsync(line);
                    }
                    Logger.Logger.Log.Debug($"Successfully added {QueueToWrite.Count()} line(s) to temp file {Path.GetFileName(TempFileName)}");
                    QueueToWrite.Clear();

                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Debug($"Exception to add line(s) to temp file" + ex.ToString());
            }
        }

        public async Task TryToSaveToDB()
        {
            while (true)
            {
                if (File.Exists(TempFileName) )
                {
                    if (new FileInfo(TempFileName).Length != 0)
                    {
                        List<string> lines = new List<string>();
                        try
                        {
                            using (StreamReader reader = new StreamReader(TempFileName))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        lines.Add(line);
                                    }
                                }
                                Logger.Logger.Log.Debug($"Successfully readed {lines.Count()} from temp file");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Logger.Log.Error("Exception to read from temp file", ex);
                        }
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
                                }
                                if (File.Exists(TempFileName)) { File.Delete(TempFileName); }
                            }
                            catch (Exception ex)
                            {
                                Logger.Logger.Log.Error("Exception to open connection to database ", ex);
                            }
                        }
                    }
                }
                await Task.Delay(30000);
            }
        }
    }
}
