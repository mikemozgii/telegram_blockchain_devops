using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class Host
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UbuntuCore2004Path { get; set; }
        public string UbuntuTonCore2004Path { get; set; }
        public string HostTypeId { get; set; }
        public string PowershellCredentialId { get; set; }
        public int PowershellPort { get; set; }
        public string NodeLocationPath { get; set; }
        public int VCpuCores { get; set; }
        public int VMemoryGb { get; set; }
        public string Ip { get; set; }
      //  public int SshCoreVmPort { get; set; }
        public string IpSubset { get; set; }
        public string VmCoreCredentialId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
