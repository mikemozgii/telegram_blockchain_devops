
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class v_NodeGridData
    {
        public string Id { get; set; }
        public string Ip { get; set; }
        public string SshIp { get; set; }
        public int SshPort { get; set; }
        public string Name { get; set; }
        public string HostName { get; set; }
        public string Description { get; set; }
        public bool Installed { get; set; }
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
        public string HostId { get; set; }
        public string Audit { get; set; }
        public string Status { get; set; }



        public string TonNetworkId { get; set; }
        public string NodeHostName { get; set; }
    }
}
