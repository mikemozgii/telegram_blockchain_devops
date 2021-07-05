using NUnit.Framework;
using System;
using System.IO;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Ffi;

namespace TONBRAINS.TONOPS.UnitTest.Core.Services
{
    public class UserAccountSAIUnitTest
    {
        private PowerShellSvc PowerShellSvc { get; set; }


        [SetUp]
        public void Setup()
        {
            PowerShellSvc = new PowerShellSvc();

        }


        [Test]
        public void TestSmartContract()
        {
            var quantchainIds = new string[] { "-MQQIoCOHZtvwxRogmwn" };
            var abiBytea = File.ReadAllBytes("C:\\Users\\mikek\\source\\repos\\qunton_sc\\test1\\testwallet.abi.json");
            var solBytea = File.ReadAllBytes("C:\\Users\\mikek\\source\\repos\\qunton_sc\\test1\\testwallet.sol");
            var tvcBytea = File.ReadAllBytes("C:\\Users\\mikek\\source\\repos\\qunton_sc\\test1\\useraccount.tvc");
            var SmartContracSvc = new SmartContracSvc();
            var sc = new SmartContract();
            sc.Name = "UserAccountSCTest3";
            sc.Namespace = null;
            sc.TypeId = "common";
            sc.Version = "v0";
            sc.Description = "-";
            sc.LibId = "-MPqSvfzuvR3MHpfsgS_";

            SmartContracSvc.Add(abiBytea, solBytea, tvcBytea, sc);
            var id = Guid.NewGuid().ToString();
         
            var UserAccountSAI = new UserAccountSAI();
            UserAccountSAI.InitSmartContract(sc.Name);
            var sa =  UserAccountSAI.InitSmartAccountAndDeploy(id, quantchainIds, 25000);
            //a993dd57-3dcf-4a64-a3e5-155c6dcd32eb
            new UserAccountSAI().Init(sa.Id, quantchainIds).TransferWithFeeWaitForCompletion("c5214ec2-9bdd-45e8-a69d-082f29cead66", 5000, 33, "-1:1111111111111111111111111111111111111111111111111111111111111111");
        }

        [Test]
        public void TestSmartContract2()
        {
            var quantchainIds = new string[] { "-MQQIoCOHZtvwxRogmwn" };
            var saId = "a993dd57-3dcf-4a64-a3e5-155c6dcd32eb";
            new UserAccountSAI().Init(saId, quantchainIds).TransferWaitForCompletion("c5214ec2-9bdd-45e8-a69d-082f29cead66", 30);
        }

    }
}