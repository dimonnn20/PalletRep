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
        public void Save(List<Layout> layouts)
        {
            Logger.Logger.Log.Debug("Method Save from DB Saver started working");
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Logger.Logger.Log.Debug("Trying to open connection to database");
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Logger.Logger.Log.Debug("Exception to open connection ", ex);
                }

                Logger.Logger.Log.Debug("Connection to database is opened");
                //*****************************************
                StringBuilder stringBuilder = new StringBuilder();
                string date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                foreach (Layout layout in layouts)
                {
                    stringBuilder.AppendLine($"INSERT INTO Layout (dateOfEnter,sscc,date) VALUES ('{date}','{layout.Sscc}','{layout.Date}');");
                }
                string query = stringBuilder.ToString();
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    Logger.Logger.Log.Debug($"Successfully added {command.ExecuteNonQuery()} line(s)");
                }
                //*****************************************
                connection.Close();
                Logger.Logger.Log.Debug("Connection to database is closed");
            }
        }
    }
}
