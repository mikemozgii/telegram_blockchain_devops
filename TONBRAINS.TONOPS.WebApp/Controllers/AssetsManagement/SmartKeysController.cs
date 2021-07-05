using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers.AssetsManagement
{
    [Route("api/smartkeys")]
    public class SmartKeysController
    {
        private CryptoSvc CryptoSvc;
        private readonly TonOpsDbContext _context;

        public SmartKeysController(TonOpsDbContext context)
        {
            CryptoSvc = new CryptoSvc();
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("data")]
        public async Task<object> GetData()
        {
            var gridData = await _context.ViewSmartKeys.ToListAsync();
            var types = await _context.SmartTypes.ToListAsync();

            return new
            {
                GridData = gridData,
                Types = types.Select(i => new { i.Id, Title = i.Name })
            };
        }

        [HttpGet("single")]
        public async Task<SmartKey> SmartKey(string id) => await _context.SmartKeys.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);

        [HttpPost("add")]
        public async Task<SmartKey> AddSmartKey([FromBody] SmartKey item)
        {
            item.Id = IdGenerator.Generate();
            await _context.SmartKeys.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpPost("edit")]
        public async Task<SmartKey> UpdateSmartKey([FromBody] SmartKey item)
        {
            _context.SmartKeys.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpDelete("soft-delete")]
        public async Task<bool> SoftDeleteSmartKey(string id)
        {
            var rows = await _context.SmartKeys.Where(i => i.Id == id).ToListAsync();
            rows.ForEach(q => q.IsDeleted = true);

            _context.SmartKeys.UpdateRange(rows);
            await _context.SaveChangesAsync();

            return rows.Any();
        }

        [HttpPost("generatephrase")]
        public async Task<SmartKey> GeneratePhrase([FromBody] SmartKey item)
        {
            item.MnemonicPhrase = CryptoSvc.GetMnemonicPhrase();

            return await GenerateKeys(item);
        }

        [HttpPost("generatekeys")]
        public async Task<SmartKey> GenerateKeys([FromBody] SmartKey item)
        {
            var keyPair = CryptoSvc.GetKeyPair(item.MnemonicPhrase);

            item.PublicKey = keyPair?.PublicKey;
            item.SecretKey = keyPair?.SecretKey;
            item.TonSafePublicKey = keyPair?.TonSafePublicKey;

            return await Task.FromResult(item);
        }
    }
}
