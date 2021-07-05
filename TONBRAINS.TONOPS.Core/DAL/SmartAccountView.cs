using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TONBRAINS.TONOPS.Core.DAL
{
    public class SmartAccountView
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string TypeId { get; set; }
        public string Address { get; set; }
        public string ContractId { get; set; }
        public string ContractName { get; set; }
        public int CountKeys { get; set; }
        public string NetworksJson { get; set; }

        [NotMapped]
        public List<SmartAccountNetworkView> Networks
        {
            get
            {
                return NetworksJson == null ? new List<SmartAccountNetworkView>() :  JsonConvert.DeserializeObject<List<SmartAccountNetworkView>>(NetworksJson);
            }
        }

        [NotMapped]
        public IEnumerable<string> NetworkIds
        {
            get
            {
                return Networks.Select(i => i.Id);
            }
        }
    }
}
