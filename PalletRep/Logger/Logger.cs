using log4net.Config;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Logger
{
    public static class Logger
    {
        public static ILog Log { get; } = LogManager.GetLogger(typeof(Logger));
        static Logger()
        {
            //log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true);
            //log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("C:/Users/makad/source/repos/PalletRep/PalletRep/log4net.config"));
            // XmlConfigurator.Configure(new System.IO.FileInfo("C:/Users/makad/source/repos/PalletRep/PalletRep/log4net.config"));
        }

    }
}
