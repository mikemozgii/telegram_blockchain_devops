using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountKeyView
    {
        [Key]
        public string Id { get; set; }
        public string SmartAccountId { get; set; }
        public string SmartContractId { get; set; }
        public string SmartKeyId { get; set; }
        public string KeyName { get; set; }
        public string TypeId { get; set; }
        public string MnemonicPhrase { get; set; }
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public string TonSafePublicKey { get; set; }
    }
}
