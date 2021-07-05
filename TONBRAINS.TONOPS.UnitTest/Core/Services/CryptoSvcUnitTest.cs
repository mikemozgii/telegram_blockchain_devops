using NUnit.Framework;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class CryptoSvcUnitTest
    {
        private CryptoSvc CryptoSvc { get; set; }
        private const string ValidMnemonicPhrase = "abandon math mimic master filter design carbon crystal rookie group knife young";
        private string ValidPublicKey { get; set; }
        private string ValidSercretKey { get; set; }
        private string ValidTonSafePublicKey { get; set; }

        [SetUp]
        public void Setup()
        {
            CryptoSvc = new CryptoSvc();
            ValidPublicKey = "61c3c5b97a33c9c0a03af112fbb27e3f44d99e1f804853f9842bb7a6e5de3ff9";
            ValidSercretKey = "832410564fbe7b1301cf48dc83cbb8a3008d3cf29e05b7457086d4de041030ea";
            ValidTonSafePublicKey = "PuZhw8W5ejPJwKA68RL7sn4_RNmeH4BIU_mEK7em5d4_-cIx";
        }

        [Test]
        public void GetMnemonicPhrase()
        {
            var r = CryptoSvc.GetMnemonicPhrase();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(r));
        }


        [Test]
        [TestCase(ValidMnemonicPhrase)]
        public void GetKeyPair(string phrase)
        {
            var r = CryptoSvc.GetKeyPair(phrase);
            Assert.IsTrue(r.PublicKey == ValidPublicKey);
            Assert.IsTrue(r.SecretKey == ValidSercretKey);
            Assert.IsTrue(r.TonSafePublicKey == ValidTonSafePublicKey);
        }

        [Test]
        public void GetMnemonicKeyPair()
        {
            var phrase = CryptoSvc.GetMnemonicPhrase();
            var r = CryptoSvc.GetKeyPair(phrase);
            Assert.Pass();
        }
    }
}