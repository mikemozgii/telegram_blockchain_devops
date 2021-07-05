using NUnit.Framework;
using System.IO;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class TONClientSvcUnitTest
    {
        private TONClientSvc TONClientSvc { get; set; }


        [SetUp]
        public void Setup()
        {
            TONClientSvc = new TONClientSvc();

        }


        [Test]
        public void GetConfigTest()
        {

            var nodeIds = new string[] {
            "-MNPvvqpyCGGDRpvHPlF",
            };

           var rs = TONClientSvc.GetTime(nodeIds);

            Assert.IsTrue(true);
        }

    }
}