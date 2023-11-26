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
                while (_isRunning)
                {
                    
                    await Connection.CheckAndProceedFile();
                    await Task.Delay(Convert.ToInt32(ConfigurationManager.AppSettings["Timeout"]));
                }
        }

        public void Stop()
        {
            _isRunning = false;
        }

    }
}
