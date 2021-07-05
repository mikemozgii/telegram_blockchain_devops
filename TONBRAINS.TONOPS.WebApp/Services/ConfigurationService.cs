using TONBRAINS.TONOPS.WebApp.Common.Models;
using SqlKata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;

namespace TONBRAINS.TONOPS.WebApp.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private void AddWithReplace(List<Configuration> collection, Configuration item)
        {
            collection.RemoveAll(a => a.Type == item.Type && a.OverrideKey == item.OverrideKey);
            collection.Add(item);
        }

        private async Task<IEnumerable<ConfigurationLinkModel>> GetAllConfigurationLinks()
        {
            return await ExecuteQuery<ConfigurationLinkModel>(
                new Query("configurations_links")
                    .Join("configurations", x => x.On("configurations.id", "configurations_links.configuration_id"))
                    .Select(new string[] { "configurations.id", "configurations.type", "configurations.value", "configurations.name", "configurations.override_key", "configurations_links.id as link_id" })
            );
        }

        public async Task<IEnumerable<Configuration>> GetModuleConfigurations(string moduleId)
        {
            var module = await ExecuteQueryFirst<ModuleModel>(
                new Query("modules").Where("id", moduleId)
            );
            var links = await GetAllConfigurationLinks();
            var ecosystemItems = links.Where(a => a.LinkId == module.Ecosystem);
            var moduleTypeItems = links.Where(a => a.LinkId == module.Name + module.Ecosystem);
            var moduleItems = links.Where(a => a.LinkId == module.Id);

            var collection = new List<Configuration>(ecosystemItems);

            foreach (var item in moduleTypeItems.Concat(moduleItems)) AddWithReplace(collection, item);

            return collection;
        }

    }

}
