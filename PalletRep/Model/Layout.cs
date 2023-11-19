using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Model
{
    [Serializable]
    internal class Layout
    {
        public string Sscc { get; set; }
        public DateTime Date { get; set; }

        public Layout(string sscc, DateTime date)
        {
            Sscc = sscc;
            Date = date;
        }

        public override string ToString()
        {
            return  $"[SSCC = {Sscc} ,date = {Date.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"))}]\n"; ;
        }
    }
}
