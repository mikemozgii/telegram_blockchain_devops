using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountNetwork
    {
        [Key]
        public string Id { get; set; }
        public string SmartAccountId { get; set; }
        public string NetworkId { get; set; }
        public string StatusId { get; set; }
        public string Status { get; set; }
        public long Balance { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDeployed { get; set; }



        [ForeignKey(nameof(NetworkId))]
        public virtual TonNetwork TonNetwork { get; set; }

        [ForeignKey(nameof(SmartAccountId))]
        public virtual SmartAccount SmartAccount { get; set; }
    }
}
