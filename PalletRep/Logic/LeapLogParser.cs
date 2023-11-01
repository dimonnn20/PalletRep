using PalletRep.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Logic
{
    internal class LeapLogParser
    {
        private ISaveable _saveable;
        private readonly string _mode;

        public LeapLogParser()
        {
            try 
            { 
            _mode = ConfigurationManager.AppSettings["Mode"];
            } catch (Exception ex)
            {
                Logger.Logger.Log.Debug("Exception to pars Mode from congif ",ex);
            }
            
        }

        public void Proceed(List<string> lines)
        {
            Logger.Logger.Log.Debug("Method Proceed lines started");
            List<Layout> layouts = deserialize(lines);
            if (_mode.Equals("TXT"))
            {
                _saveable = new FileSaver();
                _saveable.Save(layouts);
            }
            else if (_mode.Equals("DB"))
            {

                _saveable = new DBSaver();
                Logger.Logger.Log.Debug("Method Proceed DB Saver created");
                _saveable.Save(layouts);
                Logger.Logger.Log.Debug("Method Save from DB Saver implemented");
            }
            else if (_mode.Equals("TXT and DB"))
            {
                Logger.Logger.Log.Debug("Method Procced start save to file and DB");
                _saveable = new FileSaver();
                _saveable.Save(layouts);
                _saveable = new DBSaver();
                _saveable.Save(layouts);
                Logger.Logger.Log.Debug("Method Procced ended save to file and DB");
            }
            else
            {
                Logger.Logger.Log.Debug("Error in If-else in method Proceed");
                throw new Exception();
            }
        }

        private List<Layout> deserialize(List<string> lines)
        {
            List<Layout> layouts = new List<Layout>();

            foreach (string line in lines)
            {
                string[] array = line.Split(';');
                StringBuilder sb = new StringBuilder();
                sb.Append(array[0].Trim());
                sb.Append(' ');
                sb.Append(array[1].Trim());
                Layout layout = new Layout(array[2].Trim(), sb.ToString());
                layouts.Add(layout);
            }
            return layouts;
        }
    }
}
