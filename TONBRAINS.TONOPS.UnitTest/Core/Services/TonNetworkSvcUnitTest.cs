using NUnit.Framework;
using System.IO;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class TonNetworkSvcUnitTest
    {
        private TonNetworkSvc TonNetworkSvc { get; set; }


        [SetUp]
        public void Setup()
        {
            TonNetworkSvc = new TonNetworkSvc();

        }


        [Test]
        public void GetActualTonConfigTest()
        {

            //var nodeIds = new string[] {
            //"-MNPvvqpyCGGDRpvHPlF",
            //};

           var rs = TonNetworkSvc.GetActualTonConfig("-MNyZvn-qE9-GqubuwJZ");

            Assert.IsTrue(true);
        }

    }
}