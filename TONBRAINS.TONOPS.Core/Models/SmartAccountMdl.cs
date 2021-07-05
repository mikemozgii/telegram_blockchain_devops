using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Models
{
    public class SmartAccountMdl
    {
        public string Id { get; set; }

        public string PublicKey { get; set; }

        public string SecretKey { get; set; }

        
        public string Address { get; set; }

        public string Phrase { get; set; }

        public string TonSafeKey { get; set; }

        public long Balance { get; set; }

        public bool IsDeployed { get; set; }
        public string QunatchainName { get; set; }
       
        public string QunatchainId { get; set; }

        public string QunatchainStateId { get; set; }

        public string Status { get; set; }

        public string SmartContractName { get; set; }

        public string SmartContractVersion { get; set; }
    }
}
