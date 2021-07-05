using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class SmartAccountDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public SmartAccountDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(SmartAccount e)
        {
             _context.SmartAccounts.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<SmartAccount> GetByIds(IEnumerable<string> ids)
        {
            return _context.SmartAccounts.Where(q => ids.Contains(q.Id)).ToList();
        }

        public IEnumerable<SmartAccount> GetByIdsWithInclude(IEnumerable<string> ids)
        {
            return _context.SmartAccounts
                .Include(q=>q.SmartAccountNetworks).ThenInclude(q => q.TonNetwork)
                .Include(q => q.SmartAccountKeys).ThenInclude(q => q.SmartKey)
                .Include(q => q.SmartContract)
                .Where(q => ids.Contains(q.Id)).ToList();
        }

        public SmartAccount GetById(string id)
        {
            return GetByIds(new string[]{ id }).FirstOrDefault();
        }

        public bool Update(params SmartAccount[] entities)
        {
            _context.SmartAccounts.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<SmartAccountKeyView> GetAccountSmartKeys(params string[] ids)
        {
            return _context.ViewSmartAccountKeys.Where(q => ids.Contains(q.SmartAccountId)).ToList();
        }

        public IEnumerable<SmartAccountKeyView> GetAccounts_ByMnemonicphrase(string mnemonicPhrase)
        {
            return _context.ViewSmartAccountKeys.Where(q => q.MnemonicPhrase.ToLower() == mnemonicPhrase.ToLower()).ToList();
        }

        public SmartAccountKeyView GetAccounts_BySecretAndPublicKey(string secretKey, string publicKey)
        {
            return _context.ViewSmartAccountKeys.FirstOrDefault(q => q.SecretKey.ToLower() == secretKey.ToLower() && q.PublicKey.ToLower() == publicKey.ToLower());
        }

        public IEnumerable<SmartAccount> GetByNodeIds(params string[] ids)
        {
            return _context.SmartAccounts.Where(q => ids.Contains(q.NodeId)).ToList();
        }

        public IEnumerable<SmartAccount> GetByNodeIdsORTonNetwork(string[] nodeIds, string tonNetworkId)
        {
            return _context.SmartAccounts.Where(q => (nodeIds.Contains(q.NodeId) || q.TonNetworkId == tonNetworkId) && !q.IsDeleted).ToList();
        }

        public SmartAccount GetMainWalletAccountByNetwork(string tonNetworkId)
        {
            return _context.SmartAccounts.First(q =>  q.SmartAccountNetworks.Any(q1=>q1.NetworkId == tonNetworkId) && !q.IsDeleted && q.Address == GlobalAppConfHandler.MainWalletAddress);
        }

        public IEnumerable<SmartAccount> GetMainWalletAccountByNetworks(params string[] qunatchainIds)
        {
            return _context.SmartAccounts.Where(q => q.SmartAccountNetworks.Any(q1 => qunatchainIds.Contains(q1.NetworkId)) && !q.IsDeleted && q.Address == GlobalAppConfHandler.MainWalletAddress);
        }

        public bool Delete(params SmartAccount[] entities)
        {
            _context.SmartAccounts.RemoveRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteByIds(params string[] ids)
        {
            var es = GetByIds(ids);
            _context.SmartAccounts.RemoveRange(es);
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> ExecuteTransfer(string smartAccountId, string recipientSmartAccountId, long balance)
        {
            try
            {
                var account = await _context.SmartAccountNetworks.FirstOrDefaultAsync(i => i.SmartAccountId == smartAccountId);
                var recipientAccount = await _context.SmartAccountNetworks.FirstOrDefaultAsync(i => i.SmartAccountId == recipientSmartAccountId);
                if (account == null || recipientAccount == null) return false;

                account.Balance -= balance;
                if (account.Balance < 0) return false;

                recipientAccount.Balance += balance;

                _context.Update(account);
                _context.Update(recipientAccount);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> ExecutePayment(string smartAccountId, long balance)
        {
            try
            {
                var account = await _context.SmartAccountNetworks.FirstOrDefaultAsync(i => i.SmartAccountId == smartAccountId);
                if (account == null) return false;

                account.Balance += balance;

                _context.Update(account);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        //public async Task<string> CreateDefaultSmartAccount(string name, string typeId)
        //{
        //    var contract = await _context.SmartContracts.FirstOrDefaultAsync(i => i.Id == "-MPkxybtPgntLlrgoDpR");
        //    var network = await _context.TonNetworks.FirstOrDefaultAsync(i => i.Id == "-MPkwjrUkN6fACc8Ra2v");
        //    if (contract == null || network == null) return "";

        //    var item = new SmartAccount
        //    {
        //        Id = IdGenerator.Generate(),
        //        Name = name,
        //        Wc = 0,
        //        TypeId = typeId,
        //        SmartContractId = contract.Id,
        //        CreationDate = DateTime.UtcNow
        //    };

        //    var cryptoSvc = new CryptoSvc();
        //    var key = new SmartKey
        //    {
        //        Id = IdGenerator.Generate(),
        //        Name = item.Name + "Key",
        //        TypeId = item.TypeId,
        //        MnemonicPhrase = cryptoSvc.GetMnemonicPhrase()
        //    };

        //    var keyPair = cryptoSvc.GetKeyPair(key.MnemonicPhrase);
        //    key.PublicKey = keyPair?.PublicKey;
        //    key.SecretKey = keyPair?.SecretKey;
        //    key.TonSafePublicKey = keyPair?.TonSafePublicKey;

        //    var fileSvc = new FileSvc();
        //    var abiFile = await fileSvc.GetFile(contract.AbiFileId);
        //    var tvcFile = await fileSvc.GetFile(contract.TvcFileId);

        //    item.Address = new SmartAccountSvc().GenerateSmartAccountAddress(abiFile, tvcFile, key.PublicKey, key.SecretKey, item.Wc);
        //    if (string.IsNullOrEmpty(item.Address)) item.Address = "";

        //    var smartAccountKey = new SmartAccountKey
        //    {
        //        Id = IdGenerator.Generate(),
        //        SmartAccountId = item.Id,
        //        SmartContractId = item.SmartContractId,
        //        SmartKeyId = key.Id,
        //    };

        //    var smartAccountNetwork = new SmartAccountNetwork
        //    {
        //        Id = IdGenerator.Generate(),
        //        SmartAccountId = item.Id,
        //        NetworkId = network.Id,
        //        StatusId = "active",
        //        Balance = 100,
        //        IsDeployed = true,
        //        Status = "active"
        //    };

        //    await _context.SmartKeys.AddAsync(key);
        //    await _context.SmartAccounts.AddAsync(item);
        //    await _context.SmartAccountKeys.AddAsync(smartAccountKey);
        //    await _context.SmartAccountNetworks.AddAsync(smartAccountNetwork);

        //    await _context.SaveChangesAsync();

        //    return item.Id;
        //}

        public async Task<SmartAccountWithFirstKeyMdl> GetSmartAccountWithFirstKey(string id)
        {
            var smartAccount = await _context.SmartAccounts.FirstOrDefaultAsync(i => i.Id == id);
            var smartAccountKey = await _context.ViewSmartAccountKeys.FirstOrDefaultAsync(i => i.SmartAccountId == id);

            return new SmartAccountWithFirstKeyMdl
            {
                Address = smartAccount.Address,
                MnemonicPhrase = smartAccountKey.MnemonicPhrase,
                PublicKey = smartAccountKey.PublicKey,
                SecretKey = smartAccountKey.SecretKey,
            };
        }

        public async Task<long> GetAccountNetworkBalance(string smartAccountId)
        {
            var smartAccountNetwork = await _context.ViewSmartAccountNetworks.FirstOrDefaultAsync(i => i.SmartAccountId == smartAccountId);

            return smartAccountNetwork?.Balance ?? 0;
        }

        //public async Task<long> GetMainWalletByNetwork(string tonNetworkId)
        //{
        //    var smartAccountNetwork = await _context.ViewSmartAccountNetworks.FirstOrDefaultAsync(i => i.SmartAccountId == smartAccountId);



        //    return smartAccountNetwork?.Balance ?? 0;
        //}
    }
}
