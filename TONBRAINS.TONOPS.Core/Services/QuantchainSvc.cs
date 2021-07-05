using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.Core.SmartAccountInterfaces;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class QuantchainSvc
    {
        public IEnumerable<string>  _qunatchainIds { get; set; }

        public QuantchainSvc()
        {
            _qunatchainIds = new List<string> { new TonNetworkDbSvc().GetByName("prodNet01").Id };
        }

        public QuantchainSvc(IEnumerable<string> qunatchainIds)
        {
            _qunatchainIds = qunatchainIds;
        }

        public SmartAccount CreateQuantonUserBaseAccount(string customId)
        {

            var sa = new QuantonUserBaseAccountSAI().InitSmartAccountAndDeploy(customId, _qunatchainIds);
            return sa;
        }

        public SmartAccount CreateQuantonUserBaseAccountByMnemonicPhrase(string customId, string mnemonicPhrase)
        {

            var sa = new QuantonUserBaseAccountSAI().InitSmartAccountAndDeploy(customId, _qunatchainIds, mnemonicPhrase);
            return sa;
        }

        public SmartAccount CreateQuantonUserBaseAccountBySecretAndPublicKey(string customId, string secretKey, string publicKey)
        {

            var sa = new QuantonUserBaseAccountSAI().InitSmartAccountAndDeploy(customId, _qunatchainIds, secretKey, publicKey);
            return sa;
        }

        public Dictionary<TonNetwork, SmartAccountMdl> GetQuantonUserBaseAccountState(string saId)
        {
            var sa = new QuantonUserBaseAccountSAI().Init(saId, _qunatchainIds).GetCurrentStateBySaIdAndQuantChainId(saId, _qunatchainIds.ToArray());
            return sa;
        }

        public TransferDeaction InitTokenTransfer(string saId, long amount, string email, string phone)
        {
            var sai = new QuantonUserBaseAccountSAI();
            sai.Init(saId, _qunatchainIds);
            var rs = sai.InitTokenTransfer(amount, email, phone);
            return rs.First();
        }

        public TransferDeaction InitPayment(string saId, long amount)
        {
            var sai = new QuantonUserBaseAccountSAI();
            sai.Init(saId, _qunatchainIds);
            var rs = sai.InitPayment(amount);
            return rs.First();
        }


        public bool DeleteTokenTransfer(string saId, string deactionId)
        {
            var sai = new QuantonUserBaseAccountSAI();
            sai.Init(saId, _qunatchainIds);
            var rs = sai.DeleteTokenTransfer(deactionId);
            return rs;
        }

        public bool PauseTokenTransfer(string saId, string deactionId)
        {
            var sai = new QuantonUserBaseAccountSAI();
            sai.Init(saId, _qunatchainIds);
            var rs = sai.PauseTokenTransfer(deactionId);
            return rs;
        }

        public bool ResumeTokenTransfer(string saId, string deactionId)
        {
            var sai = new QuantonUserBaseAccountSAI();
            sai.Init(saId, _qunatchainIds);
            var rs = sai.ResumeTokenTransfer(deactionId);
            return rs;
        }

        public IEnumerable<TransferDeaction> GetTokenTransferStates(string saId)
        {
            var sai = new QuantonUserBaseAccountSAI();
            sai.Init(saId, _qunatchainIds);
            var rs = sai.GetTokenTransferStates();
            return rs;
        }

        public bool SendAmountFromGiver(string toSaId, long amount)
        {
            var r = new MainWalletSAI().InitAlt(_qunatchainIds).TransferWaitForCompletion(toSaId, amount);
            return r;
        }


        public TransferDeaction CompleteTokenTransfer(string saId, string authToken)
        {
            var sai = new QuantonUserBaseAccountSAI();
            sai.Init(saId, _qunatchainIds);
            var r = sai.CompleteTokenTransfer(authToken);
            return r;
        }

        public string GetUserBaseAccountByMnemonicPhrase(string mnemonicPhrase)
        {
            var cryptoSvc = new CryptoSvc();
            var keyPair = cryptoSvc.GetKeyPair(mnemonicPhrase);
            var sas = new SmartAccountDbSvc().GetAccounts_BySecretAndPublicKey(keyPair.SecretKey, keyPair.PublicKey);
            if (sas!= null)
            {
                return sas.SmartAccountId;
            }
           
            return null;
        }

        public string GetUserBaseAccountBySecretAndPublicKey(string secretKey, string publicKey)
        {
            var sas = new SmartAccountDbSvc().GetAccounts_BySecretAndPublicKey(secretKey, publicKey);
            if (sas != null)
            {
                return sas.SmartAccountId;
            }

            return null;
        }
    }
}
