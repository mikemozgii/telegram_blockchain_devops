using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Models
{
    public class InitTonDbConfigAddrMdl
    {
        public int ip { get; set; }
        public int port { get; set; }
        public int[] categories { get; set; }
        public int[] priority_categories { get; set; }
    }
}
