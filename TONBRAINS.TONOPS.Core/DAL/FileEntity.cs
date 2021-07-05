using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class FileEntity
    {
        [Key]
        public string Id { get; set; }
        public uint Oid { get; set; }
        public string Name { get; set; }
    }
}
