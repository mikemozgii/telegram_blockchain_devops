using System;
using System.ComponentModel.DataAnnotations.Schema;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class v_NodeMetric
    {
        public string Id { get; set; }
        public string HostId { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int SshPort { get; set; }
        public string Status { get; set; }
        public string HostName { get; set; }
        public string Config { get; set; }
        public int Diskavailible { get; set; }
        public int Disktotal { get; set; }
        public double Diskpercent { get; set; }
        public int Timediff { get; set; }
        public int Tonlogsize { get; set; }
        public bool? ValidatorActive { get; set; }

        public int RamTotal { get; set; }
        public int RamAvailable { get; set; }
        public int RamFree { get; set; }
        public int RamUsed { get; set; }
        public int Processes { get; set; }

        public DateTime? Date { get; set; }

        public NodeTypes Type { get; set; }

        [NotMapped]
        public string TypeName => Type.GetDescription();
    }
}
