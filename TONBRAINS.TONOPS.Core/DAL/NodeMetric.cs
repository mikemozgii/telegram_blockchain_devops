using System;
using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class NodeMetric
    {
        [Key]
        public string Id { get; set; }
        public string NodeId { get; set; }
        public string Config { get; set; }
        public int Diskavailible { get; set; }
        public int Disktotal { get; set; }
        public double Diskpercent { get; set; }
        public int Timediff { get; set; }
        public int Tonlogsize { get; set; }
        public bool ValidatorActive { get; set; }
        public DateTime Date { get; set; }

        public int RamTotal { get; set; }
        public int RamAvailable { get; set; }
        public int RamFree { get; set; }
        public int RamUsed { get; set; }
        public int? Processes { get; set; }
    }
}
