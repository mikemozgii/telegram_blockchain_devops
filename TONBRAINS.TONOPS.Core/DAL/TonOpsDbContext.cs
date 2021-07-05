using Marques.EFCore.SnakeCase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.DAL 
{
	public class TonOpsDbContext : DbContext
	{

		public TonOpsDbContext(DbContextOptions options)
		: base(options)
		{
		}


        public DbSet<Token> Tokens { get; set; }
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Security> Securities { get; set; }
		public DbSet<Node> Nodes { get; set; }
		public DbSet<NodeGroup> NodeGroups { get; set; }
		public DbSet<TonConfiguration> TonConfigurations { get; set; }
		public DbSet<TonNetwork> TonNetworks { get; set; }
		public DbSet<Host> Hosts { get; set; }
		public DbSet<NodeMetric> NodesMetrics { get; set; }

		public DbSet<TransferDeaction> TransferDeactions { get; set; }
		

		public DbSet<v_Node_Shh> ViewNodesSsh { get; set; }
		public DbSet<v_ConfigurationNetworkNode> ViewConfigurationNetworkNodes { get; set; }
		public DbSet<v_HostGridData> ViewHostsGrid{ get; set; }
        public DbSet<v_NodeMetric> ViewNodesMetrics { get; set; }
        public DbSet<v_TonNetworkGridData> ViewTonNetworksGrid { get; set; }
		public DbSet<v_NodeGridData> ViewNodesGrid { get; set; }
		

		public DbSet<SmartKey> SmartKeys { get; set; }
		public DbSet<SmartKeyView> ViewSmartKeys { get; set; }
		public DbSet<SmartContract> SmartContracts { get; set; }
		public DbSet<SmartContractView> ViewSmartContracts { get; set; }
		public DbSet<SmartContractLib> SmartContractsLibs { get; set; }
		public DbSet<SmartContractLibView> ViewSmartContractsLibs { get; set; }
		public DbSet<SmartAccount> SmartAccounts { get; set; }
		public DbSet<SmartAccountView> ViewSmartAccounts { get; set; }
		public DbSet<SmartAccountKey> SmartAccountKeys { get; set; }
		public DbSet<SmartAccountKeyView> ViewSmartAccountKeys { get; set; }
		public DbSet<SmartAccountNetwork> SmartAccountNetworks { get; set; }
		public DbSet<SmartAccountNetworkView> ViewSmartAccountNetworks { get; set; }
		public DbSet<NetworkSmartKeyView> ViewNetworkSmartKeys { get; set; }
		public DbSet<SmartAccountBalanceTransferLog> SmartAccountBalanceTransferLogs { get; set; }
		public DbSet<SmartAccountBalanceTransferLogView> ViewSmartAccountBalanceTransferLogs { get; set; }
		public DbSet<SmartAccountStatus> SmartAccountStatuses { get; set; }
		public DbSet<SmartType> SmartTypes { get; set; }
		public DbSet<Credential> Credentials { get; set; }
		public DbSet<HostType> HostTypes { get; set; }
		public DbSet<NodeCoreType> NodeCoreTypes { get; set; }
		public DbSet<SmartAccountStateLog> SmartAccountStateLogs { get; set; }

		

		public DbSet<FileEntity> Files { get; set; }

//#if DEBUG
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.LogTo(Console.WriteLine);
//#endif
        protected override void OnModelCreating(ModelBuilder builder)
        {
			base.OnModelCreating(builder);
			builder.ToSnakeCase();
		
		}
    }
}
