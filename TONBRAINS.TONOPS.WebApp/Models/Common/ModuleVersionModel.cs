namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class ModuleVersionModel
    {
        public string Id { get; set; }
        public string Module { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string AzureBuildId { get; set; }
        public string File { get; set; }
    }
}
