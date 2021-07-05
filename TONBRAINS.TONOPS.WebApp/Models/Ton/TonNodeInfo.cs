using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.WebApp.Models.Ton
{
    public class TonNodeInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Disk { get; set; }
        public string Status { get; set; }
        public int SshPort { get; set; }
        public NodeTypes Type { get; set; }
        public string HostName { get; set; }

        public Dictionary<string, string> ActualMetric { get; set; }

        public string TypeName => Type.GetDescription();
    }
}
