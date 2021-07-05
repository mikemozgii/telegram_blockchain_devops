using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using System.Linq;
using Newtonsoft.Json;
using System;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.Common;
using TONBRAINS.TONOPS.WebApp.Services;
using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using TONBRAINS.TONOPS.WebApp.Models.Nodes;
using TONBRAINS.TONOPS.WebApp.Helpers;
using TONBRAINS.TONOPS.Core.DAL;
using Microsoft.EntityFrameworkCore;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.WebApp.Models.Ton;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Collections.Concurrent;
using TONBRAINS.TONOPS.Core.SSH;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/nodes")]
    public class NodesController : SessionController
    {
        private readonly ISshStream _sshService;
        private readonly INodeSvc _nodeService;
        private readonly TonOpsDbContext _context;

        public NodesController(ISshStream sshStream, INodeSvc nodeService, TonOpsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _sshService = sshStream ?? throw new ArgumentNullException(nameof(sshStream));
            _nodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
        }

        [Route("nodetypes")]
        public IEnumerable<SelectListItem<int>> NodesType()
            => EnumHelpers<NodeTypes>.GetEnumSelectList().Where(x => x.Id != -1).ToArray();

        [Route("deploymentnodetypes")]
        public IEnumerable<SelectListItem<int>> DeploymentNodesType()
            => EnumHelpers<DeploymentNodeTypes>.GetEnumSelectList();

        [Route("grid")]
        public async Task<IEnumerable<v_NodeGridData>> GetNodes()
        {
            return await _context.ViewNodesGrid.ToListAsync();
        }

        [HttpGet("nodesrefreshmetrics")]
        public async Task<IEnumerable<v_NodeMetric>> NodesRefresh(string id)
        {
            var nodeIds = await _context.Nodes.Where(i => i.GroupId == id).Select(s => s.Id).ToListAsync();
            return await _nodeService.NodesRefreshMetrics(nodeIds);
        }

        [HttpGet("nodemetric")]
        public async Task<object> NodeMetric(string id)
        {
            return await _context.NodesMetrics.Where(i => i.NodeId == id).ToListAsync();
        }

        [Route("nginxnodes")]
        public async Task<IEnumerable<NodeWithGroup>> NodesNginx()
            => await ExecuteQuery<NodeWithGroup>(new Query("nodes").Join("node_groups", "node_groups.id", "group_id")
                    .Select("nodes.*", "node_groups.name as group_name").Where("nodes.type", "6"));


        [Route("findbygroup")]
        public async Task<IEnumerable<v_NodeGridData>> FindNodesByGroup(string groupId)
           => await _context.ViewNodesGrid.Where(i => i.GroupId == groupId).ToListAsync();

        [Route("TuneModels")]
        public async Task<IEnumerable<TuneModel>> TuneModels()
        {
            var query = new Query("bash_scripts")
                .Select("name", "description")
                .Where("type", "tuning")
                .OrderBy("order");

            return await ExecuteQuery<TuneModel>(query);
        }

        //[Route("savemoduletonode")]
        //public async Task<bool> SaveModuleToNode(string id, string moduleId)
        //{
        //    var node = await new NodeSvc().GetNode(id);
        //    if (node == null) return false;

        //    var modules = JsonConvert.DeserializeObject<List<string>>(node.Modules);
        //    if (!modules.Any(a => a == moduleId)) modules.Add(moduleId);
        //    node.Modules = JsonConvert.SerializeObject(modules);

        //    await ExecuteInsertOrUpdate(node, new Query("nodes"), insert: false, touchedFields: new string[] { "Modules" });

        //    return true;
        //}

        [Route("modulenode")]
        public async Task<NodeModel> ModuleEnvironmentsWithModels(string moduleId) => await ExecuteSqlFirst<NodeModel>($"SELECT * FROM nodes WHERE modules @> '[\"{moduleId}\"]'", Session.AccountId);


        [HttpPost("add")]
        public async Task<Node> AddNode([FromBody] Node item)
        {
            return await _nodeService.Add(item);
        }

        [HttpPost("edit")]
        public async Task<Node> UpdateNode([FromBody] Node item)
        {
            return await _nodeService.Update(item);
        }

        [Route("install")]
        [HttpPost]
        public async Task<NodeModel> InstallNode([FromBody] NodeModel item)
        {
            var host = item.Ip;
            //var pwd = item.Password;
            //if (m_SshService.CreateRoot(host, "admindp", pwd).Length > 0) return request;
            //if (m_SshService.SetStaticIp(host, pwd).Length > 0) return request;
            //_sshService.InstallZabbixAgent(host, pwd);
            //item.Installed = true;
            return await ExecuteInsertOrUpdate(item, new Query("nodes"), false, touchedFields: new string[] { "Installed" });
        }

        //[Route("tune")]
        //[HttpPost]
        //public async Task<NodeModel> TuneNode([FromBody] TuneRequestModel item)
        //{
        //    var node = await new NodeSvc().GetNode(item.NodeId);

        //    //var queryScript = new Query("bash_scripts")
        //    //    .Where("name", item.BashScript);
        //    //var script = (await ExecuteQuery<BashScriptModel>(queryScript)).FirstOrDefault();

        //    //var files = string.IsNullOrEmpty(script.Files) ?
        //    //    new Dictionary<string, string>() :
        //    //    JsonConvert.DeserializeObject<Dictionary<string, string>>(script.Files);

        //    //var commands = string.IsNullOrEmpty(script.Commands) ?
        //    //    Enumerable.Empty<string>() :
        //    //    JsonConvert.DeserializeObject<IEnumerable<string>>(script.Commands);

        //    //var result = _sshService.ExecuteCommands(node.Ip, "root", node.Password, files, commands);
        //    //if (!string.IsNullOrEmpty(result))
        //    //    Console.WriteLine(result);

        //    return node;
        //}

        //[Route("groups")]
        //[HttpGet]
        //public async Task<IEnumerable<SelectListItem<string>>> GetGroups()
        //=> (await ExecuteQuery<NodeGroup>(new Query("node_groups")))
        //    .Select(x => new SelectListItem<string>(x.Name, x.Name))
        //    .ToArray();



        [Route("commandssh")]
        [HttpGet]
        public async Task ExecuteCommandSsh(string nodeId, string command)
        {
            var credential = (await _nodeService.GetNodesCredentials(new List<string> { nodeId })).FirstOrDefault();
            if (credential == null) return;
            using var client = _sshService.GetSshClient(new ServerTemplate
            {
                Host = credential.SshIp,
                Password = credential.Password,
                Port = credential.SshPort,
                User = credential.UserName
            });
            try
            {
                client.Connect();
                await _sshService.ExecuteCommands(client, new string[] { command }, credential.Id);
            }
            catch (Exception exception)
            {
                await _sshService.WriteHistory(credential.Id, exception.Message);
                throw exception;
            }
            finally
            {
                client.Dispose();
            }
        }

        [Route("resethistory")]
        [HttpGet]
        public Task ResetHistory(string host)
            => Task.FromResult(Program.History[host].RemoveAll(x => x.Length > 0));

        //[Route("setuproot")]
        //[HttpGet]
        //public async Task<NodeModel> SetupRoot(string nodeId, string credentialId)
        //{
        //    return await new NodeSvc().SetupRoot(nodeId, credentialId);
        //}

        //[Route("setupstaticip")]
        //[HttpGet]
        //public async Task<NodeModel> SetupStaticIp(string nodeId, string ip, string gateway, string dns1, string dns2)
        //{
        //    return await new NodeSvc().SetupStaticIp(nodeId, ip, gateway, dns1, dns2);
        //}

        [Route("setuphostname")]
        [HttpGet]
        public async Task<Node> SetupHostName(string nodeId, string hostName)
        {
            return await _nodeService.SetupHostName(nodeId, hostName);
        }

        //[Route("setuppostgresql")]
        //[HttpGet]
        //public async Task<NodeModel> setupPostgreSQL(string nodeId)
        //{
        //    return await new NodeSvc().setupPostgreSQL(nodeId);
        //}

        [Route("setupzabbixserver")]
        [HttpGet]
        public async Task<Node> SetupZabbixServer(string nodeId)
        {
            return await _nodeService.SetupZabbixServer(nodeId);
        }

        [Route("setupzabbixagent")]
        [HttpGet]
        public async Task<Node> SetupZabbixAgent(string nodeId, string zabbixServerId)
        {
            new SSHCommandHlp(new List<string> { zabbixServerId }).OpenTunnels(60, TunnelsData.ActiveTunnels);
            await Task.Delay(5000);
            return await _nodeService.SetupZabbixAgent(nodeId, zabbixServerId);
        }
        [Route("changezabbixserver")]
        [HttpGet]
        public async Task<Node> ChangeZabbixServer(string nodeId, string zabbixServerId)
        {
            return await _nodeService.ChangeZabbixServer(nodeId, zabbixServerId);
        }

        //[Route("setupdatetime")]
        //[HttpGet]
        //public async Task<NodeModel> SetupDateTime(string nodeId)
        //{
        //    return await new NodeSvc().SetupNtp(nodeId);
        //}

        //[Route("setupnginix")]
        //[HttpGet]
        //public async Task<NodeModel> SetupNginix(string nodeId)
        //{
        //    return await new NodeSvc().SetupNginix(nodeId);
        //}

        //[Route("setupelk")]
        //[HttpGet]
        //public async Task<NodeModel> SetupElk(string nodeId)
        //{
        //    return await new NodeSvc().SetupElk(nodeId);
        //}

        [Route("upgrade")]
        [HttpGet]
        public async Task<Node> Upgrade(string nodeId)
        {
            return await _nodeService.Upgrade(nodeId);
        }



        [Route("dropdowns")]
        [HttpGet]
        public async Task<NodeDropdowns> GetDropdowns()
             => new NodeDropdowns
             {
                 Hosts = (await ExecuteQuery<Host>(new Query("hosts"))).Select(x => new SelectListItem<string>(x.Id, x.Name)),
                 Groups = (await ExecuteQuery<NodeGroup>(new Query("node_groups"))).Select(x => new SelectListItem<string>(x.Id, x.Name)),
                 DeplymentTypes = DeploymentNodesType(),
                 NodeTypes = NodesType(),
                 ZabbixServers = (await ExecuteQuery<NodeModel>(new Query("nodes").Where("type", NodeTypes.ZabbixServer).WhereNot("host_name", ""))).Select(x => new { x.Id, Title = x.Name, x.HostName, x.Ip }),
                 Credentials = (await ExecuteQuery<Common.Models.Credential>(new Query("credentials"))).Select(x => new { x.Id, Title = x.Name, x.UserName }),
                 OperatingSystems = (await ExecuteQuery<NodeGroup>(new Query("operating_systems"))).Select(x => new SelectListItem<string>(x.Name, x.Name))
             };

        [Route("checkserverstatus")]
        [HttpGet]
        public async Task CheckServerStatus(string ids, string groupId)
        {
            var nodeIds = JsonConvert.DeserializeObject<string[]>(ids);
            var credentials = await _nodeService.GetNodesCredentials(nodeIds);
            await Task.Run(async () =>
            {
                foreach (var credential in credentials.Where(i=>!string.IsNullOrEmpty(i.SshIp)).ToList())
                {
                    await _sshService.CheckServer(credential, groupId);
                }
            });
        }

        [Route("zabbixchecknodes")]
        [HttpGet]
        public async Task<Dictionary<string, bool>> ZabbixCheckNodes(string ids)
        {
            var nodeIds = JsonConvert.DeserializeObject<string[]>(ids);
            return await _nodeService.GetNodesProblems(nodeIds);
        }

        [HttpGet("changenodestatus")]
        public async Task<string> ChangeNodeStatus(string id, string status)
        {
            //var node = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == id);
            //if (node == null) return "";
            if (status == NodeStatusesHandler.Restart)
            {
                new NodeManagementSvc().RebootNodes(new List<string> { id });
            }
            if (status == NodeStatusesHandler.Stop)
            {
                new NodeManagementSvc().StopNodes(new List<string> { id });
            }

            return status;
        }

        [HttpGet("closetunnels")]
        public void CloseTunnels()
        {
            foreach (var item in TunnelsData.ActiveTunnels)
            {
                TunnelsData.ActiveTunnels.TryRemove(item.Key, out var old);
                old?.Disconnect();
            }
        }

        [HttpGet("createtunnel")]
        public void CreateTunnel(string id, int seconds)
        {
            new SSHCommandHlp(new List<string> { id }).OpenTunnels(seconds, TunnelsData.ActiveTunnels);
        }
    }
}
