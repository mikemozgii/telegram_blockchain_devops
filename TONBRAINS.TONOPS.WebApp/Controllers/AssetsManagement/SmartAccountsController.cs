using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.TeamFoundation.Common;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Constants;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.Core.SmartAccountInterfaces;
using TONBRAINS.TONOPS.WebApp.Models.Common;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers.AssetsManagement
{
    [Route("api/smartaccounts")]
    public class SmartAccountsController
    {
        private readonly TonOpsDbContext _context;
        private readonly ISchedulerFactory schedulerFactory;

        public SmartAccountsController(TonOpsDbContext context, ISchedulerFactory schedulerFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.schedulerFactory = schedulerFactory;
        }

        [HttpGet("data")]
        public async Task<object> GetData()
        {
            var types = await _context.SmartTypes.ToListAsync();
            var keys = await _context.SmartKeys.Where(q => !q.IsDeleted).ToListAsync();
            var contracts = await _context.SmartContracts.Where(q => !q.IsDeleted).ToListAsync();
            var contractKeys = await _context.SmartAccountKeys.Where(q => !q.IsDeleted).ToListAsync();
            var tonNetworks = await _context.TonNetworks.Where(i => !i.IsDeleted).ToListAsync();
            var networkSmartKeys = await _context.ViewNetworkSmartKeys.ToListAsync();

            return new
            {
                Types = types.Select(i => new { i.Id, Title = i.Name }),
                KeyOptions = keys.Select(i => new { i.Id, Title = i.Name }),
                ContractOptions = contracts.Select(i => new { i.Id, Title = i.Name }),
                ContractKeys = contractKeys.GroupBy(i => i.SmartContractId).ToDictionary(i => i.Key, i => i.ToList()),
                NetworkOptions = tonNetworks.Select(i => new { i.Id, Title = i.Name, i.Active, i.DateStarted }),
                NetworkSmartKeys = networkSmartKeys.ToDictionary(i => i.NetworkId, i => i.SmartKeys)
            };
        }

        [HttpGet("grid")]
        public async Task<List<SmartAccountView>> SmartAccounts(string id, string mode)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(mode))
            {
                return await _context.ViewSmartAccounts.ToListAsync();
            }

            if (mode == "smartcontracts")
            {
                return await _context.ViewSmartAccounts.Where(i => i.ContractId == id).ToListAsync();
            }

            if (mode == "smartkeys")
            {
                var accountIds = (await _context.SmartAccountKeys.Where(i => i.SmartKeyId == id && !i.IsDeleted).ToListAsync())
                    .Select(i => i.SmartAccountId);

                return await _context.ViewSmartAccounts.Where(i => accountIds.Contains(i.Id)).ToListAsync();
            }

            if (mode == "network")
            {
                return (await _context.ViewSmartAccounts.ToListAsync()).Where(i => i.NetworkIds.Contains(id)).ToList();
            }

            return new List<SmartAccountView>();
        }

        [HttpGet("getaccount")]
        public async Task<SmartAccountView> GetAccount(string id) => await _context.ViewSmartAccounts.FirstOrDefaultAsync(q => q.Id == id);

        [HttpGet("single")]
        public async Task<SmartAccount> SmartAccount(string id)
        {
            var item = await _context.SmartAccounts.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);
            var networkIds = await _context.SmartAccountNetworks.Where(q => q.SmartAccountId == id).ToListAsync();
            item.NetworkIds = networkIds.Select(i => i.NetworkId).ToList();

            return item;
        }

        
        [HttpGet("refreshstates")]
        public async Task RefreshStates(string networkId, string smartAccountId)
        {
            
            if (string.IsNullOrEmpty(networkId) && string.IsNullOrEmpty(smartAccountId))
            {
                var ids = await _context.SmartAccountNetworks.Where(i => !i.IsDeleted && i.IsDeployed && !string.IsNullOrEmpty(i.NetworkId) && i.StatusId == "active").Select(s => s.SmartAccountId).AsNoTracking().ToArrayAsync();
                new BaseSAI().Init(ids).UpdateState();
            }
            if (!string.IsNullOrEmpty(networkId) && !string.IsNullOrEmpty(smartAccountId))
            {
                new BaseSAI().Init(smartAccountId, networkId).UpdateState();
                return;
            }
            if (string.IsNullOrEmpty(networkId) && !string.IsNullOrEmpty(smartAccountId))
            {
                //var ids = await _context.SmartAccountNetworks.Where(i => !i.IsDeleted && i.IsDeployed && !string.IsNullOrEmpty(i.NetworkId) && i.StatusId == "active").Select(s => s.NetworkId).AsNoTracking().ToArrayAsync();
                //svc.UpdateAccountState(smartAccountId, null);
                new BaseSAI().Init(smartAccountId).UpdateState();
                return;
            }
        }

        [HttpGet("networkstatelogs")]
        public async Task<IEnumerable<SmartAccountStateLog>> GetNetworkStateLogs(string networkid, string smaid)
        {
            return await _context.SmartAccountStateLogs
                .Where(i => i.NetworkId == networkid && i.SmartAccountId == smaid)
                .OrderByDescending(o => o.Date)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpPost("deploysmartcontracts")]
        public async Task<bool> DeploySmartContracts([FromBody] DeploySmartContractsBundle model)
        {
            return await Task.FromResult(true);
        }

        [HttpPost("add")]
        public async Task<SmartAccountView> AddSmartAccount([FromBody] SmartAccount item)
        {
            item.Id = IdGenerator.Generate();
            item.CreationDate = DateTime.UtcNow;

            var contract = await _context.SmartContracts.FirstOrDefaultAsync(q => q.Id == item.SmartContractId);
            var keys = await _context.SmartKeys.Where(q => item.SmartKeyIds.Contains(q.Id)).ToListAsync();

            var fileSvc = new FileSvc();
            var abiFile = await fileSvc.GetFileAsync(contract.AbiFileId);
            var tvcFile = await fileSvc.GetFileAsync(contract.TvcFileId);

            var firstKey = keys.FirstOrDefault();
            item.Address = new SmartAccountSvc().GenerateSmartAccountAddress(abiFile, tvcFile, firstKey?.PublicKey, firstKey?.SecretKey, item.Wc);
            if (string.IsNullOrEmpty(item.Address))
            {
                item.Address = "";
                return null;
            }
                
                

            await _context.SmartAccounts.AddAsync(item);
            await _context.SaveChangesAsync();

            var smartAccountKeys = keys.Select(key => new SmartAccountKey
            {
                Id = IdGenerator.Generate(),
                SmartAccountId = item.Id,
                SmartContractId = item.SmartContractId,
                SmartKeyId = key.Id,
            }).ToList();

            await _context.SmartAccountKeys.AddRangeAsync(smartAccountKeys);

            //var networks = item.NetworkIds.Select(networkId => new SmartAccountNetwork
            //{
            //    Id = IdGenerator.Generate(),
            //    SmartAccountId = item.Id,
            //    NetworkId = networkId,
            //    StatusId = "undefined",
            //    Balance = 0,
            //}).ToList();

            //await _context.SmartAccountNetworks.AddRangeAsync(networks);

            await _context.SaveChangesAsync();

            return await GetAccount(item.Id);
        }

        [HttpPost("edit")]
        public async Task<SmartAccountView> UpdateSmartAccount([FromBody] SmartAccount item)
        {
            _context.SmartAccounts.Update(item);

            var existsNetworks = await _context.SmartAccountNetworks.Where(i => i.SmartAccountId == item.Id).ToListAsync();
            foreach (var network in existsNetworks)
            {
                if (!item.NetworkIds.Contains(network.NetworkId)) network.IsDeleted = true;
                if (item.NetworkIds.Contains(network.NetworkId)) network.IsDeleted = false;
            }
            _context.SmartAccountNetworks.UpdateRange(existsNetworks);

            var existsNetworkIds = existsNetworks.Select(i => i.NetworkId);
            var networks = item.NetworkIds
                .Where(networkId => !existsNetworkIds.Contains(networkId))
                .Select(networkId => new SmartAccountNetwork
                {
                    Id = IdGenerator.Generate(),
                    SmartAccountId = item.Id,
                    NetworkId = networkId,
                    StatusId = "undefined",
                    Balance = 0,
                }).ToList();

            await _context.SmartAccountNetworks.AddRangeAsync(networks);
            await _context.SaveChangesAsync();

            return await GetAccount(item.Id);
        }

        [HttpPost("transfer")]
        public async Task<List<SmartAccountView>> Transfer([FromBody] TransferBalanceMdl model)
        { 
            var scheduler = await schedulerFactory.GetScheduler();
    
            if (!string.IsNullOrEmpty(model.Id) && !string.IsNullOrEmpty(model.Mode) && !model.Balances.IsNullOrEmpty())
            {
                var SmartAccountSvc = new SmartAccountSvc(scheduler);
                if (model.Mode == "from")
                {
                    var to_smas = model.Balances.Where(q => q.Key != model.Id).ToDictionary(q => q.Key, q => q.Value);
                    new BaseSAI().Init(model.Id, model.NetworkIds).Transfer(to_smas , model.Bounce);
                }
                else if (model.Mode == "to")
                {
                    var from_smas = model.Balances.Where(q => q.Key != model.Id).ToDictionary(q => q.Key, q => q.Value);
                    foreach (var from_sma in from_smas)
                    {
                        //SmartAccountSvc.ExecuteTransfer(from_sma.Key, model.Id, from_sma.Value, model.Bounce, model.NetworkIds);
                        new BaseSAI().Init(from_sma.Key, model.NetworkIds).Transfer(model.Id, from_sma.Value, model.Bounce);
                    }
                }
            }

            return await SmartAccounts("", null);
        }

        [HttpDelete("soft-delete")]
        public async Task<bool> SoftDeleteSmartAccount(string id)
        {
            var rows = await _context.SmartAccounts.Where(i => i.Id == id).ToListAsync();
            rows.ForEach(q => q.IsDeleted = true);

            _context.SmartAccounts.UpdateRange(rows);
            await _context.SaveChangesAsync();

            return rows.Any();
        }

        [HttpGet("keys")]
        public async Task<List<SmartAccountKeyView>> GetKeys(string id)
        {
            return await _context.ViewSmartAccountKeys.Where(i => i.SmartAccountId == id).ToListAsync();
        }

        [HttpPost("generatekeys")]
        public async Task<SmartAccountView> GenerateKeys([FromBody] GenerateKeysMdl model)
        {
            if (model.LoadSmartContract)
            {
                model.SmartContractId = IdGenerator.Generate();
                var contract = new SmartContract
                {
                    Id = model.SmartContractId,
                    LibId = SmartContractLibIds.Default,
                    Name = model.Name + "SmartContrant",
                    TypeId = model.TypeId,
                    AbiFileId = model.AbiFileId,
                    TvcFileId = model.TvcFileId,
                    SolFileId = model.SolFileId,
                };

                var fileSvc = new FileSvc();

                var abiFile = await fileSvc.GetFileAsync(contract.AbiFileId);
                contract.AbiJson = Encoding.UTF8.GetString(abiFile);

                if (!string.IsNullOrEmpty(contract.SolFileId))
                {
                    var solFile = await fileSvc.GetFileAsync(contract.SolFileId);
                    contract.SolJson = Encoding.UTF8.GetString(solFile);
                }

                await _context.SmartContracts.AddAsync(contract);
            }

            var account = new SmartAccount
            {
                Id = IdGenerator.Generate(),
                Name = model.Name,
                TypeId = model.TypeId,
                Wc = model.Wc,
                SmartContractId = model.SmartContractId,
                NetworkIds = new List<string>(),
                CreationDate = DateTime.UtcNow
            };

            var cryptoSvc = new CryptoSvc();
            var smartKeys = new List<SmartKey>();
            for (var i = 1; i <= model.KeysCount; i++)
            {
                var smartKey = new SmartKey
                {
                    Id = IdGenerator.Generate(),
                    Name = model.Name + "Key" + (model.KeysCount > 1 ? i : ""),
                    TypeId = model.TypeId,
                    MnemonicPhrase = cryptoSvc.GetMnemonicPhrase()
                };

                var keyPair = cryptoSvc.GetKeyPair(smartKey.MnemonicPhrase);
                smartKey.PublicKey = keyPair?.PublicKey;
                smartKey.SecretKey = keyPair?.SecretKey;
                smartKey.TonSafePublicKey = keyPair?.TonSafePublicKey;

                smartKeys.Add(smartKey);
            }
            await _context.SmartKeys.AddRangeAsync(smartKeys);
            await _context.SaveChangesAsync();

            account.SmartKeyIds = smartKeys.Select(i => i.Id).ToList();
            return await AddSmartAccount(account);
        }

        [HttpGet("networks")]
        public async Task<List<SmartAccountNetworkView>> GetNetworks(string id) => await _context.ViewSmartAccountNetworks.Where(i => i.SmartAccountId == id).ToListAsync();

        [HttpGet("networklogs")]
        public async Task<List<SmartAccountBalanceTransferLogView>> GetNetworkLogs(string id)
        {
            var rows = await _context.ViewSmartAccountBalanceTransferLogs
                .Where(i => i.SmartAccountFromNetworkId == id || i.SmartAccountToNetworkId == id)
                .ToListAsync();

            rows.ForEach(i =>
            {
                var isCredit = i.SmartAccountFromNetworkId == id;
                i.Type = isCredit ? "Credit" : "Debit";
                i.Total = isCredit ? i.SmartAccountFromBalance - i.TransferBalance : i.SmartAccountToBalance + i.TransferBalance;
            });

            return rows.OrderByDescending(i => i.TransferTime).ToList();
        }

        [HttpGet("getconstructor")]
        public async Task<List<ParsedInputMdl>> GetConstructor(string contractId)
        {
            var functions = await GetParsedAbiFile(contractId);

            return functions?.FirstOrDefault(i => i.Name == "constructor")?.Inputs;
        }

        [HttpGet("getparsedabifile")]
        public async Task<List<ParsedFunctionMdl>> GetParsedAbiFile(string contractId)
        {
            try
            {
                var contract = await _context.SmartContracts.FirstOrDefaultAsync(q => q.Id == contractId);
                return JsonConvert.DeserializeObject<ParsedAbiFileMdl>(contract.AbiJson).Functions;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        [HttpPost("deploynetworks")]
        public async Task<SmartAccountView> DeployNetworks([FromBody] List<string> networkIds, string id, long amount)
        {

           // new SmartAccountSvc().DeployAccount(id, amount, networkIds.ToArray());

            new BaseSAI().Init(id, networkIds.ToArray()).DeployAccount(amount);

            //var networks = await _context.SmartAccountNetworks
            //    .Where(i => i.SmartAccountId == id && networkIds.Contains(i.NetworkId))
            //    .ToListAsync();


            //networks.ForEach(i => { i.IsDeployed = true; i.Balance = amount; });
            //_context.SmartAccountNetworks.UpdateRange(networks);
            //await _context.SaveChangesAsync();

            return await GetAccount(id);
        }

        [HttpGet("deploynetwork")]
        public async Task<SmartAccountNetworkView> DeployNetwork(string id)
        {
            var network = await _context.SmartAccountNetworks.FirstOrDefaultAsync(i => i.Id == id);
            if (network == null) return null;

            network.IsDeployed = true;
            _context.SmartAccountNetworks.Update(network);
            await _context.SaveChangesAsync();

            return await _context.ViewSmartAccountNetworks.FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
