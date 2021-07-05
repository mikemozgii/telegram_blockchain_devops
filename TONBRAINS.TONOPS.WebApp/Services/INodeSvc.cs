using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.WebApp.Models.Ton;

namespace TONBRAINS.TONOPS.WebApp.Services
{
    public interface INodeSvc
    {
        Task<Node> Add(Node model);
        Task<Node> Update(Node model);

        DbSet<Node> GetContext();

        Task<bool> DeleteNodeGroup(string groupId);

        Task<Node> Upgrade(string nodeId);

        Task<Node> SetupHostName(string nodeId, string hostName);

        Task<Node> ChangeZabbixServer(string nodeId, string zabbixServerId);

        Task<Node> SetupZabbixAgent(string nodeId, string zabbixServerId);

        Task<Node> SetupZabbixServer(string nodeId);

        Task<IEnumerable<v_Node_Shh>> GetNodesCredentials(IEnumerable<string> ids);

        Task<IEnumerable<v_NodeMetric>> NodesRefreshMetrics(IEnumerable<string> ids);
        Task<IEnumerable<v_NodeMetric>> GetNodesMetrics(IEnumerable<string> ids);

        Task<IEnumerable<v_NodeMetric>> GetLatesNodesMetrics(IEnumerable<string> ids);

        Task<Dictionary<string, bool>> GetNodesProblems(IEnumerable<string> nodeIds);
        //Task<bool> Delete(Node model);
    }
}
