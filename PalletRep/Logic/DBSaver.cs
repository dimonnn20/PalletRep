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
    internal class DBSaver : ISaveable
    {
        private readonly string TempFileName = @"C:\Users\makad\AppData\Local\Temp\myTmp.txt";
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
            await TryToSaveToDB();

        }

        private async Task SaveToQueue(List<Layout> layouts)
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
            StringBuilder stringBuilder = new StringBuilder();
            Queue<string> QueueToWrite = new Queue<string>();
            foreach (Layout layout in layouts)
            {
                stringBuilder.Append("('").Append(date).Append("','").Append(layout.Sscc).Append("','").Append(layout.Date.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"))).Append("')\n");
                QueueToWrite.Enqueue(stringBuilder.ToString());

            }
            try
            {
                using (StreamWriter writer = new StreamWriter(TempFileName))
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

        private async Task TryToSaveToDB()
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
                    Logger.Logger.Log.Debug($"Successgully readed {lines.Count()} from temp file" );
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
            // insertin to DB

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
}
