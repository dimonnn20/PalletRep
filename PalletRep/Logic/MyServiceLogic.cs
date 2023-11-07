using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Logic
{
    internal class MyServiceLogic
    {
        private bool _isRunning = true;
        private readonly SFTPConnection Connection;
        

        public MyServiceLogic(SFTPConnection connection)
        {
            Connection = connection;
        }

        public async Task StartAsync()
        {
            try
            {
                while (_isRunning)
                {
                    Connection.CheckAndProceedFile();
                    await Task.Delay(Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]));
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Exception ",ex);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

    }
}
