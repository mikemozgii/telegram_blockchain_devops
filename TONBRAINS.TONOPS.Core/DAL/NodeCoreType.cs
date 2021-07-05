using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class NodeCoreType
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        
    }
}
