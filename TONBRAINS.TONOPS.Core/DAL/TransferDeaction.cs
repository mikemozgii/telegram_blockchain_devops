using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class TransferDeaction
    {
        [Key]
        public string Id { get; set; }
        public string InitSmartAccountNetworkId { get; set; }
        public string CompletedSmartAccountNetworkId { get; set; }
        public long DateCreatedAt { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AuthToken { get; set; }
        public long Amount { get; set; }
        public long? DateCompletedAt { get; set; }
        public long DateUpdateAt { get; set; }
        public TransferDeactionStatuses Status { get; set; }

        [NotMapped] public long Date { get; set; }
        [NotMapped] public bool IsPayment { get; set; }
        [NotMapped] public bool IsRecipient { get; set; }
    }
}
