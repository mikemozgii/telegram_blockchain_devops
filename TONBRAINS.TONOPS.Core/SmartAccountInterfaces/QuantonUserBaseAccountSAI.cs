using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Core.DALServices;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using TONBRAINS.TONOPS.Core.Helpers;
using TONBRAINS.TONOPS.Core.Handlers;
using System.Security.Cryptography;

namespace TONBRAINS.TONOPS.Core.SmartAccountInterfaces
{
    public class QuantonUserBaseAccountSAI : BaseSAI
    {

        public string _smartContractName { get; set; } = "QuantonUserBaseAccountSC";
        public string _smartContractId { get; set; } = "QuantonUserBaseAccountSC";
        public QuantonUserBaseAccountSAI()
        {

            _smartContractName = "QuantonUserBaseAccountSC";
            var sc = new SmartContractDbSvc().GetByName(_smartContractName);
            _smartContractId = sc.Id;
        }

        public BaseSAI InitSmartAccount(string customId, IEnumerable<string> qunatchainIds)
        {
            return InitSmartAccount(customId, null, _smartContractId, qunatchainIds);
        }

        public SmartAccount InitSmartAccountAndDeploy(string customId,  IEnumerable<string> qunatchainIds, long initamount = 1)
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

        public List<TransferDeaction> InitTokenTransfer(long amount, string email, string phone)
        {
            var TransferDeactionDalSvc = new TransferDeactionDalSvc();
            var rs = new List<TransferDeaction>();
            foreach (var sa in _qunatchainStateDic)
            {
                foreach (var quantchain in sa.Value)
                {
                    if (quantchain.Value.Balance > amount)
                    {
                        
                        var dtu = UnixTimeHelper.UnixTimeUTCNow();
                        var e = new TransferDeaction
                        {
                            Id = IdGenerator.Generate(),
                            Phone = phone,
                            Email = email,
                            AuthToken = GenerateToken(),
                            InitSmartAccountNetworkId = quantchain.Value.QunatchainStateId,
                            DateCreatedAt = dtu,
                            DateUpdateAt = dtu,
                            Status = TransferDeactionStatuses.WaitingForAuthToken,
                            Amount = amount
                        };

                        rs.Add(e);
                    }
                }
            }

            TransferDeactionDalSvc.Add(rs.ToArray());

            return rs;
        }


        public List<TransferDeaction> InitPayment(long amount)
        {
            var TransferDeactionDalSvc = new TransferDeactionDalSvc();
            var rs = new List<TransferDeaction>();
            foreach (var sa in _qunatchainStateDic)
            {
                foreach (var quantchain in sa.Value)
                {
                    var dtu = UnixTimeHelper.UnixTimeUTCNow();
                    var e = new TransferDeaction
                    {
                        Id = IdGenerator.Generate(),
                        AuthToken = GenerateToken(),
                        CompletedSmartAccountNetworkId = quantchain.Value.QunatchainStateId,
                        DateCreatedAt = dtu,
                        DateUpdateAt = dtu,
                        Status = TransferDeactionStatuses.Executing,
                        Amount = amount
                    };

                    rs.Add(e);
                }
            }

            TransferDeactionDalSvc.Add(rs.ToArray());

            return rs;
        }


        public bool DeleteTokenTransfer(string deactionId)
        {
            var TransferDeactionDalSvc = new TransferDeactionDalSvc();
            var e = TransferDeactionDalSvc.GetById(deactionId);
            if (e != null && _smartAccountIds.Contains(e.InitSmartAccountNetworkId) && e.Status != TransferDeactionStatuses.Completed && e.Status != TransferDeactionStatuses.Executing)
            {
                TransferDeactionDalSvc.DeleteByIds(deactionId);
                e.DateUpdateAt = UnixTimeHelper.UnixTimeUTCNow();
                return true;
            }
           
            return true;
        }

