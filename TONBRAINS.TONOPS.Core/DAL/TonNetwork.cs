using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class TonNetwork
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ConfigurationId { get; set; }
        public bool IsDeleted { get; set; }
        public bool Active { get; set; }
        public DateTime? DateStarted { get; set; }

        public string ValidatorId { get; set; }
        public int? ValidatorInitAmount { get; set; }
        public string ContractId { get; set; }
        public string ElectorId { get; set; }
        public string ConfigId { get; set; }


        public string MainConfigSmartAccountId { get; set; }

        public string MainWalletSmartAccountId { get; set; }

        public ICollection<SmartAccountNetwork> SmartAccountNetworks { get; set; }
    }
}
