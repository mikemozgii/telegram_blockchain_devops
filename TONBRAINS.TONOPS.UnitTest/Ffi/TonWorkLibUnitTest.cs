using NUnit.Framework;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Ffi
{
    public class TonWorkLibUnitTest
    {
        private TonWorkLib TonWorkLib { get; set; }
        private string ValidMnemonicPhrase { get; set; }
        private string ValidPublicKey { get; set; }
        private string ValidSercretKey { get; set; }

        [SetUp]
        public void Setup()
        {
            TonWorkLib = new TonWorkLib();
            ValidMnemonicPhrase = "abandon math mimic master filter design carbon crystal rookie group knife young";
            ValidPublicKey = "61c3c5b97a33c9c0a03af112fbb27e3f44d99e1f804853f9842bb7a6e5de3ff9";
            ValidSercretKey = "832410564fbe7b1301cf48dc83cbb8a3008d3cf29e05b7457086d4de041030ea";
        }



        //[Test]
        //public void GenerateAccountAddressTest()
        //{
        //    var r = TonWorkLib.GenerateAccountAddress();
        //    Assert.IsTrue(!string.IsNullOrWhiteSpace(r.phrase));
        //}


        [Test]
        public void GenerateMnemonicPhraseTest()
        {
            var r = TonWorkLib.GenerateMnemonicPhrase();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(r));
        }


        [Test]
        public void VerifyMnemonicPhraseTest()
        {
            var r = TonWorkLib.VerifyMnemonicPhrase(ValidMnemonicPhrase);
            Assert.IsTrue(r);
        }

        [Test]
        public void GenerateKeyPairTest()
        {
            var r = TonWorkLib.GenerateKeyPair(ValidMnemonicPhrase);
            Assert.IsTrue(r.Public == "61c3c5b97a33c9c0a03af112fbb27e3f44d99e1f804853f9842bb7a6e5de3ff9");
            Assert.IsTrue(r.secret == "832410564fbe7b1301cf48dc83cbb8a3008d3cf29e05b7457086d4de041030ea");
        }

        [Test]
        public void ToTonSafeFormatTest()
        {
            var r = TonWorkLib.ToTonSafeFormat(ValidPublicKey);
            Assert.IsTrue(r == "PuZhw8W5ejPJwKA68RL7sn4_RNmeH4BIU_mEK7em5d4_-cIx");
        }






    }
}