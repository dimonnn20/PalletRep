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
        private FileSaver saveToFile;
        private DBSaver saveToDB;
        private readonly string Mode = ConfigurationManager.AppSettings["Mode"];
        private const string Pattern = @"(\d{2}/\d{2}/\d{4});(\d{2}:\d{2}:\d{2});(00\d{18}$)";

        public LeapLogParser()
        {   
        }

        public async Task Proceed(List<string> lines)
        {
            List<Layout> layouts = Deserialize(lines);
            if (Mode.Contains("DB") || Mode.Contains("TXT"))
            {
                if (Mode.Contains("TXT"))
                {
                    saveToFile = FileSaver.GetInstance();
                    await saveToFile.Save(layouts);
                }
                if (Mode.Contains("DB"))
                {

                    saveToDB = DBSaver.GetInstance();
                    await saveToDB.Save(layouts);
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
                    StringBuilder dateTimeString = new StringBuilder();
                    dateTimeString.Append(array[0].Trim());
                    dateTimeString.Append(' ');
                    dateTimeString.Append(array[1].Trim());
                    DateTime dateTime;
                    if (!DateTime.TryParseExact(dateTimeString.ToString(), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime)) {
                        Logger.Logger.Log.Error($"The date {dateTimeString.ToString()} from line {line} file leap.log was not parced correctly");
                    }
                    string sscc = array[2].Trim();
                    Layout layout = new Layout(sscc, dateTime);
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
