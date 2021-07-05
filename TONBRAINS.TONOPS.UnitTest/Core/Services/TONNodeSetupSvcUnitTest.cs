using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Core.SSH;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class TONNodeSetupSvcUnitTest
    {
       // public TONNodeSetupSvc TONNodeSetupSvc { get; set; }

        [SetUp]
        public void Setup()
        {
           // TONNodeSetupSvc = new TONNodeSetupSvc();

        }


        [Test]
        public void PreInit_ExtTest()
        {


           // var dd = "18B78A4E280D56A4E9632AE0210396FA43C53D3F3C05FF8D125F6946CE268745".ToBase64FromTonHex();

            var TONNodeSetupSvc = new TONNodeSetupSvc();


            //var nodeIds1 = new string[] {
            //"-MNPvvqpyCGGDRpvHPlF",
            //"-MNPwR3VPiQhm91P2LwP",
            //"-MNPwvjIBSUQIUbNvdIX",
            //"-MNPxQq8FSKUMb8pyiEj",
            //"-MNPxuYY6NLty09bEq3j"

            //};
            //TONNodeSetupSvc.PreInit_Ext1(nodeIds1);

            //var nodeIds = new string[] {
            //"-MNPvvqpyCGGDRpvHPlF",
            //"-MNPwR3VPiQhm91P2LwP",
            //"-MNPwvjIBSUQIUbNvdIX",
            //"-MNPxQq8FSKUMb8pyiEj",
            //"-MNPxuYY6NLty09bEq3j"
            //};


            //TONNodeSetupSvc.StopValidatorEngine(nodeIds);
            //TONNodeSetupSvc.PreInit_Ext(nodeIds);      
            //TONNodeSetupSvc.SetupZeroState(nodeIds);
            //TONNodeSetupSvc.RunValidatorEngine(nodeIds);

            // new TONNodeMaitananceSvc().CheckNodeStatus(nodeIds);
            Assert.IsTrue(true);
        }

        [Test]
        public void SetupZeroState()
        {
          //  var TONNodeSetupSvc = new TONNodeSetupSvc();

          //  var nodeIds = new string[] {
          //"-MNPvvqpyCGGDRpvHPlF",
          //  "-MNPwR3VPiQhm91P2LwP",
          //  "-MNPwvjIBSUQIUbNvdIX",
          //  "-MNPxQq8FSKUMb8pyiEj",
          //  "-MNPxuYY6NLty09bEq3j"
          //  };


          //  TONNodeSetupSvc.SetupZeroState(nodeIds);
            Assert.IsTrue(true);
        }

        [Test]
        public void RunValidatorEngineTest()
        {
            var TONNodeSetupSvc = new TONNodeSetupSvc();


            //var nodeIds1 = new string[] {

            //"-MNCXkw5je7aRiebjfNo"
            //};
            //TONNodeSetupSvc.PreInit_Ext1(nodeIds1);

            var nodeIds = new string[] {
          "-MNPvvqpyCGGDRpvHPlF",
            //"-MNPwR3VPiQhm91P2LwP",
            //"-MNPwvjIBSUQIUbNvdIX",
            //"-MNPxQq8FSKUMb8pyiEj",
            //"-MNPxuYY6NLty09bEq3j"
            };

           // TONNodeSetupSvc.GenerateGlobalConfig(nodeIds);
            //TONNodeSetupSvc.RunValidatorEngine(nodeIds);
            Assert.IsTrue(true);
        }

        [Test]
        public void ProccessMainWalletKeysTest()
        {
            var TONNodeSetupSvc = new TONNodeSetupSvc();


            //var nodeIds1 = new string[] {

            //"-MNCXkw5je7aRiebjfNo"
            //};
            //TONNodeSetupSvc.PreInit_Ext1(nodeIds1);

            var nodeIds = new string[] {
          "-MNyL4u9DVeo4489J7cf",
            //"-MNPwR3VPiQhm91P2LwP",
            //"-MNPwvjIBSUQIUbNvdIX",
            //"-MNPxQq8FSKUMb8pyiEj",
            //"-MNPxuYY6NLty09bEq3j"
            };

            // TONNodeSetupSvc.GenerateGlobalConfig(nodeIds);
            //TONNodeSetupSvc.RunValidatorEngine(nodeIds);
            var SSHCommandHlpFirst = new SSHCommandHlp(nodeIds.First());
            //TONNodeSetupSvc.ProccessMainWalletKeys(SSHCommandHlpFirst);
            Assert.IsTrue(true);
        }
        //
    }
}


//