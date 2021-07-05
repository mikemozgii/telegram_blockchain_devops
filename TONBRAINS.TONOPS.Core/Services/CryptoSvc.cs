using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class CryptoSvc
    {
        public TonWorkLib TonWorkLib { get; set;}

        public CryptoSvc()
        {
            TonWorkLib = new TonWorkLib();
        }

        public string GetMnemonicPhrase()
        {
            var fr = TonWorkLib.GenerateMnemonicPhrase();
            return fr;
        }

        public KeyPairMdl GetKeyPair(string phrase)
        {
            if (!TonWorkLib.VerifyMnemonicPhrase(phrase))      
                return null;

            
            var r1 = TonWorkLib.GenerateKeyPair(phrase);
            var r2 = TonWorkLib.ToTonSafeFormat(r1.Public);


            var m = new KeyPairMdl()
            {
                PublicKey = r1.Public,
                SecretKey = r1.secret,
                TonSafePublicKey = r2
            };

            return m;

        }

        public string GetTonSaFeFormat(string publicKey)
        {
            return TonWorkLib.ToTonSafeFormat(publicKey);

        }
    }
}
