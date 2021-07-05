using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/external")]
    [ApiController]
    public class ExternalController : Controller
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

        private string GetNginxAddress(ConfigurationNginxModel configurationNginxModel, IEnumerable<NodeModel> nodes)
        {
            var node = nodes.FirstOrDefault(a => a.Id == configurationNginxModel.NodeId);
            return $"http://{node.Ip}:{configurationNginxModel.Port}{(configurationNginxModel.Location == "/" ? "" : configurationNginxModel.Location)}";
        }

        [HttpGet]
        [Route("getservices")]
        public async Task<IEnumerable<ServiceItemModel>> GetServices()
        {
            var ecosystems = await ExecuteQuery<EcosystemModel>(new Query("ecosystems"));
            var modules = await ExecuteQueryJoin<ModuleLoggingModel, LoggingModel>(
                new Query("modules")
                    .LeftJoin("modules_logging", x => x.On("modules_logging.id", "modules.id")),
                    "Logging"
            );
            var environments = await ExecuteQuery<EnvironmentModel>(new Query("environments"));
            var nginxConfigurations = await ExecuteQuery<ConfigurationNginxModel>(new Query("configuration_nginx"));
            var nodes = await ExecuteQuery<NodeModel>(new Query("nodes"));
            var links = await GetAllConfigurationLinks();

            var result = new List<ServiceItemModel>();
            foreach (var module in modules)
            {

                var serviceItemModel = new ServiceItemModel
                {
                    Id = module.Id,
                    Name = module.Name,
                    Url = string.IsNullOrEmpty(module.ConfigurationNginx) ? module.Url : GetNginxAddress(nginxConfigurations.First(a => a.Id == module.ConfigurationNginx), nodes),
                    WebUrl = string.IsNullOrEmpty(module.ConfigurationNginxWeb) ? module.WebUrl : GetNginxAddress(nginxConfigurations.First(a => a.Id == module.ConfigurationNginxWeb), nodes),
                    Logging = module.Logging != null ? new LoggingModel
                    {
                        Active = module.Logging.Active,
                        Id = module.Logging?.Id,
                        LogLevel = module.Logging.LogLevel
                    } : null
                };
                serviceItemModel.Environments = environments
                    .Where(a => a.Modules.Contains(module.Id))
                    .Select(a => a.Domain)
                    .ToList();

                var ecosystemItems = links.Where(a => a.LinkId == module.Ecosystem);
                var moduleTypeItems = links.Where(a => a.LinkId == module.Name + module.Ecosystem);
                var moduleItems = links.Where(a => a.LinkId == module.Id);

                var collection = new List<Configuration>(ecosystemItems);

                foreach (var item in moduleTypeItems.Concat(moduleItems)) AddWithReplace(collection, item);

                serviceItemModel.Items = collection
                    .Select(
                        a => new ConfigationServiceModel
                        {
                            Value = a.Value,
                            ValueType = (int)a.Type,
                            OverrideKey = a.OverrideKey
                        }
                    )
                    .ToList();

                result.Add(serviceItemModel);
            }

            return result;
        }

        [HttpGet]
        [Route("getenvironmentbyorganizationname")]
        public async Task<string> GetEnvironmentByOrganizationName(string organizationName)
        {
            var result = await ExecuteQuery<OrganizationEnvironmentModel>(new Query("organization_environments").Where("id", organizationName));
            var item = result.FirstOrDefault();
            return item?.Environment ?? "";
        }

        [HttpGet]
        [Route("setorganizationname")]
        public async Task<object> SetOrganizationName(string organizationName, string localId, string environment)
        {
            var existsInOther = await ExecuteQuery<OrganizationEnvironmentModel>(
                           new Query("organization_environments")
                               .Where("id", organizationName)
                               .Where("local_id", "<>", localId)
                       );
            if (existsInOther.Any()) return new { AlreadyExists = true };

            var existsOrganization = await ExecuteQuery<OrganizationEnvironmentModel>(
                new Query("organization_environments")
                    .Where("local_id", localId)
                    .Where("environment", environment)
            );

            await ExecuteInsertOrUpdate(
                new OrganizationEnvironmentModel
                {
                    LocalId = localId,
                    Environment = environment,
                    Id = organizationName
                },
                new Query("organization_environments"),
                insert: !existsOrganization.Any()
            );

            return new { AlreadyExists = false };
        }

        [HttpGet]
        [Route("checkenvironmentdomain")]
        public async Task<object> CheckEnvironmentDomain(string domain)
        {
            var environments = await ExecuteQuery<EnvironmentModel>(new Query("environments").Where("domain", domain));

            return new { Result = environments.Any() };
        }

        [HttpGet]
        [Route("checkorganizationname")]
        public async Task<object> CheckOrganizationName(string organizationName)
        {
            var result = await ExecuteQuery<OrganizationEnvironmentModel>(new Query("organization_environments").Where("id", organizationName));
            return new { Exists = result.Any() };
        }

    }

}
