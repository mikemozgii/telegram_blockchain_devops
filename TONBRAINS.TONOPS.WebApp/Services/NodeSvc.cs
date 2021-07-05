using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.SSH;
using Microsoft.TeamFoundation.Build.WebApi;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using Microsoft.EntityFrameworkCore;
using TONBRAINS.TONOPS.WebApp.Models.Ton;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Services
{
    public class NodeSvc : INodeSvc
    {
        private readonly TonOpsDbContext _context;

        public NodeSvc(TonOpsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DbSet<Node> GetContext() => _context.Nodes;

        //public async Task<Node> GetNode(string id)
        //{
        //    return await _context.Nodes.FirstOrDefaultAsync(i => i.Id == id);
        //}

        //public async Task<Node> GetAllNodes()
        //{
        //    return await _context.Nodes.FirstOrDefaultAsync(i => i.Id == id);
        //}


        //public async Task<SSHAuthMdl> GetNodeSSh(string nodeId)
        //{
        //    var node = await GetNode(nodeId);
        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).First();
        //    return new SSHAuthMdl(node.Ip, credential.UserName, credential.Password);
        //}

        public async Task<Node> Add(Node model)
        {
            model.Id = IdGenerator.Generate();
            model.CredentialId = "init";
            model.Status = "";
            await _context.Nodes.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Node> Update(Node model)
        {
            _context.Nodes.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteNodeGroup(string groupId)
        {
            var group = await _context.NodeGroups.FirstOrDefaultAsync(i => i.Id == groupId);
            if (group == null) return false;
            //var nodes = await _context.Nodes.Where(i => i.GroupId == groupId).ToListAsync();
            //nodes.ForEach(a => { a.GroupId = ""; });
            //_context.NodeGroups.Attach(group);
            _context.NodeGroups.Remove(group);
            return await _context.SaveChangesAsync() > 0;
        }

        //public async Task<NodeModel> Update(NodeModel model)
        //{  
        //        var uResult = await ExecuteUpdateSingle(model, new Query("nodes"), new string[] {
        //            nameof(NodeModel.Ip),
        //            nameof(NodeModel.Name),
        //            nameof(NodeModel.Description),
        //            nameof(NodeModel.Type),
        //            nameof(NodeModel.Os),
        //            nameof(NodeModel.DeploymentType),
        //            nameof(NodeModel.CredentialId),
        //            nameof(NodeModel.GroupId)});       
        //    return uResult;
        //}


        //public async Task<NodeModel> SetupRoot(string nodeId, string credentialId)
        //{

        //    var node = await GetNode(nodeId);
        //    if (node.IsRoot) return node;

        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).First();

        //    if (credential.UserName == "root")
        //    {
        //        node.IsRoot = true;
        //        node.CredentialId = credentialId;
        //        var r1 = await ExecuteUpdateSingle(node, new Query("nodes"), new string[] { nameof(NodeModel.IsRoot)});
        //        return r1;
        //    }


        //    var nodeAuth = new SSHAuthMdl(node.Ip, credential.UserName, credential.Password);
        //    var rootCredential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", credentialId))).First();

        //    var cmd = new[] {
        //        $"echo \"{nodeAuth.Password}\n{rootCredential.Password}\n{rootCredential.Password}\" | sudo -S passwd root",
        //        GetSudoCommand(nodeAuth.Password,"sed -i 's/#PermitRootLogin prohibit-password/PermitRootLogin yes/' /etc/ssh/sshd_config"),
        //        GetSudoCommand(nodeAuth.Password,"service ssh restart"),
        //    };

        //    var r = new SSHCommandHlp(nodeAuth).ExecuteCommandsParallel(cmd);

        //    if (!r) return null;


        //        node.IsRoot = true;
        //        node.CredentialId = credentialId;
        //        var uResult = await ExecuteUpdateSingle(node, new Query("nodes"), new string[] { nameof(NodeModel.IsRoot), nameof(NodeModel.CredentialId) });

        //    return uResult;
        //}

        //public async Task<NodeModel> SetupStaticIp(string nodeId, string ip, string gateway, string dns1, string dns2)
        //{
        //    var node = await GetNode(nodeId);
        //    if (node.IsStaticIp) return node;

        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).First();
        //    var nodeAuth = new SSHAuthMdl(node.Ip, credential.UserName, credential.Password);

        //    var commonHelprs = new CommonHelprs();
        //    var bashFile = commonHelprs.GetBashContentFromFile("static_ip");
        //    bashFile = bashFile.Replace("${ip_address}", ip).Replace("${dns_ip_address_1}", dns1).Replace("${dns_ip_address_2}", dns2).Replace("${gateway_ip_address}", gateway);
        //    using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
        //    {
        //        new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
        //    }

        //    node.IsStaticIp = true;
        //    node.Ip = ip;
        //    var uResult = await ExecuteUpdateSingle(node, new Query("nodes"), new string[] { nameof(NodeModel.IsStaticIp), nameof(NodeModel.Ip) });
        //    return uResult;
        //}

        //public async Task<NodeModel> SetupNtp(string nodeId)
        //{
        //    var node = await GetNode(nodeId);
        //    if (node.IsNtp) return node;

        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).First();
        //    var nodeAuth = new SSHAuthMdl(node.Ip, credential.UserName, credential.Password);

        //    var commonHelprs = new CommonHelprs();
        //    var bashFile = commonHelprs.GetBashContentFromFile("set_ntp");
        //    using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
        //    {
        //        new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
        //    }

        //    node.IsNtp = true;
        //    var uResult = await ExecuteUpdateSingle(node, new Query("nodes"), new string[] { nameof(NodeModel.IsNtp) });
        //    return uResult;
        //}

        public async Task<Node> Upgrade(string nodeId)
        {
            var credential = await _context.ViewNodesSsh.FirstOrDefaultAsync(i => i.Id == nodeId);
            var nodeAuth = new SSHAuthMdl(credential.SshIp, credential.UserName, credential.Password);

            var commonHelprs = new CommonHelprs();
            var bashFile = commonHelprs.GetBashContentFromFile("init_node");
            using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
            {
                new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
            }
            return await _context.Nodes.FirstOrDefaultAsync(i => i.Id == nodeId);
        }


        //public async Task<NodeModel> SetupElk(string nodeId)
        //{
        //    var node = await GetNode(nodeId);
        //    // if (node.IsDeployed) return node;

        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).First();
        //    var nodeAuth = new SSHAuthMdl(node.Ip, credential.UserName, credential.Password);

        //    var commonHelprs = new CommonHelprs();
        //    var bashFile = commonHelprs.GetBashContentFromFile("install_elk");
        //    using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
        //    {
        //        new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
        //    }

        //    node.IsDeployed = true;
        //    var uResult = await ExecuteUpdateSingle(node, new Query("nodes"), new string[] { nameof(NodeModel.IsDeployed) });
        //    return uResult;
        //}

        //public async Task<NodeModel> SetupNginix(string nodeId)
        //{
        //    var node = await GetNode(nodeId);
        //   // if (node.IsDeployed) return node;

        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).First();
        //    var nodeAuth = new SSHAuthMdl(node.Ip, credential.UserName, credential.Password);

        //    var commonHelprs = new CommonHelprs();
        //    var bashFile = commonHelprs.GetBashContentFromFile("install_nginx");
        //    using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
        //    {
        //        new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
        //    }

        //    node.IsDeployed = true;
        //    var uResult = await ExecuteUpdateSingle(node, new Query("nodes"), new string[] { nameof(NodeModel.IsDeployed) });
        //    return uResult;
        //}

        //public string GetSudoCommand(string passowrd, string command)
        //{
        //    return $"echo {passowrd} | sudo -S {command}";
        //}

        public async Task<Node> SetupZabbixAgent(string nodeId, string zabbixServerId)
        {
            
            var zabbixServer = await _context.ViewNodesSsh.FirstOrDefaultAsync(i => i.Id == zabbixServerId);
            var serverZabbix = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == zabbixServerId);
            var node = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == nodeId);
            var zabbixUser = await _context.Credentials.FirstOrDefaultAsync(i => i.Name == "ZabbixDefault");
            using (var context = new ZabbixApi.Context($"http://127.0.0.1:{zabbixServer.SshPort}/zabbix/api_jsonrpc.php", zabbixUser.UserName, zabbixUser.Password))
            {
                var host = new ZabbixApi.Entities.Host {
                    ipmi_privilege = ZabbixApi.Entities.Host.PrivilegeLevel.User,
                    flags = ZabbixApi.Entities.Host.Flags.PlainHost,
                    host = node.Name, 
                    interfaces = new List<ZabbixApi.Entities.HostInterface>{ new ZabbixApi.Entities.HostInterface { type = ZabbixApi.Entities.HostInterface.InterfaceType.Agent, main = true, useip = true, ip = node.Ip, port = "10050", dns = "" } },
                    groups = new List<ZabbixApi.Entities.HostGroup> { new ZabbixApi.Entities.HostGroup { Id = "2" } },
                    templates = new List<ZabbixApi.Entities.Template> { new ZabbixApi.Entities.Template { Id = "10001" } },
                };
                if (int.TryParse(await context.Hosts.CreateAsync(host), out var zabbixHostId))
                    node.ZabbixHostId = zabbixHostId;
            }
            var SSHCommandHlp = new SSHCommandHlp(nodeId);
            var commonHelprs = new CommonHelprs();
            var bashContent = commonHelprs.GetBashContentFromFile("install_zabbix_agent").Replace("${host_name}", serverZabbix.HostName);
            SSHCommandHlp.ExecuteCommandsParallelBashByContent(bashContent);

            node.ZabbixServerId = zabbixServerId;
            _context.Nodes.Update(node);
            await _context.SaveChangesAsync();
            return node;
        }

        public async Task<Dictionary<string, bool>> GetNodesProblems(IEnumerable<string> nodeIds)
        {
            var result = new Dictionary<string, bool>();
            if (nodeIds?.Any() == false) return result;
            var groupedNodes = (await _context.Nodes.Where(i => nodeIds.Contains(i.Id) && i.ZabbixHostId != 0).AsNoTracking().ToListAsync())
                .GroupBy(i => i.ZabbixServerId)
                .ToDictionary(i => i.Key, i => i.ToDictionary(i => i.ZabbixHostId.ToString(), i => i.Id));
            if(groupedNodes.Keys?.Any() == false) return result;
            var credentialZabbix = await _context.ViewNodesSsh.Where(i => groupedNodes.Keys.Contains(i.Id)).AsNoTracking().ToListAsync();
            new SSHCommandHlp(credentialZabbix.Select(s => s.Id).ToList()).OpenTunnels(120, TunnelsData.ActiveTunnels);
            await Task.Delay(3000);
            var zabbixUser = await _context.Credentials.FirstOrDefaultAsync(i => i.Name == "ZabbixDefault");
            foreach (var group in groupedNodes)
            {
                var zabbixServer = credentialZabbix.FirstOrDefault(i => i.Id == group.Key);
                if (zabbixServer == null) continue;

                using (var context = new ZabbixApi.Context($"http://127.0.0.1:{zabbixServer.SshPort}/zabbix/api_jsonrpc.php", zabbixUser.UserName, zabbixUser.Password))
                {
                    var data = await context.Hosts.GetByIdAsync(group.Value.Keys);
                    foreach (var item in data)
                    {
                        if (result.ContainsKey(item.Id)) continue;

                        result.Add(group.Value[item.Id], !string.IsNullOrEmpty(item.error));
                    }
                }
            }
            return result;
        }

        public async Task<Node> ChangeZabbixServer(string nodeId, string zabbixServerId)
        {
            var node = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == nodeId);

            var serverZabbixPrev = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == node.ZabbixServerId);
            var credentialZabbixPrev = await _context.ViewNodesSsh.FirstOrDefaultAsync(i => i.Id == node.ZabbixServerId);

            var credentialZabbix = await _context.ViewNodesSsh.FirstOrDefaultAsync(i => i.Id == zabbixServerId);
            var serverZabbix = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == zabbixServerId);
            var zabbixUser = await _context.Credentials.FirstOrDefaultAsync(i => i.Name == "ZabbixDefault");


            using (var context = new ZabbixApi.Context($"http://{serverZabbixPrev.Ip}/zabbix/api_jsonrpc.php", zabbixUser.UserName, zabbixUser.Password))
            {
                await context.Hosts.DeleteAsync(node.ZabbixHostId.ToString());
            }

            using (var context = new ZabbixApi.Context($"http://{serverZabbix.Ip}/zabbix/api_jsonrpc.php", zabbixUser.UserName, zabbixUser.Password))
            {
                var host = new ZabbixApi.Entities.Host
                {
                    ipmi_privilege = ZabbixApi.Entities.Host.PrivilegeLevel.User,
                    flags = ZabbixApi.Entities.Host.Flags.PlainHost,
                    host = node.Name,
                    interfaces = new List<ZabbixApi.Entities.HostInterface> { new ZabbixApi.Entities.HostInterface { type = ZabbixApi.Entities.HostInterface.InterfaceType.Agent, main = true, useip = true, ip = node.Ip, port = "10050", dns = "" } },
                    groups = new List<ZabbixApi.Entities.HostGroup> { new ZabbixApi.Entities.HostGroup { Id = "2" } },
                    templates = new List<ZabbixApi.Entities.Template> { new ZabbixApi.Entities.Template { Id = "10001" } },
                };
                if (int.TryParse(await context.Hosts.CreateAsync(host), out var zabbixHostId))
                {
                    node.ZabbixHostId = zabbixHostId;
                    node.ZabbixServerId = zabbixServerId;
                }
            }

            var credential = await _context.ViewNodesSsh.FirstOrDefaultAsync(i => i.Id == nodeId);
            var nodeAuth = new SSHAuthMdl(credential.SshIp, credential.UserName, credential.Password);

            var commonHelprs = new CommonHelprs();
            var bashFile = commonHelprs.GetBashContentFromFile("change_zabbix_server").Replace("${host_name}", serverZabbix.HostName);
            using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
            {
                new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
            }
            _context.Nodes.Update(node);
            await _context.SaveChangesAsync();
            return node;
        }

        //public async Task<NodeModel> setupPostgreSQL(string nodeId)
        //{
        //    var node = await GetNode(nodeId);
        //    if (node.IsDeployed) return node;

        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).First();
        //    var nodeAuth = new SSHAuthMdl(node.Ip, credential.UserName, credential.Password);

        //    var commonHelprs = new CommonHelprs();
        //    var bashFile = commonHelprs.GetBashContentFromFile("install_postgres");
        //    using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
        //    {
        //        new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
        //    }
        //    node.IsDeployed = true;
        //    var uResult = await ExecuteUpdateSingle(node, new Query("nodes"), new string[] { nameof(NodeModel.IsDeployed) });
        //    return uResult;
        //}
        public async Task<Node> SetupZabbixServer(string nodeId)
        {
            //var credential = await _context.ViewNodesSsh.FirstOrDefaultAsync(i => i.Id == nodeId);
            //var nodeAuth = new SSHAuthMdl(credential.SshIp, credential.UserName, credential.Password);

            var commonHelprs = new CommonHelprs();
            var SSHCommandHlp = new SSHCommandHlp(nodeId);
            SSHCommandHlp.ExecuteCommandsParallelBash("install_zabbix_server");

            var node = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == nodeId);
            node.IsDeployed = true;
            await _context.SaveChangesAsync();
            return node;
        }


        public async Task<Node> SetupHostName(string nodeId, string hostName)
        {
            var credential = await _context.ViewNodesSsh.FirstOrDefaultAsync(i => i.Id == nodeId);
            var nodeAuth = new SSHAuthMdl(credential.SshIp, credential.UserName, credential.Password);

            var commonHelprs = new CommonHelprs();
            var bashFile = commonHelprs.GetBashContentFromFile("set_host_name");
            bashFile = bashFile.Replace("${new_host_name}", hostName);
            using (var stream = commonHelprs.GenerateStreamFromString(bashFile))
            {
                new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
            }
            var node = await _context.Nodes.FirstOrDefaultAsync(i => i.Id == nodeId);
            node.HostName = hostName;
            _context.Nodes.Update(node);
            await _context.SaveChangesAsync();
            return node;
        }

        public async Task<IEnumerable<v_NodeMetric>> NodesRefreshMetrics(IEnumerable<string> ids)
        {
            var nodes = await _context.Nodes.Where(i => ids.Contains(i.Id)).ToListAsync();
            var cmds = new List<string>
            {
                "df -BG --total --output=source,avail,pcent,size | grep total",
                $"ls -sh --block-size=KB {GlobalVarHandler.TON_WORK_NODE_LOG}",
                $"ps -aux | grep [/]{GlobalVarHandler.VALIDATOR_ENGINE.Remove(0,1)}",
                "free -m | grep Mem",
                "ps aux -e --no-headers | wc -l"
            };
            var nodeIds = nodes.Select(s => s.Id).ToList();
            var runningNodeIds = nodes.Where(node => node.Type == NodeTypes.QUANTONActive).Select(s => s.Id).ToList(); 
            var timeDiffs = new TONValidatorEngineConsoleSvc().GetMasterBlockTimeDifference(runningNodeIds);
            var configs = new TonNetworkSvc().GetActualTonConfigForNodes(runningNodeIds.ToArray());
            var diskSpaces = new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel(cmds);


            var cmds1 = new List<string>
            {
                $"ps -aux | grep [/]{GlobalVarHandler.VALIDATOR_ENGINE.Remove(0,1)}"
            };

            var validatorActive = new SSHCommandHlp(nodeIds).ExecuteCommandsWithResultParallel(cmds1);

            var date = DateTime.UtcNow;
            foreach (var node in nodes)
            {
                var metricModel = new NodeMetric { NodeId = node.Id, Date = date, Id = IdGenerator.Generate() };

                var hasData = diskSpaces.TryGetValue(node.Id, out var cmdData);
                if (!hasData) continue;

                var cmdResults = cmdData.ToList();

                if (cmdResults.Count >= 1)
                {
                    var diskInfos = cmdResults[0].Replace("total", "").Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (diskInfos.Length == 3)
                    {
                        metricModel.Diskavailible = Convert.ToInt32(diskInfos[0].Replace("G", ""));
                        metricModel.Diskpercent = Convert.ToDouble(diskInfos[1]?.Replace("%", ""));
                        metricModel.Disktotal = Convert.ToInt32(diskInfos[2].Replace("G", ""));
                    }
                }

                if (cmdResults.Count >= 4)
                {
                    var diskInfos = cmdResults[3].Replace("Mem:", "").Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    metricModel.RamTotal = Convert.ToInt32(diskInfos[0].Trim());
                    metricModel.RamUsed = Convert.ToInt32(diskInfos[1].Trim());
                    metricModel.RamFree = Convert.ToInt32(diskInfos[2].Trim());
                    metricModel.RamAvailable = Convert.ToInt32(diskInfos[5].Trim());
                }

                if (cmdResults.Count >= 5)
                {
                    metricModel.Processes = Convert.ToInt32(cmdResults[4]);
                }

                if (node.Type == NodeTypes.QUANTONActive)
                {
                    if (cmdResults.Count >= 2)
                    { 
                        var tonLogsInfos = cmdResults[1]?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        if (tonLogsInfos?.Count() > 1)
                            metricModel.Tonlogsize = Convert.ToInt32(tonLogsInfos.FirstOrDefault()?.Replace("kB", "").Trim());
                       
                    }

                    var hasData1 = validatorActive.TryGetValue(node.Id, out var cmdData1);
                    if (hasData1)
                    {
                        var ccmdData1Results = cmdData1.ToList();
                        if (ccmdData1Results.Count >= 1)
                        {
                            metricModel.ValidatorActive = !string.IsNullOrEmpty(ccmdData1Results[0]);
                        }
                       
                    }
                    

                    if (timeDiffs.ContainsKey(node.Id))
                        metricModel.Timediff = Convert.ToInt32(timeDiffs[node.Id]);
                    if (configs.ContainsKey(node.Id))
                        metricModel.Config = configs[node.Id];
                }


                await _context.NodesMetrics.AddAsync(metricModel);
            }
            
            await _context.SaveChangesAsync();

            return await GetNodesMetrics(ids);
        }

        public async Task<IEnumerable<v_NodeMetric>> GetNodesMetrics(IEnumerable<string> ids)
        {
            var data = await _context.ViewNodesMetrics.Where(i => ids.Contains(i.Id)).OrderByDescending(o => o.Date).AsNoTracking().ToListAsync();

            return data;
        }

        public async Task<IEnumerable<v_NodeMetric>> GetLatesNodesMetrics(IEnumerable<string> ids)
        {
            var result = new List<v_NodeMetric>();
            foreach (var id in ids)
            {
                var node = await _context.ViewNodesMetrics.Where(i => id == i.Id).OrderByDescending(o => o.Date).AsNoTracking().FirstOrDefaultAsync();
                if(node != null)
                    result.Add(node);
            }
            return result;
        }

        public async Task<IEnumerable<v_Node_Shh>> GetNodesCredentials(IEnumerable<string> ids) =>
            await _context.ViewNodesSsh.Where(i => ids.Contains(i.Id)).ToListAsync();
    }
}
