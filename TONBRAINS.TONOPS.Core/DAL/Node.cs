using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class Node
    {
        [Key]
        public string Id { get; set; }
        public string Ip { get; set; }
        public string SshIp { get; set; }
        public int SshPort { get; set; }
        public string Name { get; set; }
        public string HostName { get; set; }
        public string Description { get; set; }
        public bool Installed { get; set; }
        [Column(TypeName = "jsonb")]
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
        public string HostId { get; set; }
        public bool IsDeleted { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Audit { get; set; }
        public string Status { get; set; }

        [ForeignKey(nameof(GroupId))]
        public NodeGroup NodeGroup { get; set; }


        public string TonNetworkId { get; set; }


        public ICollection<SmartAccount> SmartAccounts { get; set; }


    }
}
