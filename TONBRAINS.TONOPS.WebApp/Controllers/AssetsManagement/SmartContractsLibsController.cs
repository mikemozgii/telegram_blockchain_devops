using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Constants;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers.AssetsManagement
{
    [Route("api/smartcontractslibs")]
    public class SmartContractsLibsController
    {
        private readonly TonOpsDbContext _context;

        public SmartContractsLibsController(TonOpsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("grid")]
        public async Task<List<SmartContractLibView>> Grid() => await _context.ViewSmartContractsLibs.ToListAsync();

        [HttpGet("single")]
        public async Task<SmartContractLib> Single(string id) => await _context.SmartContractsLibs.FirstOrDefaultAsync(q => q.Id == id);

        [HttpPost("add")]
        public async Task<SmartContractLibView> Add([FromBody] SmartContractLib item)
        {
            item.Id = IdGenerator.Generate();

            await _context.SmartContractsLibs.AddAsync(item);
            await _context.SaveChangesAsync();

            return await _context.ViewSmartContractsLibs.FirstOrDefaultAsync(q => q.Id == item.Id);
        }

        [HttpPost("edit")]
        public async Task<SmartContractLibView> Edit([FromBody] SmartContractLib item)
        {
            _context.SmartContractsLibs.Update(item);
            await _context.SaveChangesAsync();
            return await _context.ViewSmartContractsLibs.FirstOrDefaultAsync(q => q.Id == item.Id);
        }

        [HttpDelete("delete")]
        public async Task<bool> Delete(string id)
        {
            var rows = await _context.SmartContractsLibs.Where(i => i.Id == id).ToListAsync();

            var contracts = await _context.SmartContracts.Where(i => i.LibId == id).ToListAsync();
            contracts.ForEach(i => i.LibId = SmartContractLibIds.Default);

            _context.SmartContracts.UpdateRange(contracts);
            _context.SmartContractsLibs.RemoveRange(rows);
            await _context.SaveChangesAsync();

            return rows.Any();
        }
    }
}
