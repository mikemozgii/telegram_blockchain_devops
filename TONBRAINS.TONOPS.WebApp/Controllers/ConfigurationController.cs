using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Models.Configurations;
using TONBRAINS.TONOPS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/configurations")]
    [ApiController]
    public class ConfigurationController : SessionController
    {
        private readonly ISshStream _sshService;

        public ConfigurationController(ISshStream sshService)
        {
            _sshService = sshService ?? throw new ArgumentNullException(nameof(sshService));
        }

        [Route("types")]
        [HttpGet]
        public IEnumerable<SelectListItem<int>> ConfigurationTypes() => EnumHelpers<ConfigurationTypes>.GetEnumSelectList();

        [Route("all")]
        [HttpGet]
        public async Task<IEnumerable<Configuration>> GetAll()
            => await ExecuteQuery<Configuration>(new Query("configurations"));

        [Route("single")]
        [HttpGet]
        public async Task<Configuration> Get(string id)
            => (await ExecuteQuery<Configuration>(new Query("configurations").Where("id", id))).FirstOrDefault();

        [Route("allbytype")]
        [HttpGet]
        public async Task<IEnumerable<Configuration>> GetAllByType(ConfigurationTypes type)
            => await ExecuteQuery<Configuration>(new Query("configurations").Where("type", (int)type));

        [Route("links")]
        [HttpGet]
        public async Task<IEnumerable<Configuration>> GetLinks(string id)
            => await ExecuteQuery<Configuration>(new Query("configurations_links")
                .Join("configurations", x=>x.On("configurations.id", "configurations_links.configuration_id"))
                .Where("configurations_links.id", id)
                .Select(new string[] { "configurations.id", "configurations.type", "configurations.value" }));

        [Route("linksbytype")]
        [HttpGet]
        public async Task<IEnumerable<Configuration>> GetLinksByType(string id, ConfigurationTypes type)
            => await ExecuteQuery<Configuration>(new Query("configurations_links")
                .Join("configurations", x => x.On("configurations.id", "configurations_links.configuration_id"))
                .Where("configurations_links.id", id)
                .Select(new string[] { "configurations.id", "configurations.type", "configurations.value" })
                .Where("configurations.type", (int)type));

        [Route("deleteconfiguration")]
        [HttpDelete]
        public async Task<bool> DeleteConfiguration(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("configurations").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }

        [Route("deletelink")]
        [HttpDelete]
        public async Task<bool> Deletelink(string id, string configurationId)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("configurations_links")
                 .Where("id", id).Where("configuration_id", configurationId).AsDelete());

                result = false;
            }
            catch
            {

            }
            return result;
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<Configuration> AddOrEdit([FromBody]Configuration item)
        {
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = Guid.NewGuid().ToString();
            var configuration = await ExecuteInsertOrUpdate(item, new Query("configurations"), insert);
            return configuration;
        }
        private async Task<IEnumerable<ConfigurationLinkModel>> GetAllConfigurationLinks()
        {
            return await ExecuteQuery<ConfigurationLinkModel>(
                new Query("configurations_links")
                    .Join("configurations", x => x.On("configurations.id", "configurations_links.configuration_id"))
                    .Select(new string[] { "configurations.id", "configurations.type", "configurations.value", "configurations.name", "configurations.override_key", "configurations_links.id as link_id" })
            );
        }

        private void AddWithReplace(List<Configuration> collection, Configuration item)
        {
            collection.RemoveAll(a => a.Type == item.Type && a.OverrideKey == item.OverrideKey);
            collection.Add(item);
        }

        private async Task<(IEnumerable<ServiceItemModel>, IEnumerable<ConfigurationLinkModel>)> GetServices()
        {
            var ecosystems = await ExecuteQuery<EcosystemModel>(new Query("ecosystems"));
            var modules = await ExecuteQueryJoin<ModuleLoggingModel, LoggingModel>(
                new Query("modules")
                    .LeftJoin("modules_logging", x => x.On("modules_logging.id", "modules.id")),
                    "Logging"
            );
            var environments = await ExecuteQuery<EnvironmentModel>(new Query("environments"));
            var links = await GetAllConfigurationLinks();

            var result = new List<ServiceItemModel>();
            foreach (var module in modules)
            {
                var serviceItem = new ServiceItemModel
                {
                    Id = module.Id,
                    Name = module.Name,
                    Url = module.Url,
                    WebUrl = module.WebUrl
                };

                var ecosystemItems = links.Where(a => a.LinkId == module.Ecosystem);
                var moduleTypeItems = links.Where(a => a.LinkId == module.Name + module.Ecosystem);
                var moduleItems = links.Where(a => a.LinkId == module.Id);

                var collection = new List<Configuration>(ecosystemItems);

                foreach (var item in moduleTypeItems.Concat(moduleItems)) AddWithReplace(collection, item);

                serviceItem.Items = collection
                        .Select(
                            a => new ConfigationServiceModel
                            {
                                Id = a.Id,
                                Value = a.Value,
                                ValueType = (int)a.Type,
                                OverrideKey = a.OverrideKey
                            }
                        )
                        .ToList();

                result.Add(serviceItem);
            }

            return (result, links);
        }

        [Route("addlink")]
        [HttpGet]
        public async Task<bool> AddLink(string id, string configurationId)
        {
            var result = false;
            try
            {
                await ExecuteQuery(new Query("configurations_links").AsInsert(new { id, configuration_id = configurationId }));

                result = true;
            }
            catch
            {
            }
            return result;
        }

        [Route("addlinks")]
        [HttpPost]
        public async Task<bool> AddLinks([FromBody]Links item)
        {
            var result = false;
            var exists = (await GetLinks(item.Id))
                .Select(x=>x.Id)
                .ToArray();

            var deleted = exists.Except(item.Configurations);

            var inserts = item.Configurations.Except(exists)
                .Select(x => new object []{ item.Id, x })
                .ToArray();

            try
            {
                if(inserts.Any())
                    await ExecuteQuery(new Query("configurations_links").AsInsert(new string[] {"id", "configuration_id" }, inserts));
                if(deleted.Any())
                    await ExecuteQuery(new Query("configurations_links").Where("id", item.Id).WhereIn("configuration_id", deleted).AsDelete());

                result = true;
            }
            catch
            {
            }
            return result;
        }
    }
}
