using PalletRep.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Logic
{
    internal class FileSaver
    {
        private static FileSaver _instance;

        private FileSaver()
        {
        }
        public static FileSaver GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FileSaver();
                return _instance;
            }
            else { return _instance; }
        }

        public async Task Save(List<Layout> layouts)
        {
            string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt";
            string pathToSave = ConfigurationManager.AppSettings["PathToSaveReport"] + fileName;
            using (FileStream stream = new FileStream(pathToSave, FileMode.Append))
            {
                foreach (Layout layout in layouts)
                {
                    string layoutString = layout.ToString();
                    await stream.WriteAsync(Encoding.Default.GetBytes(layoutString), 0, layoutString.Length);
                }

            }
            Logger.Logger.Log.Info($"Data is successfully written to the file {fileName}");
        }
    }
}
