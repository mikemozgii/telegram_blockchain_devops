using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class HostType
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }

    }
}
