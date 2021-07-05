using NUnit.Framework;
using System.IO;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class PowerShellSvcUnitTest
    {
        private PowerShellSvc PowerShellSvc { get; set; }


        [SetUp]
        public void Setup()
        {
            PowerShellSvc = new PowerShellSvc();

        }


        [Test]
        public void AddNewVM()
        {

            //PowerShellSvc.AddNewVM();

            //Assert.IsTrue(true);
        }

    }
}