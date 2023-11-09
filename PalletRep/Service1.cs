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
        public Service1()
        {
            InitializeComponent();
        }

        protected override async void OnStart(string[] args)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(configFilePath));
            LeapLogParser leapParser = new LeapLogParser();
            SFTPConnection connection = new SFTPConnection(leapParser);
            _serviceLogic = new MyServiceLogic(connection);
            Logger.Logger.Log.Info("Service started");
            try
            {
                await _serviceLogic.StartAsync();
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("Exception in _serviceLogic and try to stop the service ", ex);
                OnStop();
                ServiceController serviceController = new ServiceController("Service1");
                serviceController.Stop();
            }


            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        await _serviceLogic.StartAsync();
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.Logger.Log.Error("Exception in _serviceLogic and try to stop the service ", ex);
            //        OnStop();
            //        ServiceController serviceController = new ServiceController("Service1");
            //        serviceController.Stop();
            //    }
            //});
        }


        protected override void OnStop()
        {
            _serviceLogic.Stop();
            Logger.Logger.Log.Info("Service stopped");

        }
    }
}
