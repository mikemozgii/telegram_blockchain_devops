using NUnit.Framework;
using System.IO;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class TONValidatorEngineConsoleSvcUnitTest
    {
        private TONValidatorEngineConsoleSvc TONValidatorEngineSvc { get; set; }


        [SetUp]
        public void Setup()
        {
            TONValidatorEngineSvc = new TONValidatorEngineConsoleSvc();

        }


        [Test]
        public void GetStatsTest()
        {

            var nodeIds = new string[] {
            "-MNPvvqpyCGGDRpvHPlF",
            };

           var rs = TONValidatorEngineSvc.GetStats(nodeIds);

            Assert.IsTrue(true);
        }

        [Test]
        public void GetMasterBlockTimeDifferenceTest()
        {

            //var nodeIds = new string[] {
            //       "-MNPvvqpyCGGDRpvHPlF",
            //"-MNPwR3VPiQhm91P2LwP",
            //"-MNPwvjIBSUQIUbNvdIX",
            //"-MNPxQq8FSKUMb8pyiEj",
            //"-MNPxuYY6NLty09bEq3j"
            //};

            //var rs = TONValidatorEngineSvc.GetMasterBlockTimeDifference(nodeIds);


            var rs = TONValidatorEngineSvc.GetMasterBlockTimeDifferenceForTonNetwork("-MNyZvn-qE9-GqubuwJZ");

            Assert.IsTrue(true);
        }

        

    }
}