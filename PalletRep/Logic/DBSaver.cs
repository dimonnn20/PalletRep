using PalletRep.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PalletRep.Logic
{
    internal class DBSaver : ISaveable
    {
        public async Task Save(List<Layout> layouts)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception ex)
                {
                    Logger.Logger.Log.Error("Exception to open connection to database ", ex);
                }
                StringBuilder stringBuilder = new StringBuilder();
                string date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                foreach (Layout layout in layouts)
                {
                    stringBuilder.AppendLine($"INSERT INTO Layout (dateOfEnter,sscc,date) VALUES ('{date}','{layout.Sscc}','{layout.Date}');");
                }
                string query = stringBuilder.ToString();
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    Logger.Logger.Log.Info($"Successfully added {command.ExecuteNonQuery()} line(s) to database");
                }
                connection.Close();
            }
        }
    }
}
