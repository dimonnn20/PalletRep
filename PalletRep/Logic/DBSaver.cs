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
        private readonly string TempFileName = Path.GetTempFileName();

        public async Task Save(List<Layout> layouts)
        {
            await SaveToQueue(layouts);
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    StringBuilder stringBuilder = new StringBuilder();
                    using (StreamReader reader = new StreamReader(TempFileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                string[] array = line.Split(';');
                                stringBuilder.AppendLine($"INSERT INTO Layout (dateOfEntry,sscc,date) VALUES ('{array[0]}','{array[1]}','{array[2]}');");
                            }
                        }
                    }
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
                finally { connection.Close(); }

            }
        }

        private async Task SaveToQueue(List<Layout> layouts)
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"));
            StringBuilder stringBuilder = new StringBuilder();
            Queue<string> QueueToWrite = new Queue<string>();
            foreach (Layout layout in layouts)
            {
                stringBuilder.Append(date).Append(";").Append(layout.Sscc).Append(";").Append(layout.Date.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"))).Append("\n");
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
                    Logger.Logger.Log.Debug($"Successfully added line(s) to temp file {Path.GetFileName(TempFileName)}");
                    QueueToWrite.Clear();

                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Debug($"Exception to add line(s) to temp file" + ex.ToString());
            }
        }
    }
}
