using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class NetworkSmartKeyView
    {
        [Key]
        public string NetworkId { get; set; }
        public List<string> SmartKeys { get; set; }
    }
}
