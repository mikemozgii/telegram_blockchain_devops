using System.Collections.Generic;

namespace TONBRAINS.TONOPS.Core.Models
{
    public class TransferBalanceMdl
    {
        public string Id { get; set; }
        public Dictionary<string, long> Balances { get; set; }
        public string[] NetworkIds { get; set; }
        public string Mode { get; set; }
        public bool Bounce { get; set; }
    }
}
