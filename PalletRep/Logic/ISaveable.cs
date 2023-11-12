using PalletRep.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalletRep.Logic
{
    internal interface ISaveable
    {
        Task Save(List <Layout> layouts);
    }
}
