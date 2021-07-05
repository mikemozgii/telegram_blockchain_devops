using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.SmartAccountInterfaces;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class MainWalletSAI : BaseSAI
    {
    
        public BaseSAI InitAlt(IEnumerable<string> qunatchainIds, IScheduler scheduler = null)
        {
            var mainWalletSmartAccounts = new SmartAccountDbSvc().GetMainWalletAccountByNetworks(qunatchainIds.ToArray());
            return Init(mainWalletSmartAccounts.Select(q => q.Id), qunatchainIds, null);
        }

        public BaseSAI InitAlt(params string[] qunatchainIds)
        {
            return InitAlt(qunatchainIds, null);
        }

        //public bool TransferFromMainWallet(string deploySmartAccountId)
        //{
        //    foreach (var qunatchainId in _qunatchainIds)
        //    {
                
        //        var r = ExecuteTransferNewSmartAccount(mainWalletSmartAccount.Id, deploySmartAccountId, amount, tonNetworkId);
        //    }

        //    return true;
        //}

        //public bool TransferFromMainWalletWaitForCompletion(string deploySmartAccountId, long amount, params string[] tonNetworkIds)
        //{
        //    foreach (var tonNetworkId in tonNetworkIds)
        //    {
        //        var mainWalletSmartAccount = new SmartAccountDbSvc().GetMainWalletAccountByNetwork(tonNetworkId);
        //        var r = ExecuteTransferNewSmartAccount(mainWalletSmartAccount.Id, deploySmartAccountId, amount, tonNetworkId);
        //    }

        //    return true;
        //}

    }
}
