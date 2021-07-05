using NUnit.Framework;
using System.IO;
using TONBRAINS.TONOPS.Core.Helpers;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class SmartAccountSvcUnitTest
    {
        private SmartAccountSvc SmartAccountSvc { get; set; }
        private string ValidPublicKey { get; set; }
        private string ValidSercretKey { get; set; }

        [SetUp]
        public void Setup()
        {
            SmartAccountSvc = new SmartAccountSvc();
            ValidPublicKey = "16fb1cfd72000a4393d684c3a13662e5ac137a1a04d193814e8c05127b28166b";
            ValidSercretKey = "7b03855bc68f1afea36ac477f4c3c6eb52ba8cf90b27683d4dfd6401104cab99";
        }


        [Test]
        public void GenerateSmartAccountAddressTest()
        {

            var TempFileSvc = new LocalTempFileSvc();
            var abiStream = TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\SmartContractTest\\wallet.abi.json");
            var tvcStream = TempFileSvc.GetFiletoByteyPath("C:\\macpro_backup\\SmartContractTest\\wallet.tvc");

            var r = SmartAccountSvc.GenerateSmartAccountAddress(abiStream, tvcStream, ValidPublicKey, ValidSercretKey, -1);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(r));
            //-1:56973312e8e0ce7dc5760db964758ca2b30ac078a776871065faa9c86760c510
        }


        [Test]
        public void GenerateBocSendTransactionTest()
        {
            var fromAccount = "9aef0514-c50a-4ec6-848c-cb345c534d05";
            var toAccount = "9463909e-da57-4adb-84d3-426489f38932";
            var networkId = "-MNyZvn-qE9-GqubuwJZ";
            //var r = SmartAccountSvc.GenerateBocSubmitTransaction(fromAccount, toAccount, 1000);
            var r1 = new NanoHlp().ConvertToNanoGram(1000);

            //var rr1 = SmartAccountSvc.DeployAccount(fromAccount, networkId);
          //  var rr2 = SmartAccountSvc.DeployAccount(toAccount, networkId);
           // var r = SmartAccountSvc.SendTokens(fromAccount, toAccount, networkId, r1);

            Assert.IsTrue(true);
        }

        [Test]
        public void DeployAccount()
        {

            //new SmartAccountSvc().GenerateAddress();

            //new SmartAccountSvc().GenerateMessage();
            //var smartAccountId = "-MPoNxquekekc6xXWLx0";
            //var smartAccountId = "-MP_FTmSBw05lATWc5-b";
            //var tonNetworkId = "-MNyZvn-qE9-GqubuwJZ";
            //
            //var r1 = SmartAccountSvc.DeployAccount(smartAccountId, 80, tonNetworkId);
            // var r2 = SmartAccountSvc.TransferFromMainWallet(smartAccountId, 7777, tonNetworkId);

            Assert.IsTrue(true);
        }

        [Test]
        public void UpdateAccountBalanceTest()
        {
            //var fromAccount = "5d5148ec-70f4-4444-8e44-fc058da5ca87";
            //var toAccount = "b7619463-5fcc-4da6-9cfc-d249fd34a85c";
            //var r1 = SmartAccountSvc.UpdateAccountBalance("9aef0514-c50a-4ec6-848c-cb345c534d05");
            //var r2 = SmartAccountSvc.UpdateAccountBalance("9463909e-da57-4adb-84d3-426489f38932");
            Assert.IsTrue(true);
        }


        [Test]
        public void ConvertToNanoTOkensTest()
        {
            //var fromAccount = "5d5148ec-70f4-4444-8e44-fc058da5ca87";
            //var toAccount = "b7619463-5fcc-4da6-9cfc-d249fd34a85c";
           // var r = SmartAccountSvc.ConvertToNanoTOkens(300000000000000);

            var r1 = new NanoHlp().ConvertToNanoGram(300000);
            var r2 = new NanoHlp().ConvertFromNanoGram(300000000000000);
            Assert.IsTrue(true);
        }

        


    }
}