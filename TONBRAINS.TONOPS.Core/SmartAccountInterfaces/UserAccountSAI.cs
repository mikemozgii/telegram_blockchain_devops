using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Helpers;
using TONBRAINS.TONOPS.Core.SmartAccountInterfaces;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class UserAccountSAI : BaseSAI
    {

        public string _smartContractName { get; set; } = "UserAccount";
        public string _smartContractId { get; set; }

        public UserAccountSAI()
        {
            InitSmartContract();
        }

        public void InitSmartContract(string smartContractName = null)
        {
            if (!string.IsNullOrWhiteSpace(smartContractName))
            {
                _smartContractName = smartContractName;
            }

            var sc = new SmartContractDbSvc().GetByName(_smartContractName);
            _smartContractId = sc.Id;
        }


        public BaseSAI InitSmartAccount(string customId, IEnumerable<string> qunatchainIds)
        {
            return InitSmartAccount(customId, null, _smartContractId, qunatchainIds);
        }

        public SmartAccount InitSmartAccountAndDeploy(string customId, IEnumerable<string> qunatchainIds, long initamount = 1)
        {
            return InitSmartAccountAndDeploy(customId, _smartContractId, qunatchainIds, initamount);
        }
        public SmartAccount InitSmartAccountAndDeploy(string customId, IEnumerable<string> qunatchainIds, string mnemonicPhrase, long initamount = 1)
        {
            return InitSmartAccountAndDeploy(customId, _smartContractId, qunatchainIds, mnemonicPhrase, initamount);
        }
        public SmartAccount InitSmartAccountAndDeploy(string customId, IEnumerable<string> qunatchainIds, string secretKey, string publicKey, long initamount = 1)
        {
            return InitSmartAccountAndDeploy(customId, _smartContractId, qunatchainIds, secretKey, publicKey, initamount);
        }

    }
}
