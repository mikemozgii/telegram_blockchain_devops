using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class v_HostGridData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Ip { get; set; }
        public int VCpuCores { get; set; }
        public int VMemoryGb { get; set; }
        public long NodesCount { get; set; }
        public int SshCoreVmPort { get; set; }
        public string IpSubset { get; set; }
        public string HostTypeId { get; set; }
        public string HostTypeName { get; set; }
        public string PowershellCredentialId { get; set; }
        public string VmCoreCredentialId { get; set; }
        public string PowershellCredentialName { get; set; }
        public string VmCoreCredentialName { get; set; }
    }
}
