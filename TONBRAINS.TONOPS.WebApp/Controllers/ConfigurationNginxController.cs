using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using TONBRAINS.TONOPS.WebApp.Services;
using TONBRAINS.TONOPS.WebApp;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/configurationnginx")]
    [ApiController]
    public class ConfigurationNginxController : ControllerBase
    {
        private readonly ISshStream _sshService;

        public ConfigurationNginxController(ISshStream sshService)
        {
            _sshService = sshService ?? throw new ArgumentNullException(nameof(sshService));
        }


        [Route("grid")]
        public async Task<IEnumerable<ConfigurationNginxModel>> Modules(string nodeId = default)
        {
            var query = new Query("configuration_nginx");

            if (!string.IsNullOrEmpty(nodeId)) query.Where("node_id", nodeId);

            return await ExecuteQuery<ConfigurationNginxModel>(query);
        }

        [Route("moduletype")]
        public async Task<IEnumerable<ConfigurationNginxModuleTypeModel>> ModuleType(string ecosystem)
        {
            var moduleTypes = await ExecuteSql<ModuleTypeModel>(
                QueryToSql(new Query("module_types")) + " WHERE ecosystems  @> '[\"" + ecosystem + "\"]'"
            );

            var modules = await ExecuteQuery<ModuleModel>(
                new Query("modules")
                    .Where("modules.ecosystem", ecosystem)
            );

            var result = new List<ConfigurationNginxModuleTypeModel>();
            foreach (var moduleType in moduleTypes)
            {
                result.Add(await GetModuleTypes(moduleType.Id, ecosystem, modules));
            }

            return result;
        }

        public async Task<ConfigurationNginxModuleTypeModel> GetModuleTypes(string moduleType, string ecosystem, IEnumerable<ModuleModel> modules)
        {
            var result = new ConfigurationNginxModuleTypeModel
            {
                ModuleTypeId = moduleType,
                External = null,
                Internal = null,
            };

            var module = modules.FirstOrDefault(a => a.Ecosystem == ecosystem && a.Name == moduleType);
            if (module == null) return result;

            var query = new Query("configuration_nginx")
                .Select("configuration_nginx.*", "nodes.ip")
                .Join("nodes", "nodes.id", "configuration_nginx.node_id")
                .Where(a => a.Where("configuration_nginx.id", module.ConfigurationNginx).OrWhere("configuration_nginx.id", module.ConfigurationNginxWeb));

            var configurations = await ExecuteQuery<ConfigurationNginxWithIpModel>(query);

            result.External = configurations.FirstOrDefault(a => a.Id == module.ConfigurationNginxWeb);
            result.Internal = configurations.FirstOrDefault(a => a.Id == module.ConfigurationNginx);

            return result;
        }

        [Route("settomoduletype")]
        public async Task SetToModuleType(string moduleType, string ecosystem, string id, string webId)
        {
            var query = new Query("modules")
                .Where("name", moduleType)
                .Where("ecosystem", ecosystem)
                .AsUpdate(new List<string> { "configuration_nginx", "configuration_nginx_web" }, new List<string> { id ?? "", webId ?? "" });

            await ExecuteSql(QueryToSql(query));
        }

        [Route("single")]
        public async Task<ConfigurationNginxModel> Module(string id) => (await ExecuteQuery<ConfigurationNginxModel>(new Query("configuration_nginx").Where("id", id))).FirstOrDefault();

        [Route("addoredit")]
        [HttpPost]
        public async Task<ConfigurationNginxModel> AddOrEdit([FromBody] ConfigurationNginxModel item)
        {
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = IdGenerator.Generate();
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("configuration_nginx"), insert);

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("configuration_nginx").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }

        [Route("generatenginxconfig")]
        public async Task<bool> GenerateNginxConfigs()
        {
            var modules = await ExecuteQuery<ModuleModel>(new Query("modules"));
            var nodes = await ExecuteQuery<NodeModel>(new Query("nodes"));

            var configs = (await ExecuteQuery<ConfigurationNginxModel>(new Query("configuration_nginx")));
            foreach (var node in nodes.Where(a => configs.Any(b => b.NodeId == a.Id)))
            {
                await UploadNginxConfigToNode(node, GenerateNginxConfig(node, modules, configs));
            }

            return true;
        }

        private async Task UploadNginxConfigToNode(NodeModel node, (string, string) nginxConfig)
        {
            var credential = await ExecuteQueryFirst<Credential>(new Query("credentials").Where("id", node.CredentialId));

            var sshAuthorization = new ServerTemplate
            {
                Host = node.Ip,
                User = credential.UserName,
                Password = credential.Password,
                Port = 22
            };

            using (var stream = new MemoryStream())
            using (var streamWeb = new MemoryStream())
            using (var bashStream = new MemoryStream())
            using (var client = _sshService.GetSshClient(sshAuthorization))
            {
                client.Connect();

                stream.Write(Encoding.UTF8.GetBytes(nginxConfig.Item1));
                stream.Position = 0;
                await _sshService.UploadFileToHost(client.ConnectionInfo, stream, "/etc/nginx/conf.d/service.conf");

                streamWeb.Write(Encoding.UTF8.GetBytes(nginxConfig.Item2));
                streamWeb.Position = 0;
                await _sshService.UploadFileToHost(client.ConnectionInfo, streamWeb, "/etc/nginx/conf.d/web.conf");

                bashStream.Write(Encoding.UTF8.GetBytes("systemctl restart nginx"));
                bashStream.Position = 0;
                await _sshService.ExecuteBashOnHost(client, bashStream);

                client.Disconnect();
            }
        }

        private (string, string) GenerateNginxConfig(NodeModel node, IEnumerable<ModuleModel> modules, IEnumerable<ConfigurationNginxModel> configs)
        {
            var configurationNginxIds = configs
                .Where(a => a.NodeId == node.Id)
                .Select(a => a.Id)
                .ToList();
            var http2Modules = modules.Where(a => configurationNginxIds.Contains(a.ConfigurationNginx)).Select(a => new { Module = a, Config = configs.First(b => b.Id == a.ConfigurationNginx) });

            var builder = new StringBuilder();

            var moduleLookup = http2Modules.ToLookup(a => a.Config.Location.Replace("/", "p") + a.Config.Port);

            foreach (var group in moduleLookup)
            {
                builder.AppendLine($"   upstream http2{group.Key} {{");
                foreach (var item in group)
                {
                    var url = new Uri(item.Module.Url);
                    builder.AppendLine($"       server {url.Host}:{url.Port};");
                }
                builder.AppendLine("    }");

                var config = group.First().Config;
                builder.AppendLine("    server {");
                builder.AppendLine($"       listen {config.Port} http2;");
                builder.AppendLine($"       location {config.Location} {{");
                builder.AppendLine($"           grpc_pass grpc://http2{group.Key};");
                builder.AppendLine("       }");
                builder.AppendLine("    }");
            }

            var http2Result = builder.ToString();

            builder.Clear();

            var webModules = modules.Where(a => configurationNginxIds.Contains(a.ConfigurationNginxWeb)).Select(a => new { Module = a, Config = configs.First(b => b.Id == a.ConfigurationNginxWeb) });
            if (!webModules.Any()) return (http2Result, "");

            var webModuleLookup = webModules.ToLookup(a => a.Config.Location.Replace("/", "p") + a.Config.Port);
            foreach (var group in webModuleLookup)
            {
                builder.AppendLine($"   upstream web{group.Key} {{");
                foreach (var item in group)
                {
                    var url = new Uri(item.Module.Url);
                    builder.AppendLine($"       server {url.Host}:{url.Port};");
                }
                builder.AppendLine("    }");

                var config = group.First().Config;
                builder.AppendLine("    server {");
                builder.AppendLine($"       listen {config.Port};");
                builder.AppendLine($"       location {config.Location} {{");
                builder.AppendLine($"           proxy_pass http://web{group.Key};");
                builder.AppendLine("       }");
                builder.AppendLine("    }");
            }

            return (http2Result, builder.ToString());
        }
    }
}
