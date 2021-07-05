using System;
using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountBalanceTransferLog
    {
        [Key]
        public string Id { get; set; }
        public string NetworkId { get; set; }
        public string SmartAccountFromNetworkId { get; set; }
        public long SmartAccountFromBalance { get; set; }
        public string SmartAccountToNetworkId { get; set; }
        public long SmartAccountToBalance { get; set; }
        public long TransferBalance { get; set; }
        public DateTime TransferTime { get; set; }
    }
}
