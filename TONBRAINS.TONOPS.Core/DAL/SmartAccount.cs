using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccount
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Wc { get; set; }
        public string TypeId { get; set; }
        public string Description { get; set; }
        public string SmartContractId { get; set; }
        public string NodeId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public string TonNetworkId { get; set; }
        //error, because inside the link to SmartAccount class
        public ICollection<SmartAccountNetwork> SmartAccountNetworks { get; set; }
        public ICollection<SmartAccountKey> SmartAccountKeys { get; set; }

        [ForeignKey( nameof(SmartContractId))]
        public virtual SmartContract SmartContract { get; set; }

        [NotMapped] public List<string> SmartKeyIds { get; set; }
        [NotMapped] public List<string> NetworkIds { get; set; }
    }
}
