using PalletRep.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep
{
    public partial class Service1 : ServiceBase
    {

        private MyServiceLogic _serviceLogic;
        private ServiceController sc;
        private DBSaver _dbSaver;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _dbSaver = DBSaver.GetInstance();
            Task.Run(async () =>
            {
                try
                {
                    await _dbSaver.StartSavingToDBAsync();
                }
                catch (Exception ex)
                {
                    Logger.Logger.Log.Error("Exception to save to database ", ex);
                }
            });
            
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(configFilePath));
            LeapLogParser leapParser = new LeapLogParser();
            SFTPConnection connection = new SFTPConnection(leapParser);
            _serviceLogic = new MyServiceLogic(connection);
            Logger.Logger.Log.Info("Service started");

            Task.Run(async () =>
            {
                try
                {
                    await _serviceLogic.StartAsync();
                }
                catch (Exception ex)
                {
                    Logger.Logger.Log.Error("Exception in _serviceLogic ", ex);
                    Stop();
                }
            });
        }


        protected override void OnStop()
        {
            Logger.Logger.Log.Info("The service is stopped");

        }
    }
}
