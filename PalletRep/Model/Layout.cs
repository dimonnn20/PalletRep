using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Model
{
    [Serializable]
    internal class Layout
    {
        public string Sscc { get; set; }
        public string Date { get; set; }

        public Layout(string sscc, string date)
        {
            Sscc = sscc;
            Date = date;
        }

        public override string ToString()
        {
            return  $"[SSCC = {Sscc} ,date = {Date}]\n"; ;
        }
    }
}
