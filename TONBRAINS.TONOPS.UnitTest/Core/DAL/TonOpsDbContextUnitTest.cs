using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;

namespace TONBRAINS.TONOPS.UnitTest.Core.DAL
{
    public class TonOpsDbContextUnitTest
    {
        private DbContextOptions TonOpsDbContextOptions { get; set; }

        [SetUp]
        public void Setup()
        {
            var connectionString = "Host=192.168.1.34;Database=tonops;Port=5432;Username=postgres;Password=admin3232";

            var builder = new DbContextOptionsBuilder<TonOpsDbContext>().UseNpgsql(connectionString);
            TonOpsDbContextOptions = builder.Options;
        }

        [Test]
        public void Check()
        {
            using (var c = new TonOpsDbContext(TonOpsDbContextOptions))
            {
                var r = c.Tokens.FirstOrDefault(q=>q.Id == "0c40ddfc-8b48-4967-b4f0-55dafde3f48e");

                Assert.IsNotNull(r);
            }
        }
    }
}
