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
        private SFTPConnection _connection;
        

        public MyServiceLogic()
        {
            _connection = new SFTPConnection();
        }

        public async Task StartAsync()
        {
            try
            {
                while (_isRunning)
                {
                    await _connection.CheckAndProceedFile();
                    await Task.Delay(Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]));
                }
            }

            catch (Exception ex)
            {

            }


        }

        public void Stop()
        {
            _isRunning = false;
        }

    }
}
