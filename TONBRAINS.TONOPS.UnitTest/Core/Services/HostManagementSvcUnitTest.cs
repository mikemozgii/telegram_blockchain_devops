using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Services;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class HostManagementSvcUnitTest
    {
        private HostManagementSvc HostManagementSvc { get; set; }


        [SetUp]
        public void Setup()
        {
            HostManagementSvc = new HostManagementSvc();

        }


        [Test]
        public void AddNodeTest()
        {

            var r = HostManagementSvc.AddNode("-MNY2oEVuA91-8x3Dx-c", "ubuntuTONCore20_04", 41, 2241);

            Assert.IsTrue(true);
        }

        [Test]
        public void DeleteNodeTest()
        {

            var r = HostManagementSvc.DeleteNode("-MNPvvqpyCGGDRpvHPlF");

            Assert.IsTrue(true);
        }



        [Test]
        public void TestPowerShellConnectionTest()
        {

            var r = HostManagementSvc.TestPowerShellConnection("-MNY2oEVuA91-8x3Dx-c");

            Assert.IsTrue(true);
        }

        
    }
}
