using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using TONBRAINS.TONOPS.Core.DAL;
using Microsoft.EntityFrameworkCore;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.Models.Ton;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.WebApp.Services;
using TONBRAINS.TONOPS.Core.Models;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/tonnetworks")]
    [ApiController]
    public class TonNetworksController : SessionController
    {
        private readonly TonOpsDbContext _context;

        public TonNetworksController(TonOpsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("data")]
        public async Task<object> GetData()
        {
            var networkIds = new List<string>();

            var contractTypes = new List<string>
            {
                "system",
                "elector",
                "validator",
            };
            return new
            {
                data = await _context.ViewTonNetworksGrid.ToListAsync(),
                nodes = await _context.Nodes.Where(i => i.Type == NodeTypes.QUANTONReady && !i.IsDeleted && i.TonNetworkId != i.Id).Select(s => new { s.Id, Title = $"{s.Name} [{s.Ip}]", s.GroupId }).ToListAsync(),
                configurations = await _context.TonConfigurations.Where(i => !i.IsDeleted).Select(s => new { s.Id, Title = s.Name }).ToListAsync(),
                SmartContracts = await _context.SmartContracts.Where(i => contractTypes.Contains(i.TypeId) && !i.IsDeleted ).Select(s => new { s.Id, Title = s.Name, Type = s.TypeId }).ToListAsync(),
            };
        }

        [HttpGet("single")]
        public async Task<object> Single(string id)
        {
            return new
            {
                network = await _context.TonNetworks.FirstOrDefaultAsync(i => i.Id == id),
                nodesIds = await _context.Nodes.Where(i => i.TonNetworkId == id).Select(i => i.Id).ToListAsync()
            };
        }

        [HttpGet("nodesrefreshmetrics")]
        public async Task<IEnumerable<v_NodeMetric>> NodesRefresh(string id)
        {
            var nodeIds = new NodeDbSvc().GetByTonNeworkId(id).Select(s => s.Id).ToList();
            return await new NodeSvc(_context).NodesRefreshMetrics(nodeIds);
        }

        [HttpGet("nodes")]
        public async Task<IEnumerable<v_NodeMetric>> Nodes(string id)
        {
            var nodesIds = await _context.ViewConfigurationNetworkNodes.Where(i => i.NetworkId == id).Select(s => s.Id).ToListAsync();

            return await new NodeSvc(_context).GetLatesNodesMetrics(nodesIds);
        }

        [HttpGet("zabbixserver")]
        public async Task<string> GetZabbixServer(string id)
        {
            var zabixServerId = await _context.ViewConfigurationNetworkNodes.Where(i => i.NetworkId == id && !string.IsNullOrEmpty(i.ZabbixServerId)).Select(s => s.ZabbixServerId).FirstOrDefaultAsync();
            if (zabixServerId != null)
            {
                return (await _context.Nodes.FirstOrDefaultAsync(i => i.Id == zabixServerId))?.Ip;
            }
            else return "";
        }

        [HttpPost("add")]
        public async Task<TonNetwork> AddConfiguration([FromBody] TonNetworkConfigurationModel model)
        {
            var network = model.Network;
            network.Id = IdGenerator.Generate();
            await _context.TonNetworks.AddAsync(network);
            var nodes = await _context.Nodes.Where(i => model.NodesIds.Contains(i.Id)).ToListAsync();

            foreach (var node in nodes)
            {
                node.TonNetworkId = network.Id;
            }

            await _context.SaveChangesAsync();
            return network;
        }

        [HttpGet("startton")]
        public Task<bool> StartTon(string id)
        {
            var r = new TONNodeSetupSvc().StartTonNetowrk(id);
            return Task.FromResult(r);
        }

        [HttpPost("parseconfig")]
        public TonConfigActulMdl ParseConfig(ParseConfigModel config)
        {
            return new TonNetworkSvc().ParseActualTonConfig(config.Data);
        }

        [HttpGet("runnetwork")]
        public async Task<bool> RunNetwork(string id)
        {
            var nodesIds = await _context.ViewConfigurationNetworkNodes.Where(i => i.NetworkId == id).Select(s => s.Id).ToListAsync();
            var result = new TONNodeSetupSvc().RunValidatorEngine(nodesIds);
            await new NodeSvc(_context).NodesRefreshMetrics(nodesIds);
            return true;
        }

        [HttpGet("stopnetwork")]
        public async Task<bool> StopNetwork(string id)
        {
            var nodesIds = await _context.ViewConfigurationNetworkNodes.Where(i => i.NetworkId == id).Select(s => s.Id).ToListAsync();
            var result = new TONNodeSetupSvc().StopValidatorEngine(nodesIds);
            await new NodeSvc(_context).NodesRefreshMetrics(nodesIds);
            return true;
        }

        [HttpGet("runvalidatorengine")]
        public async Task<bool> RunValidatorEngine(string nodeId)
        {
            var result =  new TONNodeSetupSvc().RunValidatorEngine(new List<string> { nodeId });
            await new NodeSvc(_context).NodesRefreshMetrics(new List<string> { nodeId });
            return result;
        }

        [HttpGet("stopvalidatorengine")]
        public async Task<bool> StopValidatorEngine(string nodeId)
        {
            var result = new TONNodeSetupSvc().StopValidatorEngine(new List<string> { nodeId });
            //await new NodeSvc(_context).NodesRefreshMetrics(new List<string> { nodeId });
            return result;
        }

        [HttpGet("runexplorer")]
        public async Task<string> RunExplorer(string nodeId)
        {
            TunnelsData.ActiveTunnels.TryRemove(nodeId, out var old);
            old?.Disconnect();
            var helper = new SSHCommandHlp(new List<string> { nodeId });
            helper.ExecuteCommandsAsBashParallel(new List<string> {
                "pkill -f blockchain-explorer" ,
                "\"/root/ton/build/blockchain-explorer/blockchain-explorer\" -p \"/root/ton-keys/liteserver.pub\" -C \"/var/ton-work/etc/ton-global.config.json\" -a 127.0.0.1:3031 > \"/var/ton-work/blockchain-explorer.log\" 2>&1 &"
            });
            //await Task.Delay(3000);
            helper.OpenTunnels(600, TunnelsData.ActiveTunnels);
            //await Task.Delay(3000);
            
            return $"http://127.0.0.1:{helper.nodes.First().Port}/status";
        }

        [HttpGet("stopexplorer")]
        public void StopExplorer(string nodeId)
        {
            var helper = new SSHCommandHlp(new List<string> { nodeId });
            helper.ExecuteCommands(new List<string> { "pkill -f blockchain-explorer"});

            TunnelsData.ActiveTunnels.TryRemove(nodeId, out var old);
            old?.Disconnect();
        }        

        [HttpPost("edit")]
        public async Task<TonNetwork> UpdateConfiguration([FromBody] TonNetworkConfigurationModel model)
        {
            var network = model.Network;
            _context.TonNetworks.Update(network);
            var nodes = await _context.Nodes.Where(i => model.NodesIds.Contains(i.Id)).ToListAsync();

            foreach (var node in nodes)
            {
                node.TonNetworkId = network.Id;
            }

            await _context.SaveChangesAsync();
            return network;
        }

        [HttpDelete("softdelete")]
        public async Task<bool> SoftDeleteConfiguration(string id)
        {
            var item = await _context.TonNetworks.FirstOrDefaultAsync(i => i.Id == id);
            item.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

    }

}
