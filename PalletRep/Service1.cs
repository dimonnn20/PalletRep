using PalletRep.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            ServiceLogic = new MyServiceLogic();
            Task.Run(() => ServiceLogic.StartAsync());
           
        }

        protected override void OnStop()
        {
            ServiceLogic.Stop();
        }
    }
}
