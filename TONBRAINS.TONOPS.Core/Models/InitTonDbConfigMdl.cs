using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Models
{
    public class InitTonDbConfigMdl
    {

        public int out_port { get; set; }

        public InitTonDbConfigAddrMdl[] addrs { get; set; }
        public InitTonDbConfigEngineAdnlMdl[] adnl { get; set; }
        public InitTonDbConfigEngineDhtlMdl[] dht { get; set; }
        public string fullnode { get; set; }

        public string[] fullnodeslaves { get; set; }

        public string[] fullnodemasters { get; set; }
        public string[] liteservers { get; set; }

        public string[] control { get; set; }
    }
}
