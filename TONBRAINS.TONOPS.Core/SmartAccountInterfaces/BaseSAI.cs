using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Enums;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Helpers;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.Core.Models.AbiMdls;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.Ffi;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.SmartAccountInterfaces
{
    public class BaseSAI
    {


        public IScheduler _scheduler { get; set; }
        public IEnumerable<string> _smartAccountIds { get; set; }
        public IEnumerable<string> _qunatchainIds { get; set; }

        public Dictionary<SmartAccount, Dictionary<TonNetwork, SmartAccountMdl>> _qunatchainStateDic { get; set; }
        public Dictionary<SmartAccount, Dictionary<TonNetwork, SmartAccountMdl>> _init_qunatchainStateDic { get; set; }


        public List<SmartAccount> _meta_smartAccounts { get; set; }
        public List<SmartAccountNetwork> _meta_smartAccountStates { get; set; }
        public List<TonNetwork> _meta_quantchains { get; set; }

        public BaseSAI Init(IEnumerable<string> saIds, IEnumerable<string> qunatchainIds, IScheduler scheduler = null)
        {
            _smartAccountIds = saIds;
            _qunatchainIds = qunatchainIds;
            _scheduler = scheduler;
            FillInitData();

            return this;
        }

        public BaseSAI Init(IEnumerable<string> saIds, params string[] qunatchainIds)
        {
            return Init(saIds, qunatchainIds, null);
        }

        public BaseSAI Init(IEnumerable<string> saIds, IScheduler scheduler = null)
        {
            return Init(saIds, new List<string>(), scheduler);
        }

        public BaseSAI Init(string saId, IScheduler scheduler = null)
        {
            return Init(new string[] { saId }, new List<string>(), scheduler);
        }

        public BaseSAI Init(string saId, string qunatchainId, IScheduler scheduler = null)
        {
            return Init(new string[] { saId }, new string[] { qunatchainId }, scheduler);
        }

        public BaseSAI Init(string saId, IEnumerable<string> qunatchainIds, IScheduler scheduler = null)
        {
            return Init(new string[] { saId }, qunatchainIds, scheduler);
        }

        public void FillInitData()
        {
            _meta_smartAccounts = new SmartAccountDbSvc().GetByIdsWithInclude(_smartAccountIds).ToList();
            _meta_smartAccountStates = new List<SmartAccountNetwork>();
            foreach (var sa in _meta_smartAccounts)
            {
                _meta_smartAccountStates.AddRange(sa.SmartAccountNetworks);
            }
            _meta_smartAccountStates = _meta_smartAccountStates.Distinct().ToList();


            _meta_quantchains = new List<TonNetwork>();
            if (_qunatchainIds == null)
            {
                _qunatchainIds = _meta_smartAccountStates.Select(q => q.Id).Distinct();
            }
            else
            {
                _meta_smartAccountStates = _meta_smartAccountStates.Where(q=> _qunatchainIds.Contains(q.NetworkId)).Distinct().ToList();

            }

            _meta_quantchains = _meta_smartAccountStates.Select(q => q.TonNetwork).Distinct().ToList();

            FillStateData();
            CopyStateToInitStateData();


        }

        public void FillStateData()
        {
            _qunatchainStateDic = new Dictionary<SmartAccount, Dictionary<TonNetwork, SmartAccountMdl>>();
            foreach (var sa in _meta_smartAccounts)
            {
                var nets = _meta_smartAccountStates.Where(q => q.SmartAccountId == sa.Id);
                var qchs = _meta_quantchains.Where(q => nets.Select(q => q.NetworkId).Contains(q.Id)).ToList();
                var ts = new Dictionary<TonNetwork, SmartAccountMdl>();
                foreach (var qch in qchs)
                {
                    var state = nets.FirstOrDefault(q => q.NetworkId == qch.Id);
                   

                    var r = new SmartAccountMdl()
                    {
                        Id = sa.Id,
                        Phrase = sa.SmartAccountKeys.First().SmartKey.MnemonicPhrase,
                        PublicKey = sa.SmartAccountKeys.First().SmartKey.PublicKey,
                        SecretKey = sa.SmartAccountKeys.First().SmartKey.SecretKey,
                        TonSafeKey = sa.SmartAccountKeys.First().SmartKey.TonSafePublicKey,
                        SmartContractName = sa.SmartContract.Name,
                        SmartContractVersion = sa.SmartContract.Version,
                        Address = sa.Address
                    };

                    r.Balance = state.Balance;
                    r.QunatchainName = state.TonNetwork.Name;
                    r.QunatchainId = state.TonNetwork.Id;
                    r.Status = state.Status;
                    r.QunatchainStateId = state.Id;
                    r.IsDeployed = state.IsDeployed;

                    ts.Add(qch, r);
                }

                _qunatchainStateDic.Add(sa, ts);
            }

        }

        public void CopyStateToInitStateData()
        {
            _init_qunatchainStateDic = new Dictionary<SmartAccount, Dictionary<TonNetwork, SmartAccountMdl>>();

            foreach (var qchs in _qunatchainStateDic)
            {
                var t = new Dictionary<TonNetwork, SmartAccountMdl>();

                foreach (var ts in qchs.Value)
                {
                    t.Add(ts.Key.Clone(), ts.Value.Clone());
                }

                _init_qunatchainStateDic.Add(qchs.Key.Clone(), t);
            }
        }

        public void UpdateStateData(string saId = null, string quantchainId = null)
        {
            var saIds = _smartAccountIds;
            if (!string.IsNullOrWhiteSpace(saId))
            {
                saIds = new string[] { saId };
            }

            var sas = _meta_smartAccounts.Where(q => saIds.Contains(q.Id)).ToList();

            // need to ge news state from db // metadata is not updating.
            var sma_networks = new SmartAccountNetworkDbSvc().GetBySmartAccountIds(_smartAccountIds).ToList();

            var qunatchainIds = _qunatchainIds;
            var quantchains = new List<TonNetwork>();
            if (!string.IsNullOrWhiteSpace(quantchainId))
            {
                _qunatchainIds = new string[] { quantchainId };
            }

            quantchains = _meta_quantchains.Where(q => _qunatchainIds.Contains(q.Id)).ToList();

            foreach (var sa in _qunatchainStateDic)
            {
                var s1 = sas.FirstOrDefault(q => q.Id == sa.Key.Id);
                if (s1 == null)
                    continue;

                foreach (var val in sa.Value)
                {
                    var v1 = quantchains.FirstOrDefault(q => q.Id == val.Key.Id);
                    if (v1 == null)
                        continue;

                    var state = sma_networks.FirstOrDefault(q => q.SmartAccountId == s1.Id && q.NetworkId == v1.Id);

                    var r = new SmartAccountMdl()
                    {
                        Id = sa.Key.Id,
                        Phrase = sa.Key.SmartAccountKeys.First().SmartKey.MnemonicPhrase,
                        PublicKey = sa.Key.SmartAccountKeys.First().SmartKey.PublicKey,
                        SecretKey = sa.Key.SmartAccountKeys.First().SmartKey.SecretKey,
                        TonSafeKey = sa.Key.SmartAccountKeys.First().SmartKey.TonSafePublicKey,
                        SmartContractName = sa.Key.SmartContract.Name,
                        SmartContractVersion = sa.Key.SmartContract.Version,
                        Address = sa.Key.Address
                    };

                    r.Balance = state.Balance;                
                    r.Status = state.Status;
                    r.QunatchainStateId = state.Id;
                    r.IsDeployed = state.IsDeployed;

                    // does not include TON NETworks
                    r.QunatchainName = val.Value.QunatchainName;
                    r.QunatchainId = val.Value.QunatchainId;


                    sa.Value[val.Key] = r;
                }
            }


        }

        public bool UpdateState(string saId = null, string quantchainId = null)
        {
            var rs = new Dictionary<string, long>();
            var qunatchainsDic = _qunatchainStateDic;

            // filtering saIds if any
            if (!string.IsNullOrWhiteSpace(saId))
            {
                qunatchainsDic = _qunatchainStateDic.Where(q => q.Key.Id == saId).ToDictionary(q => q.Key, q => q.Value);
            }

            //than filtering by quantchain
            if (!string.IsNullOrWhiteSpace(quantchainId))
            {
                qunatchainsDic = qunatchainsDic.ToDictionary(q => q.Key, q => q.Value.Where(q1 => q.Key.Id == quantchainId).ToDictionary(q2 => q2.Key, q2 => q2.Value));
            }

            foreach (var sa in qunatchainsDic)
            {
                foreach (var quantchain in sa.Value)
                {
                    long balance = 0;
                    var node = new NodeDbSvc().GetRandomExecutionNodeByTonNeworkId(quantchain.Key.Id);
                    var accountRawState = new TONClientSvc().GetAccount(sa.Key.Address, node.Id).First().Value.First();

                    var accountState = quantchain.Value;
                    if (accountState == null)
                    {
                        return true;
                    }


                    var accountStatus = SmartAccountStatuses.undefined;

                    if (!accountRawState.Contains("account state is empty"))
                    {
                        var r = Regex.Matches(accountRawState, "[\n\r].*account balance is\\s*([^\n\r]*)")[0].ToString();
                        int index = r.IndexOf("ng");
                        if (index > 0)
                            r = r.Substring(0, index);
                        r = r.Replace("account balance is", "").Replace("ng", "").Replace("(", "").Trim();

                        balance = long.Parse(r.Replace("\"", ""));

                    }

                    if (accountRawState.Contains("account state is empty"))
                    {
                        accountStatus = SmartAccountStatuses.undefined;
                        accountState.IsDeployed = false;
                    }
                    else if (accountRawState.Contains("account_uninit"))
                    {
                        accountStatus = SmartAccountStatuses.uninit;
                        accountState.IsDeployed = false;
                    }
                    else if (accountRawState.Contains("account_active"))
                    {
                        accountStatus = SmartAccountStatuses.active;
                        accountState.IsDeployed = true;
                    }

                    rs.Add(quantchain.Key.Id, balance);

                    accountState.Balance = balance;
                    accountState.Status = accountStatus;
                    accountState.Status = accountStatus;

                    var accountStateEntity = _meta_smartAccountStates.First(q => q.Id == accountState.QunatchainStateId);
                    accountStateEntity.Balance = balance;
                    accountStateEntity.Status = accountStatus;
                    accountStateEntity.StatusId = accountStatus;
                    var svc = new SmartAccountNetworkDbSvc();
                    svc.Update(accountStateEntity);
                    svc.SaveAccountStateMetric(sa.Key.Id, quantchain.Key.Id, accountRawState, accountStatus, balance);
                    UpdateStateData(saId, quantchainId);
                }
            }

            return true;
        }

        public bool AddInitAccountStateIfDoesnoTexist()
        {
            foreach (var sa in _qunatchainStateDic)
            {
                foreach (var quantchain in sa.Value)
                {
                    if (quantchain.Value == null)
                    {
                        var sm = new SmartAccountNetwork()
                        {
                            Id = Guid.NewGuid().ToString(),
                            IsDeployed = false,
                            NetworkId = quantchain.Key.Id,
                            Balance = 0,
                            Status = "undefined",
                            StatusId = "undefined",
                            SmartAccountId = sa.Key.Id
                            //StatusId = "not active"
                        };
                        new SmartAccountNetworkDbSvc().Add(sm);
                        UpdateState(sa.Key.Id, quantchain.Key.Id);
                    }

                }
            }

            return true;
        }

        public bool DeployAccount(long amount = 1)
        {
            UpdateState();
            AddInitAccountStateIfDoesnoTexist();

            foreach (var sa in _qunatchainStateDic)
            {
                foreach (var quantchain in sa.Value)
                {
                    if (quantchain.Value.Balance == 0)
                    {
                       var d = new MainWalletSAI().InitAlt(quantchain.Key.Id);
                       d.TransferToNonExistinAccount(sa.Key.Id, amount);
                    }
                }
            }


            WaitTillBalanceIncrease();

            InitConstructor();

            WaitTillAccountActive();

            return true;
        }

        public bool WaitTillBalanceIncrease(string saId, string quantchainId, long currentBalance)
        {
            Task.Delay(10000).Wait();
            UpdateState(saId, quantchainId);
            var udpate = _qunatchainStateDic.First(q => q.Key.Id == saId).Value.First(q => q.Key.Id == quantchainId);
            if (currentBalance > udpate.Value.Balance)
            {
                return WaitTillBalanceIncrease(saId, quantchainId, currentBalance);
            }
            return true;
        }

        public bool WaitTillBalanceIncrease()
        {
            Task.Delay(10000).Wait();
            UpdateState();
            foreach (var sa in _qunatchainStateDic)
            {
                foreach (var quantchain in sa.Value)
                {
                    var initState = _init_qunatchainStateDic.First(q => q.Key.Id == sa.Key.Id).Value.First(q => q.Key.Id == quantchain.Key.Id);

                    if (initState.Value.Balance == quantchain.Value.Balance)
                    {

                        return WaitTillBalanceIncrease();
                    }
                }
            }
            return true;
        }

        public bool WaitTillAccountActive(string saId, string quantchainId)
        {
            Task.Delay(10000).Wait();
            UpdateState(saId, quantchainId);
            var udpate = _qunatchainStateDic.First(q => q.Key.Id == saId).Value.First(q => q.Key.Id == quantchainId);
            if (udpate.Value.Status != "active")
            {
                return WaitTillAccountActive(saId, quantchainId);
            }

            return true;
        }

        public bool WaitTillAccountActive()
        {
            Task.Delay(10000).Wait();
            UpdateState();
            foreach (var sa in _qunatchainStateDic)
            {
                foreach (var quantchain in sa.Value)
                {
                    if (quantchain.Value.Status != "active")
                    {

                        return WaitTillBalanceIncrease();
                    }
                }
            }
            return true;
        }

        public bool TransferWithFee(string toSAId, long amount, long feeAmount, string feedest, bool bounce = true)
        {
            foreach (var sa in _qunatchainStateDic)
            {
                if (amount == 0)
                    return false;
                // var amountNg = new NanoHlp().ConvertToNanoGram(amount);
                var toSA = new SmartAccountDbSvc().GetById(toSAId);
                var inputValues = new Dictionary<string, string>
            {
                { "dest", toSA.Address },
                { "value", amount.ToString() },
                { "bounce", bounce.ToString() },
                { "feedest", feedest },
                { "fee", feeAmount.ToString() },
            };
                var funcBocMessage = GenerateBoc(sa.Key.Id, "transferWithFee", inputValues);

                foreach (var quantchain in sa.Value)
                {


                    ExecuteBoc(funcBocMessage, quantchain.Key.Id);
                    AddTransferLog(sa.Key.Id, toSAId, amount, quantchain.Key.Id);

                    //TODO
                    //var tasks = new string[] { _saIds, toSAId }.Select(q => new Task(() => { UpdateState(q, tonNetworkIds); })).ToList();
                    //new BackGroundTaskHlp(_scheduler).RunTasksInBackgorund(tasks, GlobalAppConfHandler.TonNetMsgExecWaitTime);
                }
            }

            return true;
        }
        public bool TransferWithFeeWaitForCompletion(string toSAId, long amount, long feeAmount, string feedest, bool bounce = true)
        {
            TransferWithFee(toSAId, amount, feeAmount, feedest, bounce);
            new BaseSAI().Init(toSAId, _qunatchainIds).WaitTillBalanceIncrease();
            UpdateState();
            return true;
        }

        public bool Transfer(string toSAId, long amount, bool bounce = true)
        {
            foreach (var sa in _qunatchainStateDic)
            {
                if (amount == 0)
                    return false;
                // var amountNg = new NanoHlp().ConvertToNanoGram(amount);
                var toSA = new SmartAccountDbSvc().GetById(toSAId);
                var inputValues = new Dictionary<string, string>
            {
                { "dest", toSA.Address },
                { "value", amount.ToString() },
                { "bounce", bounce.ToString() },
            };
                var funcBocMessage = GenerateBoc(sa.Key.Id, "transfer", inputValues);

                foreach (var quantchain in sa.Value)
                {


                    ExecuteBoc(funcBocMessage, quantchain.Key.Id);
                    AddTransferLog(sa.Key.Id, toSAId, amount, quantchain.Key.Id);

                    //TODO
                    //var tasks = new string[] { _saIds, toSAId }.Select(q => new Task(() => { UpdateState(q, tonNetworkIds); })).ToList();
                    //new BackGroundTaskHlp(_scheduler).RunTasksInBackgorund(tasks, GlobalAppConfHandler.TonNetMsgExecWaitTime);
                }
            }

            return true;
        }

        public bool Transfer(Dictionary<string, long> toSAs, bool bouce = false)
        {
            foreach (var toSA in toSAs)
            {
                Transfer(toSA.Key, toSA.Value, bouce);
            }

            return true;
        }

        public bool TransferWaitForCompletion(string toSAId, long amount, bool bounce = true)
        {
            Transfer(toSAId, amount, bounce);
            new BaseSAI().Init(toSAId, _qunatchainIds).WaitTillBalanceIncrease();
            UpdateState();
            return true;
        }

        public bool TransferToNonExistinAccount(string toSAId, long amount)
        {
            return Transfer(toSAId, amount, false);
        }

        public bool TransferToNonExistinAccount(Dictionary<string, long> toSAs)
        {
            return Transfer(toSAs, false);
        }

        public bool TransferWaitForCompletion(string toSAId, long amount)
        {
            return TransferWaitForCompletion(toSAId, amount, false);
        }

        public bool AddTransferLog(string fromSAId, string toSAId, long amount, params string[] tonNetworkIds)
        {
            var dtn = DateTime.UtcNow;

            foreach (var tonNetworkId in tonNetworkIds)
            {
                var from_smartAccountInfoes = new SmartAccountNetworkDbSvc().GetBySmartAccountIdAndTonNetworkId(fromSAId, tonNetworkId);
                var to_smartAccountInfoes = new SmartAccountNetworkDbSvc().GetBySmartAccountIdAndTonNetworkId(toSAId, tonNetworkId);

                if (to_smartAccountInfoes != null)
                {
                    var log = new SmartAccountBalanceTransferLog
                    {
                        Id = IdGenerator.Generate(),
                        NetworkId = tonNetworkId,
                        TransferTime = dtn,
                        //TODO: the balance of the sender must be here
                        SmartAccountFromBalance = amount,
                        SmartAccountFromNetworkId = from_smartAccountInfoes.Id,
                        TransferBalance = amount,
                        //TODO: the balance of the recipient must be here
                        SmartAccountToBalance = amount,
                        SmartAccountToNetworkId = to_smartAccountInfoes.Id

                    };
                    var r = new SmartAccountTransferLogDBSvc().Add(log);
                }
            }

            return true;
        }

        public byte[] GenerateBoc(string abi, byte[] tvc, string address, string funcName, KeyPairMdl keyPair, Dictionary<string, string> inputValues = null)
        {
            var keysFileContent = GenerateKeyContent(keyPair);
            var prams = GenerateParams(abi, funcName, inputValues);

            if (string.IsNullOrWhiteSpace(prams))
            {
                prams = "{}";
            }
            var bocBytea1 = new TonWorkLib().GenerateMessageBoc(address, abi, funcName, prams, keysFileContent, tvc);
            return bocBytea1;
        }

        public byte[] GenerateBoc(string saId, string funcName, Dictionary<string, string> inputValues = null)
        {
            var smartAccount = new SmartAccountDbSvc().GetById(saId);
            var smartKeyView = new SmartAccountDbSvc().GetAccountSmartKeys(saId).First();
            var smartContract = new SmartContractDbSvc().GetById(smartAccount.SmartContractId);
            var keyPair = new KeyPairMdl
            {
                PublicKey = smartKeyView.PublicKey,
                SecretKey = smartKeyView.SecretKey,
                TonSafePublicKey = smartKeyView.TonSafePublicKey
            };
            var abiFile = new NpgSQLHlp().GetFile(new FileEntityDbSvc().GetById(smartContract.AbiFileId).Oid);
            return GenerateBoc(abiFile, smartAccount.Address, funcName, keyPair, inputValues);
        }

        public byte[] GenerateBoc(byte[] abi, string address, string funcName, KeyPairMdl keyPair, Dictionary<string, string> inputValues = null)
        {
            var keysFileContent = GenerateKeyContent(keyPair);
            var prams = GenerateParams(new CommonHelprs().GetStringFromNBytea(abi), funcName, inputValues);
            prams = ConvertValsToWindows(prams);

            var bocBytea1 = new TonWorkLib().GenerateMessageBoc(address, new CommonHelprs().GetStringFromNBytea(abi), funcName, prams, keysFileContent);

            return bocBytea1;
        }

        public string GenerateKeyContent(KeyPairMdl keyPair)
        {
            return "{" + $"{Environment.NewLine}\"public\": \"{keyPair.PublicKey}\",{Environment.NewLine}\"secret\": \"{keyPair.SecretKey}\"{Environment.NewLine}" + "}";
        }

        public string GenerateParams(string abiContent, string funcName, Dictionary<string, string> inputValues)
        {

            if (inputValues == null || !inputValues.Any())
            {
                return "";
            }

            var abi = JsonConvert.DeserializeObject<AbiMdl>(abiContent);
            var func = abi.functions.FirstOrDefault(q => string.Equals(q.name, funcName, StringComparison.OrdinalIgnoreCase));
            if (func == null)
                return null;
            var ps = new List<string>();
            foreach (var p in func.inputs)
            {
                if (inputValues.TryGetValue(p.name, out var value))
                {
                    if (p.type == "address")
                    {
                        ps.Add(value.ToVVwithKey(p.name));
                    }
                    else if (p.type == "uint128")
                    {
                        ps.Add(value.WithKey(p.name));
                    }
                    else if (p.type == "bool")
                    {
                        ps.Add(value.ToLower().WithKey(p.name));
                    }
                    else if (p.type == "cell")
                    {
                        ps.Add("".ToVV().WithKey(p.name));
                    }

                }
                else
                {
                    if (p.type == "address")
                    {
                        throw new ArgumentException("address need to be present");
                    }
                    else if (p.type == "uint128")
                    {
                        throw new ArgumentException("uint128 need to be present");
                    }
                    else if (p.type == "bool")
                    {
                        ps.Add("false".WithKey(p.name));
                    }
                    else if (p.type == "cell")
                    {
                        ps.Add("".ToVV().WithKey(p.name));
                    }
                }

            }

            if (ps.Any())
            {
                return string.Join(",", ps);
            }

            return "";
        }

        public string ConvertValsToWindows(string input)
        {
            input = ParseQuotes(input);
            input = "{" + input + "}";
            return input;
        }

        public string ParseQuotes(string input)
        {

            return input.Replace("^^", "\"");
        }

        public bool InitConstructor(Dictionary<string, string> inputValues = null)
        {
            foreach (var sa in _qunatchainStateDic)
            {
                var smartAccount = new SmartAccountDbSvc().GetById(sa.Key.Id);
                var smartKeyView = new SmartAccountDbSvc().GetAccountSmartKeys(sa.Key.Id).First();
                var smartContract = new SmartContractDbSvc().GetById(smartAccount.SmartContractId);
                var keyPair = new KeyPairMdl
                {
                    PublicKey = smartKeyView.PublicKey,
                    SecretKey = smartKeyView.SecretKey,
                    TonSafePublicKey = smartKeyView.TonSafePublicKey
                };

                var abiContent = smartContract.AbiJson;
                if (string.IsNullOrWhiteSpace(abiContent))
                {
                    var abiFile = new NpgSQLHlp().GetFile(new FileEntityDbSvc().GetById(smartContract.AbiFileId).Oid);
                    abiContent = new CommonHelprs().GetStringFromNBytea(abiFile);
                }
                var tvcFile = new NpgSQLHlp().GetFile(new FileEntityDbSvc().GetById(smartContract.TvcFileId).Oid);
                var transferBocMessage = GenerateBoc(abiContent, tvcFile, smartAccount.Address, "constructor", keyPair, inputValues);


                foreach (var quantchain in sa.Value)
                {
                    ExecuteBoc(transferBocMessage, quantchain.Key.Id);
                }
            }


            return true;
        }

        public bool InitConstructorWaitForCompletion(Dictionary<string, string> inputValues = null)
        {
            InitConstructor(inputValues);
            WaitTillAccountActive();
            return true;
        }

        public bool ExecuteBoc(byte[] bocMessage, params string[] quantchainIds)
        {
            foreach (var quantchainId in quantchainIds)
            {
                var execNode = new NodeDbSvc().GetRandomExecutionNodeByTonNeworkId(quantchainId);
                var SSHCommandHlp = new SSHCommandHlp(execNode.Id);
                var tempFilepath = $"{GlobalVarHandler.ROOT_DIR}/{Guid.NewGuid()}.boc";
                SSHCommandHlp.UploadFileToHost(bocMessage, tempFilepath);
                var rs = new TONClientSvc().SendFile(tempFilepath, execNode.Id);
                SSHCommandHlp.DeleteFile(tempFilepath);
            }

            return true;
        }

        public bool ExecuteFunc(string funcName, Dictionary<string, string> inputValues = null)
        {

            foreach (var sa in _qunatchainStateDic)
            {
                var funcBocMessage = GenerateBoc(sa.Key.Id, funcName, inputValues);

                foreach (var quantchain in sa.Value)
                {
                    ExecuteBoc(funcBocMessage, quantchain.Key.Id);
                }
            }


            return true;
        }

        public virtual BaseSAI InitSmartAccount(string customId, string skId, string scId, IEnumerable<string> qunatchainIds)
        {
            var rs = new SmartAccountSvc().InitSmartAccount(scId, 0, skId, customId, qunatchainIds.ToArray());

            if (rs.Any())
            {
               return Init(rs.Values.Select(q => q.Id), qunatchainIds.ToArray());
            }

            return null;
        }

        public virtual SmartAccount InitSmartAccountAndDeploy(string customId, string scId, IEnumerable<string> qunatchainIds, long initamount = 1)
        {
            var r = InitSmartAccount(customId, null, scId, qunatchainIds);
            r.DeployAccount(initamount);
            return r._meta_smartAccounts.First();
        }

        public virtual  SmartAccount InitSmartAccountAndDeploy(string customId, string scId, IEnumerable<string> qunatchainIds, string mnemonicPhrase, long initamount = 1)
        {
            var sk = new SmartAccountSvc().CreateSmartKey(mnemonicPhrase);
            var r = InitSmartAccount(customId, sk.Id, scId, qunatchainIds);
            r.DeployAccount(initamount);
            return r._meta_smartAccounts.First();
        }
      
        public virtual SmartAccount InitSmartAccountAndDeploy(string customId, string scId, IEnumerable<string> qunatchainIds, string secretKey, string publicKey, long initamount = 1)
        {
            var sk = new SmartAccountSvc().CreateSmartKey(secretKey, publicKey);
            var r = InitSmartAccount(customId, sk.Id, scId, qunatchainIds);
            r.DeployAccount(initamount);
            return r._meta_smartAccounts.First();
        }


        public Dictionary<TonNetwork, SmartAccountMdl> GetCurrentStateBySaId(string saId)
        {    
            return _qunatchainStateDic.First(q => q.Key.Id == saId).Value;
        }

        public Dictionary<TonNetwork, SmartAccountMdl> GetCurrentStateBySaIdAndQuantChainId(string saId, params string[] quantChainIds)
        {
            return GetCurrentStateBySaId(saId).Where(q=> quantChainIds.Contains(q.Key.Id)).ToDictionary(q=>q.Key, q=>q.Value);
        }


        //public Dictionary<string, SmartAccountMdl> GetLatestStateInfo()
        //{
        //    UpdateState();
        //    return GetBaseAccountInfo(saId, tonNetworkIds);
        //}

        //public Dictionary<string, SmartAccountMdl> GetStateInfo()
        //{
        //    var rs = new Dictionary<string, SmartAccountMdl>();
        //    var sa = _meta_smartAccounts.First();
        //    foreach (var tonNetworkId in _qunatchainStateDic)
        //    {
        //        var r = new SmartAccountMdl()
        //        {
        //            Id = sa.Id,
        //            Phrase = sa.SmartAccountKeys.First().SmartKey.MnemonicPhrase,
        //            PublicKey = sa.SmartAccountKeys.First().SmartKey.PublicKey,
        //            SecretKey = sa.SmartAccountKeys.First().SmartKey.SecretKey,
        //            TonSafeKey = sa.SmartAccountKeys.First().SmartKey.TonSafePublicKey,
        //            SmartContractName = sa.SmartContract.Name,
        //            SmartContractVersion = sa.SmartContract.Version,
        //            Address = sa.Address
        //        };

        //        var sa_net = sa.SmartAccountNetworks.First(q => q.NetworkId == tonNetworkId);
        //        r.Balance = sa_net.Balance;
        //        r.NetworkName = sa_net.TonNetwork.Name;
        //        r.Status = sa_net.Status;

        //        rs.Add(tonNetworkId, r);
        //    }

        //    return rs;
        //}


        //public virtual BaseSAI InitSmartAccountBySCName(string customId, string scName, IEnumerable<string> qunatchainIds)
        //{
        //    var sc = new SmartContractDbSvc().GetByName(scName);
        //    return InitSmartAccount(customId, sc.Id, qunatchainIds);
        //}


        //public virtual bool InitSmartAccountBySCNameAndDeploy(string customId, string scName, IEnumerable<string> qunatchainIds, long initamount = 1)
        //{
        //    var sc = new SmartContractDbSvc().GetByName(scName);
        //    return InitSmartAccountAndDeploy(customId, sc.Id, qunatchainIds, initamount);
        //}

    }
}
