using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class Configuration
    {
        public string Id { get; set; }

        public ConfigurationTypes Type { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }

        public string OverrideKey { get; set; }
    }
}
