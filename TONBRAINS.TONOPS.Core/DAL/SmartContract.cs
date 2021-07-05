using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartContract
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string TypeId { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string TvcFileId { get; set; }
        public string AbiFileId { get; set; }
        public string SolFileId { get; set; }
        public bool IsDeleted { get; set; }
        public string AbiJson { get; set; }
        public string SolJson { get; set; }
        public string LibId { get; set; }
    }
}
