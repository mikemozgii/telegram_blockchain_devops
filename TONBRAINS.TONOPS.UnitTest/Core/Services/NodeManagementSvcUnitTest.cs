using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Services;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class NodeManagementSvcUnitTest
    {
        public NodeManagementSvc NodeManagementSvc { get; set;}

        [SetUp]
        public void Setup()
        {
            NodeManagementSvc = new NodeManagementSvc();

        }


        //[Test]
        //public void AddNewTonNodetoUbuntuServerByHypervTest()
        //{

        //    NodeManagementSvc.AddNewTonNodeto_UbuntuServer_ByHypervBYCount(31, 35);
        //    Assert.IsTrue(true);
        //}
    }
}
