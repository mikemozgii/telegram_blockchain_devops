namespace TONBRAINS.TONOPS.WebApp.Common.Models
{
    public class ConfigurationNginxModuleTypeModel
    {

        public string ModuleTypeId { get; set; }

        public ConfigurationNginxWithIpModel Internal { get; set; }

        public ConfigurationNginxWithIpModel External { get; set; }

    }
}
