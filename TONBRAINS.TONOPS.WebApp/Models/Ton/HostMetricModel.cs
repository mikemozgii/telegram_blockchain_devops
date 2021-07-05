using System.Collections.Generic;

namespace TONBRAINS.TONOPS.WebApp.Models.Ton
{
    public class HostMetricModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<object> Nodes { get; set; }
    }
}
