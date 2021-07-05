using NUnit.Framework;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Ffi
{
    public class TLTonCliUnitTest
    {
        private TLTonCli TLTonCli { get; set; }
        //private string ValidMnemonicPhrase { get; set; }
        //private string ValidPublicKey { get; set; }
        //private string ValidSercretKey { get; set; }

        [SetUp]
        public void Setup()
        {
            TLTonCli = new TLTonCli();
            //ValidMnemonicPhrase = "abandon math mimic master filter design carbon crystal rookie group knife young";
            //ValidPublicKey = "61c3c5b97a33c9c0a03af112fbb27e3f44d99e1f804853f9842bb7a6e5de3ff9";
            //ValidSercretKey = "832410564fbe7b1301cf48dc83cbb8a3008d3cf29e05b7457086d4de041030ea";
        }


        [Test]
        public void GenerateMnemonicPhraseTest()
        {
            //var r = TLTonCli.GenerateMnemonicPhrase();
            //Assert.IsTrue(!string.IsNullOrWhiteSpace(r));
        }


        



    }
}