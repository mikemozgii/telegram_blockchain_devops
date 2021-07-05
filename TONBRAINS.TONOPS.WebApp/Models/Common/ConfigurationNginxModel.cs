namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class ConfigurationNginxModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NodeId { get; set; }
        public string Location { get; set; }
        public int Port { get; set; }
        public bool Http2 { get; set; }

    }
}
