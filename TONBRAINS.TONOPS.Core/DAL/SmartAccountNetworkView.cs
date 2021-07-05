using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountNetworkView
    {
        [Key]
        public string Id { get; set; }
        public string SmartAccountId { get; set; }
        public long Balance { get; set; }
        public string NetworkId { get; set; }
        public string Network { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public bool IsDeployed { get; set; }
    }
}
