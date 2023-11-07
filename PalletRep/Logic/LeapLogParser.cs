using PalletRep.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PalletRep.Logic
{
    internal class LeapLogParser
    {
        private ISaveable _saveable;
        private readonly string _mode;
        private const string patter = @"(\d{2}/\d{2}/\d{4});(\d{2}:\d{2}:\d{2});(00\d{18}$)";

        public LeapLogParser()
        {
            try 
            { 
            _mode = ConfigurationManager.AppSettings["Mode"];
            } catch (Exception ex)
            {
                Logger.Logger.Log.Error("Exception to parse Mode from App.config ",ex);
            }
            
        }

        public void Proceed(List<string> lines)
        {
            List<Layout> layouts = deserialize(lines);
            if (_mode.Equals("TXT"))
            {
                _saveable = new FileSaver();
                _saveable.Save(layouts);
            }
            else if (_mode.Equals("DB"))
            {

                _saveable = new DBSaver();
                _saveable.Save(layouts);
            }
            else if (_mode.Equals("TXT and DB"))
            {
                _saveable = new FileSaver();
                _saveable.Save(layouts);
                _saveable = new DBSaver();
                _saveable.Save(layouts);
            }
            else
            {
                Logger.Logger.Log.Error("Exception in App.config, line [Mode]");
                throw new Exception();
            }
        }

        private List<Layout> deserialize(List<string> lines)
        {
            bool isFileCorrect = false;
            List<Layout> layouts = new List<Layout>();

            foreach (string line in lines)
            {
                if (checkLine(line))
                {
                    string[] array = line.Split(';');
                    StringBuilder dateTime = new StringBuilder();
                    dateTime.Append(array[0].Trim());
                    dateTime.Append(' ');
                    dateTime.Append(array[1].Trim());
                    string sscc = array[2].Trim();
                    Layout layout = new Layout(sscc, dateTime.ToString());
                    layouts.Add(layout);
                    isFileCorrect = true;
                }
                else
                {
                    Logger.Logger.Log.Error($"The line {line} in file leap.log is incorrect and was not proceeded");
                }
            }
            if (!isFileCorrect) 
            {
                Logger.Logger.Log.Error($"The file leap.log contains incorrect data, please check the file and restart service");
                throw new Exception(); 
               
                
            }
            return layouts;
        }

        private bool checkLine (string line)
        {
            bool isCorrect = false;
            Match match = Regex.Match(line, patter);
            if (match.Success) 
            { 
                isCorrect = true;
            }
                return isCorrect;
        }
    }
}
