using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountBalanceTransferLogView
    {
        [Key]
        public string Id { get; set; }
        public DateTime TransferTime { get; set; }
        public long TransferBalance { get; set; }
        public string SmartAccountFromNetworkId { get; set; }
        public string FromName { get; set; }
        public string SmartAccountToNetworkId { get; set; }
        public string ToName { get; set; }
        public long SmartAccountFromBalance { get; set; }
        public long SmartAccountToBalance { get; set; }

        [NotMapped] public string Type { get; set; }
        [NotMapped] public long Total { get; set; }
    }
}
