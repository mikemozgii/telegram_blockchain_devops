using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class IdGeneratorTest
    {

        [Test]
        public void Test()
        {
            var data = new HashSet<string>();

            var testCount = 100000;

            for (int i = 0; i < testCount; i++)
            {
                var id = IdGenerator.Generate();
                if(!data.Contains(id)) data.Add(id);
            }

            Assert.IsTrue(data.Count == testCount);
        }
    }
}
