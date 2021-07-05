using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using TONBRAINS.TONOPS.Core.DAL;
using Microsoft.EntityFrameworkCore;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.WebApp.Models.Ton;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.Services;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/tonconfigurations")]
    [ApiController]
    public class TonConfigurationsController : SessionController
    {
        private readonly TonOpsDbContext _context;

        public TonConfigurationsController(TonOpsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("grid")]
        public async Task<IEnumerable<TonConfiguration>> GridData()
        {
            return await _context.TonConfigurations.Where(i => !i.IsDeleted).ToListAsync();
        }

        [HttpGet("single")]
        public async Task<TonConfiguration> Single(string id)
        {
            return await _context.TonConfigurations.FirstOrDefaultAsync(i => i.Id == id);
        }

        [HttpGet("nodes")]
        public async Task<IEnumerable<v_NodeMetric>> Nodes(string id)
        {
            var nodesIds = await _context.ViewConfigurationNetworkNodes.Where(i => i.ConfigurationId == id).Select(s => s.Id).ToListAsync();
            if (!nodesIds.Any()) return Enumerable.Empty<v_NodeMetric>();

            return await new NodeSvc(_context).GetLatesNodesMetrics(nodesIds);

        }

        [HttpGet("nodelog")]
        public async Task<IEnumerable<string>> NodeLog(string id)
        {
            var logs = new SSHCommandHlp(new List<string> { id }).ExecuteCommandsWithResultParallel($"tail -100 {GlobalVarHandler.TON_WORK_NODE_LOG}").ToList();
            return logs.First().Value.FirstOrDefault()?.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        }

        [HttpGet("zerostate")]
        public TonConfiguration ZeroState()
        {
            return new TonConfiguration();
        }

        [HttpPost("add")]
        public async Task<TonConfiguration> AddConfiguration([FromBody] TonConfiguration item)
        {
            item.Id = IdGenerator.Generate();
            await _context.TonConfigurations.AddRangeAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpPost("edit")]
        public async Task<TonConfiguration> UpdateConfiguration([FromBody] TonConfiguration item)
        {
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpDelete("softdelete")]
        public async Task<bool> SoftDeleteConfiguration(string id)
        {
            var item = await _context.TonConfigurations.FirstOrDefaultAsync(i => i.Id == id);
            item.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

    }

}
