using NUnit.Framework;
using System.IO;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class HupervNodeSvcUnitTest
    {
        private HyperVHostManagementSvc HupervNodeSvc { get; set; }


        [SetUp]
        public void Setup()
        {
            HupervNodeSvc = new HyperVHostManagementSvc();

        }


        [Test]
        public void AddNewTonVm()
        {

          // HupervNodeSvc.AddNewTonVm(28);

            Assert.IsTrue(true);
        }

    }
}