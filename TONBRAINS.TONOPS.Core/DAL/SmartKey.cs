using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartKey
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string TypeId { get; set; }
        public string Description { get; set; }
        public string MnemonicPhrase { get; set; }
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public string TonSafePublicKey { get; set; }
        public bool IsDeleted { get; set; }

    }
}
