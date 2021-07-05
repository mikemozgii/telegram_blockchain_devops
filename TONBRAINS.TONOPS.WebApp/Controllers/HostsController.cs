using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using TONBRAINS.TONOPS.Core.DAL;
using Microsoft.EntityFrameworkCore;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.WebApp.Models.Ton;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.Core.Handlers;
using System.Text.RegularExpressions;
using TONBRAINS.TONOPS.WebApp.Services;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/hosts")]
    [ApiController]
    public class HostsController : SessionController
    {
        private readonly TonOpsDbContext _context;
        private readonly INodeSvc _nodeService;
        

        public HostsController(TonOpsDbContext context, INodeSvc nodeService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _nodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
        }

        [HttpGet("data")]
        public async Task<object> GridData()
        {
            return new
            {
                hosts = await _context.ViewHostsGrid.ToListAsync(),
                types = await _context.HostTypes.Select(s => new { s.Id, Title = s.Name }).ToListAsync(),
                nodeCoreTypes = await _context.NodeCoreTypes.Select(s => new { s.Id, Title = s.Name }).ToListAsync(),
                credentials = await _context.Credentials.Select(s => new { s.Id, Title = s.Name}).ToListAsync(),
            };
        }

        [HttpDelete("deletenode")]
        public Task<bool> DeleteNode(string id)
        {
            var r = new HostManagementSvc().DeleteNode(id);
            return Task.FromResult(r);
        }

        [HttpGet("nodesrefresh")]
        public async Task<IEnumerable<v_NodeMetric>> NodesRefresh(string id)
        {
            var nodeIds = await _context.Nodes.Where(i => i.HostId == id).Select(s => s.Id).ToListAsync();
            return await _nodeService.NodesRefreshMetrics(nodeIds);
        }

        [HttpGet("nodes")]
        public async Task<IEnumerable<v_NodeMetric>> Nodes(string id)
        {
            var nodeIds = await _context.Nodes.Where(i => i.HostId == id).Select(s => s.Id).ToListAsync();
            return await _nodeService.GetLatesNodesMetrics(nodeIds);
        }

        [HttpGet("nodemetric")]
        public async Task<object> NodeMetric(string id)
        {
            return await _context.NodesMetrics.Where(i => i.NodeId == id).ToListAsync();
        }


        [HttpGet("single")]
        public async Task<Host> Single(string id)
        {
            return await _context.Hosts.FirstOrDefaultAsync(i => i.Id == id);
        }

        [HttpGet("install")]
        public void InstallNode(string id, int index, int port, string coreTypeId)
        {
           var tempFix = Convert.ToInt32($"22{index}");
           new HostManagementSvc().AddNode(id, coreTypeId, index, tempFix);
        }

        [HttpGet("changenodestatus")]
        public async Task<string> ChangeNodeStatus(string id, string status)
        {
            var service = new HostManagementSvc();
            if(status == NodeStatusesHandler.Restart)
            {
                service.RestartNode(id);
            }
            else if(status == NodeStatusesHandler.Stop)
            {
                service.StopNode(id);
            }
            else if (status == NodeStatusesHandler.Run)
            {
                service.StartNode(id);
            }

            return status;
        }

        
        [HttpPost("changenodehost")]
        public async Task ChangeNodeHost([FromBody] ChangeNodeHostModel model)
        {
            var node = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == model.Id);
            node.HostId = model.HostId;
            _context.Nodes.Update(node);
            await _context.SaveChangesAsync();
        }

        [HttpPost("add")]
        public async Task<Host> AddHost([FromBody] Host item)
        {
            item.Id = IdGenerator.Generate();
            await _context.Hosts.AddRangeAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpPost("edit")]
        public async Task<Host> UpdateHost([FromBody] Host item)
        {
            _context.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpDelete("softdelete")]
        public async Task<bool> SoftDeleteHost(string id)
        {
            var item = await _context.Hosts.FirstOrDefaultAsync(i => i.Id == id);
            item.IsDeleted = true;
            return await _context.SaveChangesAsync() > 0;
        }

    }

}
