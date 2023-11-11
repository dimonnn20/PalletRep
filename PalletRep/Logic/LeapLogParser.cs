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
        private readonly string Mode = ConfigurationManager.AppSettings["Mode"];
        private const string Pattern = @"(\d{2}/\d{2}/\d{4});(\d{2}:\d{2}:\d{2});(00\d{18}$)";

        public LeapLogParser()
        {   
        }

        public void Proceed(List<string> lines)
        {
            List<Layout> layouts = Deserialize(lines);
            if (Mode.Contains("DB") || Mode.Contains("TXT"))
            {
                if (Mode.Contains("TXT"))
                {
                    _saveable = new FileSaver();
                    _saveable.Save(layouts);
                }
                if (Mode.Contains("DB"))
                {

                    _saveable = new DBSaver();
                    _saveable.Save(layouts);
                }

            } else {
                Logger.Logger.Log.Error("Exception in App.config, line [Mode]");
                throw new Exception("Exception in App.config, line [Mode]");
            }
        }

        private List<Layout> Deserialize(List<string> lines)
        {
            bool isFileCorrect = false;
            List<Layout> layouts = new List<Layout>();

            foreach (string line in lines)
            {
                if (CheckLine(line))
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
                throw new Exception("The file leap.log contains incorrect data, please check the file and restart service"); 
            }
            return layouts;
        }

        private bool CheckLine(string line)
        {
            bool isCorrect = false;
            Match match = Regex.Match(line, Pattern);
            if (match.Success) 
            { 
                isCorrect = true;
            }
                return isCorrect;
        }
    }
}
