using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class NodeModel
    {
        public string Id { get; set; }
        public string Ip { get; set; }
        public string Name { get; set; }
        public string HostName { get; set; }
        public string Description { get; set; }
        public bool Installed { get; set; }
        public string Modules { get; set; }
        public NodeTypes Type { get; set; }
        public string Os { get; set; }
        public DeploymentNodeTypes DeploymentType { get; set; }
        public bool IsRoot { get; set; }
        public bool IsStaticIp { get; set; }
        public bool IsNtp { get; set; }
        public string CredentialId { get; set; }
        public bool IsCustomService { get; set; }
        public string GroupId { get; set; }
        public bool IsDeployed { get; set; }
        public int ZabbixHostId { get; set; }
        public string ZabbixServerId { get; set; }
        public string InstanceName { get; set; }
        public string Audit { get; set; }

    }
}
