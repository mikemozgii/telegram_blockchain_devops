namespace TONBRAINS.TONOPS.Core.Models
{
    public class GenerateKeysMdl
    {
        public string Name { get; set; }
        public string TypeId { get; set; }
        public int Wc { get; set; }
        public long KeysCount { get; set; }
        public string TvcFileId { get; set; }
        public string AbiFileId { get; set; }
        public string SolFileId { get; set; }
        public bool LoadSmartContract { get; set; }
        public string SmartContractId { get; set; }
    }
}
