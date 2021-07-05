using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountKey
    {
        [Key]
        public string Id { get; set; }
        public string SmartAccountId { get; set; }
        public string SmartContractId { get; set; }
        public string SmartKeyId { get; set; }
        public bool IsDeleted { get; set; }


        [ForeignKey(nameof(SmartContractId))]
        public virtual SmartContract SmartContract  { get; set; }

        [ForeignKey(nameof(SmartKeyId))]
        public virtual SmartKey SmartKey { get; set; }

        [ForeignKey(nameof(SmartAccountId))]
        public virtual SmartAccount SmartAccount { get; set; }
    }
}
