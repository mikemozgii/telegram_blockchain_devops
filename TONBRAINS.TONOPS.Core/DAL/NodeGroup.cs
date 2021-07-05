using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class NodeGroup
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        //public int NodeCount { get; set; }
    }
}
