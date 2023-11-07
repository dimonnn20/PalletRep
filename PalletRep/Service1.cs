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

        private MyServiceLogic ServiceLogic;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.Configure(new FileInfo(configFilePath));
            Logger.Logger.Log.Info("Service started");
            LeapLogParser leapParser = new LeapLogParser();
            SFTPConnection connection = new SFTPConnection(leapParser);
            ServiceLogic = new MyServiceLogic(connection);
            Task.Run(() => ServiceLogic.StartAsync());

           
        }

        protected override void OnStop()
        {
            Logger.Logger.Log.Info("Service stopped");
            ServiceLogic.Stop();
        }
    }
}