        public bool PauseTokenTransfer(string deactionId)
        {
            var TransferDeactionDalSvc = new TransferDeactionDalSvc();
            var e = TransferDeactionDalSvc.GetById(deactionId);
            if (e != null && _smartAccountIds.Contains(e.InitSmartAccountNetworkId) && e.Status != TransferDeactionStatuses.Completed && e.Status != TransferDeactionStatuses.Executing && e.Status != TransferDeactionStatuses.Paused)
            {
                e.Status = TransferDeactionStatuses.Paused;
                e.DateUpdateAt = UnixTimeHelper.UnixTimeUTCNow();
                TransferDeactionDalSvc.Update(e);
                return true;
            }

            return false;
        }

        public bool ResumeTokenTransfer(string deactionId)
        {
            var TransferDeactionDalSvc = new TransferDeactionDalSvc();
            var e = TransferDeactionDalSvc.GetById(deactionId);
            if (e != null && _smartAccountIds.Contains(e.InitSmartAccountNetworkId) && e.Status != TransferDeactionStatuses.Completed && e.Status != TransferDeactionStatuses.Executing && e.Status != TransferDeactionStatuses.WaitingForAuthToken)
            {
                e.Status = TransferDeactionStatuses.WaitingForAuthToken;
                e.DateUpdateAt = UnixTimeHelper.UnixTimeUTCNow();
             
                TransferDeactionDalSvc.Update(e);
            }

            return true;
        }

        public IEnumerable<TransferDeaction> GetTokenTransferStates()
        {
            var TransferDeactionDalSvc = new TransferDeactionDalSvc();
            var initSmartAccountNetworkIds = _meta_smartAccountStates.Select(q => q.Id);
            var es = TransferDeactionDalSvc.GetByInitSmartAccountNetworkIds(initSmartAccountNetworkIds);

            foreach (var e in es)
            {
                e.Date = e.DateCreatedAt;

                if (initSmartAccountNetworkIds.Contains(e.CompletedSmartAccountNetworkId))
                {
                    if (e.DateCompletedAt.HasValue) e.Date = e.DateCompletedAt.Value;
                    e.IsPayment = string.IsNullOrEmpty(e.InitSmartAccountNetworkId);
                    e.IsRecipient = !string.IsNullOrEmpty(e.InitSmartAccountNetworkId);
                }
            }

            return es.OrderByDescending(i => i.Date);
        }

        public TransferDeaction CompleteTokenTransfer(string authToken)
        {
            var TransferDeactionDalSvc = new TransferDeactionDalSvc();
            var e = TransferDeactionDalSvc.GetByAuthToken(authToken);
            if (e != null && !_smartAccountIds.Contains(e.InitSmartAccountNetworkId) && e.Status != TransferDeactionStatuses.Completed && e.Status != TransferDeactionStatuses.Executing && e.Status != TransferDeactionStatuses.Paused)
            {
                var fromAccountState = new SmartAccountNetworkDbSvc().GetById(e.InitSmartAccountNetworkId);
                var quantChainId = fromAccountState.NetworkId;

                var existtingQuantchain = _qunatchainStateDic.Values.First(q => q.Keys.Any(q1 => q1.Id == quantChainId));
                if (existtingQuantchain.Any())
                {
                    var existtingUserAccountState = existtingQuantchain.First(q => q.Key.Id == quantChainId);
                    new QuantonUserBaseAccountSAI().Init(fromAccountState.SmartAccountId, quantChainId).TransferWaitForCompletion(existtingUserAccountState.Value.Id, e.Amount);
                    var dtu = UnixTimeHelper.UnixTimeUTCNow();
                    e.Status = TransferDeactionStatuses.Completed;
                    e.DateCompletedAt = dtu;
                    e.CompletedSmartAccountNetworkId = existtingUserAccountState.Value.Id;
                    TransferDeactionDalSvc.Update(e);
                }


                return e;

            }

            return null;
        }


        public string GenerateToken(int keyLength = 32)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
