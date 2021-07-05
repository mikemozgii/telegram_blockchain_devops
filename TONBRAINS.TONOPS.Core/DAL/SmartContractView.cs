using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartContractView
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string TypeId { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string LibId { get; set; }
        public int CountAccounts { get; set; }
    }
}
