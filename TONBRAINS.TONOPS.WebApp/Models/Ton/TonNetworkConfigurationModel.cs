using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;

namespace TONBRAINS.TONOPS.WebApp.Models.Ton
{
    public class TonNetworkConfigurationModel
    {
        public TonNetwork Network { get; set; }
        public IEnumerable<string> NodesIds { get; set; }
    }
}
