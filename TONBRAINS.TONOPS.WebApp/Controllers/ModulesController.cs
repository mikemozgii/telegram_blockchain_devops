using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.Models.Modules;
using TONBRAINS.TONOPS.WebApp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.Core.Handlers;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using TONBRAINS.TONOPS.WebApp.Services;
using Npgsql;
using TONBRAINS.TONOPS.Core.SSH;
using TONBRAINS.TONOPS.WebApp.Models.Sessions;
using TONBRAINS.TONOPS.WebApp.WebApp.Models;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;
using System.Net.Http;
using TONBRAINS.TONOPS.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/modules")]
    [ApiController]
    public class ModulesController : SessionController
    {
        private readonly ISshStream _sshService;
        private readonly IConfigurationService _configurationService;

        public ModulesController(ISshStream sshService, IConfigurationService configurationService)
        {
            _sshService = sshService ?? throw new ArgumentNullException(nameof(sshService));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        }

        private Stream GenerateServiceFile(string pathToService, string serviceExecutableName, string moduleDescription)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("[Unit]\n");
            writer.Write($"Description = {moduleDescription}\n\n");

            writer.Write("[Service]\n");
            writer.Write("Type=notify\n");
            writer.Write($"WorkingDirectory={pathToService}\n");
            writer.Write($"ExecStart={pathToService}/{serviceExecutableName}\n");
            writer.Write($"SyslogIdentifier={serviceExecutableName}\n"); //?????
            writer.Write("User=root\n");
            writer.Write("RestartSec=5\n\n");

            writer.Write("[Install]\n");
            writer.Write("WantedBy=multi-user.target\n");

            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        private Stream GeneratePrimaryInstallBash(string pathToService, string serviceExecutableName)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write($"#!/bin/bash\n");
            writer.Write($"unzip -d{pathToService} {pathToService}/service.zip\n"); //unpack archive with service
            writer.Write($"rm {pathToService}/service.zip\n"); // remove archive
            writer.Write($"chmod +x {pathToService}/{serviceExecutableName}"); // set executable attribute for binary file

            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        private Stream GenerateRegisterAndStartService(string moduleName, string moduleId)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write($"#!/bin/bash\n");
            writer.Write($"systemctl enable {moduleName}{moduleId}\n"); //enable service
            writer.Write($"systemctl start {moduleName}{moduleId}\n"); //start service

            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        private Stream GenerateServiceJson(string moduleId, string WebAppAddress)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(
                JsonConvert.SerializeObject(
                    new ServiceSettingsModel
                    {
                        ModuleId = moduleId,
                        WebAppAddress = WebAppAddress
                    }
                )
            );

            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        private string GetVersionFolderPath() => Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "../versions");

        [Route("grid")]
        public async Task<IEnumerable<ModuleModel>> Modules(string nodeId = default, string moduleName = default, string ecosystemId = default)
        {
            var modulesQuery = new Query("modules");
            if (!string.IsNullOrEmpty(nodeId))
            {
                var node = (await ExecuteQuery<NodeModel>(new Query("nodes").Where("id", nodeId))).FirstOrDefault();
                if (node == null) return Enumerable.Empty<ModuleModel>();

                var modulesIds = JsonConvert.DeserializeObject<IEnumerable<string>>(node.Modules);
                modulesQuery.WhereIn("modules.id", modulesIds);
            }

            if (!string.IsNullOrEmpty(moduleName)) modulesQuery.Where("name", moduleName);
            if (!string.IsNullOrEmpty(ecosystemId)) modulesQuery.Where("ecosystem", ecosystemId);

            modulesQuery.LeftJoin("modules_logging", x => x.On("modules_logging.id", "modules.id"));

            var modules = await ExecuteQueryJoin<ModuleLoggingModel, LoggingModel>(modulesQuery, "Logging");

            return modules;
        }

        [Route("moduletypes")]
        public async Task<IEnumerable<object>> ModuleTypes(string ecosystemId = default)
        {
            if (!string.IsNullOrEmpty(ecosystemId))
            {
                return await ExecuteSql<ModuleTypeModel>($"SELECT * FROM module_types WHERE ecosystems @> '[\"{ecosystemId}\"]'", Session.AccountId);
            }
            else
            {
                return await ExecuteQuery<ModuleTypeModel>(new Query("module_types"), Session.AccountId);
            }
        }

        [Route("nodes")]
        public async Task<IEnumerable<NodeModel>> ModuleNodes(string moduleId)
        {
            var sql = QueryToSql(new Query("nodes")) + $" WHERE modules @> '[\"{moduleId}\"]'";
            return await ExecuteSql<NodeModel>(sql, Session.AccountId);
        }

        [Route("moduleenvironments")]
        public async Task<IEnumerable<object>> ModuleEnvironmentsWithModels(string moduleId) => await ExecuteSql<EnvironmentModel>($"SELECT * FROM environments WHERE modules @> '[\"{moduleId}\"]'", Session.AccountId);

        [Route("single")]
        public async Task<ModuleLoggingModel> Module(string id)
        {
            var modulesQuery = new Query("modules")
               .LeftJoin("modules_logging", x => x.On("modules_logging.id", "modules.id"))
               .Where("modules.id", id);
            return (await ExecuteQueryJoin<ModuleLoggingModel, LoggingModel>(modulesQuery, "Logging")).FirstOrDefault();
        }

        [Route("environments")]
        public async Task<IEnumerable<string>> ModuleEnvironments(string id)
        {
            var environments = await ExecuteSql<EnvironmentModel>("SELECT * FROM environments WHERE modules @> '[\"" + id + "\"]'");

            return environments.Select(a => a.Id).ToList();
        }

        [Route("saveenvironments")]
        [HttpPost]
        public async Task<bool> SaveModuleEnvironments([FromBody] SaveModuleEnvironmentModel model)
        {
            var environments = await ExecuteSql<EnvironmentModel>("SELECT * FROM environments WHERE modules @> '[\"" + model.Id + "\"]'");

            var deletedEnvironments = environments.Where(a => !model.Environments.Contains(a.Id));
            foreach (var deletedEnvironment in deletedEnvironments)
            {
                var modules = JsonConvert.DeserializeObject<IEnumerable<string>>(deletedEnvironment.Modules);
                deletedEnvironment.Modules = JsonConvert.SerializeObject(modules.Where(a => a != model.Id));
                await ExecuteInsertOrUpdate(deletedEnvironment, new Query("environments"), insert: false);
            }

            var newEnvironmentIds = model.Environments
                .Where(a => !environments.Any(b => b.Id == a))
                .ToList();
            var newEnvironments = await ExecuteQuery<EnvironmentModel>(new Query("environments").WhereIn("id", newEnvironmentIds));
            foreach (var newEnvironment in newEnvironments)
            {
                var modules = JsonConvert.DeserializeObject<List<string>>(newEnvironment.Modules);
                modules.Add(model.Id);
                newEnvironment.Modules = JsonConvert.SerializeObject(modules);
                await ExecuteInsertOrUpdate(newEnvironment, new Query("environments"), insert: false);
            }

            return true;
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<ModuleModel> AddOrEdit([FromBody] ModuleLoggingModel item)
        {
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = IdGenerator.Generate();
            var queryModules = new Query("modules");

            var module = new
            {
                id = item.Id,
                name = item.Name,
                description = item.Description,
                url = item.Url,
                installed_version = item.InstalledVersion,
                web_url = item.WebUrl,
                configuration_nginx = item.ConfigurationNginx ?? "",
                configuration_nginx_web = item.ConfigurationNginxWeb ?? "",
                ecosystem = item.Ecosystem
            };
            var query = new StringBuilder();
            query.Append(QueryToSql(insert ? queryModules.AsInsert(module) : queryModules.Where("id", item.Id).AsUpdate(module)));
            query.Append(";");
            if (item.Logging != null)
            {
                var queryLogging = new Query("modules_logging");

                var loggingItem = (await ExecuteQuery<LoggingModel>(queryLogging.Where("id", item.Id))).FirstOrDefault();

                var insertLogging = loggingItem == null;

                var logging = new
                {
                    id = item.Id,
                    active = item.Logging.Active,
                    log_level = item.Logging.LogLevel
                };

                query.Append(QueryToSql(insertLogging ? queryLogging.AsInsert(logging) : queryLogging.Where("id", item.Id).AsUpdate(logging)));
                query.Append(";");
            }
            query.Append(@$"
SELECT * 
FROM modules
LEFT JOIN modules_logging ON modules_logging.id = modules.id
WHERE modules.id = '{item.Id}';");

            var savedItem = (await ExecuteJoinSql<ModuleLoggingModel, LoggingModel>(query.ToString(), "Logging")).FirstOrDefault();
            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var session = Session;
            var result = false;

            try
            {
                var module = await ExecuteQueryFirst<ModuleModel>(
                    new Query("modules").Where("id", id),
                    ApplicationName: session.AccountId
                );

                await DeleteModuleFromNode(session, module);

                await ExecuteQuery(new Query("modules").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }

        [Route("multidelete")]
        [HttpDelete]
        public async Task<bool> MultiDelete(IEnumerable<string> ids = default, string moduleType = default, string ecosystem = default)
        {
            var session = Session;
            var result = false;

            var query = new Query("modules");

            if (ids != null) query.WhereIn("id", ids);
            if (moduleType != null) query.Where("name", moduleType);
            if (ecosystem != null) query.WhereIn("ecosystem", ecosystem);

            var deletedModules = await ExecuteQuery<ModuleModel>(query);

            foreach (var deletedModule in deletedModules)
            {
                await DeleteModuleFromNode(session, deletedModule);
            }

            try
            {
                await ExecuteQuery(query.AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }

        private static async Task DeleteModuleFromNode(AccountSession session, ModuleModel deletedModule)
        {
            if (string.IsNullOrEmpty(deletedModule.InstalledVersion)) return;

            var node = await ExecuteSqlFirst<NodeWithCredentialsModel>(
                QueryToSql(new Query("nodes").Join("credentials", "credentials.id", "nodes.credential_id")) + " WHERE nodes.modules @> '[\"" + deletedModule.Id + "\"]'",
                ApplicationName: session.AccountId
            );

            var nodeAuth = new SSHAuthMdl(node.Ip, node.UserName, node.Password);
            var commands = new List<string>
                {
                    $"systemctl stop {deletedModule.Name}{deletedModule.Id}",
                    $"rm -rf /root/{deletedModule.Id}/",
                    $"rm -rf /etc/systemd/system/{deletedModule.Name + deletedModule.Id}.service"
                };
            using (var stream = new SSHBash().GenerateBashStream(commands))
            {
                new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
            }
        }

        //[Route("installversion")]
        //[HttpGet]
        //public async Task<DeployVersionModel> InstallModuleToNode(string nodeId, string moduleId, string version)
        //{
        //    var node = (await ExecuteQuery<NodeModel>(new Query("nodes").Where("id", nodeId))).FirstOrDefault();
        //    var module = (await Module(moduleId));

        //    var configurations = await _configurationService.GetModuleConfigurations(module.Id);
        //    var WebAppConfiguration = configurations.FirstOrDefault(a => a.Type == ConfigurationTypes.WebAppAddress);

        //    var versionsQuery = new Query("module_versions")
        //        .Where("module", module.Name)
        //        .OrderByDesc("version");
        //    if (!string.IsNullOrEmpty(version)) versionsQuery.Where("version", version);

        //    var moduleVersions = await ExecuteQuery<ModuleVersionModel>(versionsQuery);

        //    if (!moduleVersions.Any()) return new DeployVersionModel { Message = "No actual module version", Result = false };

        //    var moduleType = (await ExecuteQuery<ModuleTypeModel>(new Query("module_types").Where("id", module.Name))).FirstOrDefault();
        //    if (moduleType == null) return new DeployVersionModel { Message = "No actual module type", Result = false };

        //    var moduleVersion = moduleVersions.First();
        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).FirstOrDefault();
        //    using var client = _sshService.GetSshClient(new ServerTemplate
        //    {
        //        Host = node.Ip,
        //        Password = credential.Password,
        //        Port = 22,
        //        User = credential.UserName
        //    });

        //    var filePath = "";
        //    if (string.IsNullOrEmpty(moduleVersion.File))
        //    {
        //        filePath = Path.Combine(GetVersionFolderPath(), moduleVersion.Module + moduleVersion.Version);
        //    }
        //    else
        //    {
        //        var fileModel = await ExecuteQueryFirst<FileModel>(new Query("files").Where("id", moduleVersion.File));
        //        var file = await GetFileAsync(fileModel.Oid, ConnectionString);
        //        filePath = Path.GetTempFileName();
        //        await System.IO.File.WriteAllBytesAsync(filePath, file);
        //    }

        //    client.Connect();
        //    using (var file = System.IO.File.OpenRead(filePath))
        //    {
        //        await _sshService.ExecuteCommands(client, new string[]
        //        {
        //              "mkdir /root/" + module.Id
        //        });
        //        await _sshService.UploadFileToHost(client.ConnectionInfo, file, "/root/" + module.Id + "/service.zip");
        //        await _sshService.ExecuteBashOnHost(client, GeneratePrimaryInstallBash("/root/" + module.Id, moduleType.Execute));
        //        await _sshService.UploadFileToHost(client.ConnectionInfo, GenerateServiceFile("/root/" + module.Id, moduleType.Execute, module.Description), $"/etc/systemd/system/{module.Name + module.Id}.service");
        //        await _sshService.UploadFileToHost(client.ConnectionInfo, GenerateServiceJson(module.Id, WebAppConfiguration.Value), $"/root/{module.Id}/service.json");
        //        await _sshService.ExecuteBashOnHost(client, GenerateRegisterAndStartService(module.Name, module.Id));
        //    }
        //    client.Disconnect();

        //    if (!string.IsNullOrEmpty(moduleVersion.File)) System.IO.File.Delete(filePath);

        //    module.InstalledVersion = moduleVersion.Version;
        //    await ExecuteInsertOrUpdate<object>(
        //        module,
        //        new Query("modules"),
        //        insert: false,
        //        touchedFields: new List<string> { nameof(ModuleModel.InstalledVersion) }
        //    );

        //    return new DeployVersionModel();
        //}

        //[Route("updateversion")]
        //[HttpGet]
        //public async Task<DeployVersionModel> UpdateModuleToNode(string nodeId, string moduleId, string version)
        //{
        //    var node = (await ExecuteQuery<NodeModel>(new Query("nodes").Where("id", nodeId))).FirstOrDefault();
        //    var module = await Module(moduleId);
        //    var moduleVersion = (await ExecuteQuery<ModuleVersionModel>(new Query("module_versions").Where("id", version))).FirstOrDefault();

        //    var configurations = await _configurationService.GetModuleConfigurations(module.Id);
        //    var WebAppConfiguration = configurations.FirstOrDefault(a => a.Type == ConfigurationTypes.WebAppAddress);

        //    var moduleType = (await ExecuteQuery<ModuleTypeModel>(new Query("module_types").Where("id", module.Name))).FirstOrDefault();
        //    if (moduleType == null) return new DeployVersionModel { Message = "No actual module type", Result = false };

        //    var credential = (await ExecuteQuery<Credential>(new Query("credentials").Where("id", node.CredentialId))).FirstOrDefault();
        //    using var client = _sshService.GetSshClient(new ServerTemplate
        //    {
        //        Host = node.Ip,
        //        Password = credential.Password,
        //        Port = 22,
        //        User = credential.UserName
        //    });

        //    var filePath = "";
        //    if (string.IsNullOrEmpty(moduleVersion.File))
        //    {
        //        filePath = Path.Combine(GetVersionFolderPath(), moduleVersion.Module + moduleVersion.Version);
        //    }
        //    else
        //    {
        //        var fileModel = await ExecuteQueryFirst<FileModel>(new Query("files").Where("id", moduleVersion.File));
        //        var file = await GetFileAsync(fileModel.Oid, ConnectionString);
        //        filePath = Path.GetTempFileName();
        //        await System.IO.File.WriteAllBytesAsync(filePath, file);
        //    }

        //    client.Connect();
        //    using (var file = System.IO.File.OpenRead(filePath))
        //    {
        //        await _sshService.ExecuteCommands(client, new string[]
        //        {
        //            $"systemctl stop {module.Name}{module.Id}",
        //            "rm -rf /root/" + module.Id + "/*"
        //        });
        //        await _sshService.UploadFileToHost(client.ConnectionInfo, file, "/root/" + module.Id + "/service.zip");
        //        await _sshService.ExecuteCommands(client, new string[]
        //        {
        //           $"unzip -d{("/root/" + module.Id)} /root/" + module.Id + "/service.zip"
        //        });

        //        await _sshService.UploadFileToHost(client.ConnectionInfo, GenerateServiceJson(module.Id, WebAppConfiguration.Value), $"/root/{module.Id}/service.json");
        //        await _sshService.ExecuteCommands(client, new string[]
        //        {
        //             $"chmod +x {("/root/" + module.Id)}/{moduleType.Execute}",
        //             $"systemctl start {module.Name}{module.Id}",
        //             "rm /root/" + module.Id + "/service.zip"
        //        });
        //    }
        //    client.Disconnect();

        //    if (!string.IsNullOrEmpty(moduleVersion.File)) System.IO.File.Delete(filePath);

        //    module.InstalledVersion = moduleVersion.Version;

        //    await ExecuteInsertOrUpdate<object>(
        //        module,
        //        new Query("modules"),
        //        insert: false,
        //        touchedFields: new List<string> { nameof(ModuleModel.InstalledVersion) }
        //    );

        //    return new DeployVersionModel();
        //}

        [Route("updatelogging")]
        [HttpGet]
        public async Task<ModuleModel> UpdateLogging(string moduleId, bool logging)
        {
            var updateQuery = new Query("modules_logging")
                            .Where("id", moduleId)
                            .AsUpdate(new string[] { "active" }, new object[] { logging });
            await ExecuteQuery(updateQuery);

            return await Module(moduleId);
        }

        [Route("getlogginglevels")]
        [HttpGet]
        public object GetLoggingLevel()
            => new
            {
                Items = EnumHelpers<MinimumLogLevelTypes>.GetEnumSelectList()
            };

        [Route("modulecount")]
        public async Task<IEnumerable<ModuleTypeCount>> ModuleCount(string ecosystemId)
        {
            return await ExecuteSql<ModuleTypeCount>(
                $"SELECT module_types.id, COUNT(*) FROM module_types JOIN modules ON modules.name = module_types.id WHERE modules.ecosystem='{ecosystemId}' GROUP BY module_types.id",
                Session.AccountId
            );
        }

        [Route("environmentcount")]
        public async Task<IEnumerable<object>> EnvironmentCount()
        {
            return await ExecuteSql<ModuleTypeCount>(
                $"SELECT modules.id, (SELECT COUNT(*) FROM environments WHERE modules @> CONCAT('[\"', modules.id, '\"]')::jsonb) AS count FROM modules",
                Session.AccountId
            );
        }

        [Route("environmentecosystemcount")]
        public async Task<int> EnvironmentEcosystemCount(string ecosystemId)
        {
            var modules = await ExecuteQuery<ModuleModel>(
                new Query("modules").Where("ecosystem", ecosystemId),
                Session.AccountId
            );
            var modulesIds = modules.Select(a => a.Id);
            var envorinments = await ExecuteQuery<EnvironmentModel>(
                new Query("environments"),
                Session.AccountId
            );
            return envorinments
                .Where(a => modulesIds.Any(moduleId => a.Modules.Contains(moduleId)))
                .Count();
        }

        public async Task<byte[]> GetFileAsync(uint oid, string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var fileManager = new NpgsqlLargeObjectManager(connection);
                using (connection.BeginTransaction())
                using (var memoryStream = new MemoryStream())
                using (var dbStream = await fileManager.OpenReadAsync(oid, default))
                {
                    var buffer = new byte[262144];

                    while (memoryStream.Position < dbStream.Length)
                    {
                        var count = await dbStream.ReadAsync(buffer, 0, buffer.Length, default);
                        await memoryStream.WriteAsync(buffer, 0, count, default);
                    }

                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }

        [Route("startservice")]
        public async Task<bool> StartService(string moduleId)
        {
            var module = await ExecuteSqlFirst<ModuleModel>(QueryToSql(new Query("modules").Where("id", moduleId)), Session.AccountId);
            var node = await ExecuteSqlFirst<NodeWithCredentialsModel>(
                QueryToSql(new Query("nodes").Join("credentials", "credentials.id", "nodes.credential_id")) + " WHERE nodes.modules @> '[\"" + moduleId + "\"]'",
                ApplicationName: Session.AccountId
            );

            var nodeAuth = new SSHAuthMdl(node.Ip, node.UserName, node.Password);
            var commands = new List<string>
                {
                    $"systemctl start {module.Name}{module.Id}",
                };
            using (var stream = new SSHBash().GenerateBashStream(commands))
            {
                new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
            }

            return true;
        }

        [Route("stopservice")]
        public async Task<bool> StopService(string moduleId)
        {
            var module = await ExecuteSqlFirst<ModuleModel>(QueryToSql(new Query("modules").Where("id", moduleId)), Session.AccountId);
            var node = await ExecuteSqlFirst<NodeWithCredentialsModel>(
                QueryToSql(new Query("nodes").Join("credentials", "credentials.id", "nodes.credential_id")) + " WHERE nodes.modules @> '[\"" + moduleId + "\"]'",
                ApplicationName: Session.AccountId
            );

            var nodeAuth = new SSHAuthMdl(node.Ip, node.UserName, node.Password);
            var commands = new List<string>
                {
                    $"systemctl stop {module.Name}{module.Id}",
                };
            using (var stream = new SSHBash().GenerateBashStream(commands))
            {
                new SSHSvc().ExecuteBashOnHost(nodeAuth, stream);
            }

            return true;
        }

        [Route("liveservice")]
        public async Task<bool> LiveService(string moduleId)
        {
            var module = await ExecuteSqlFirst<ModuleModel>(QueryToSql(new Query("modules").Where("id", moduleId)), Session.AccountId);
            var node = await ExecuteSqlFirst<NodeWithCredentialsModel>(
                QueryToSql(new Query("nodes").Join("credentials", "credentials.id", "nodes.credential_id")) + " WHERE nodes.modules @> '[\"" + moduleId + "\"]'",
                ApplicationName: Session.AccountId
            );

            if (string.IsNullOrEmpty(module.InstalledVersion)) return false;

            var nodeAuth = new SSHAuthMdl(node.Ip, node.UserName, node.Password);
            var commands = new List<string>
                {
                    $"systemctl status {module.Name}{module.Id}",
                };
            using (var stream = new SSHBash().GenerateBashStream(commands))
            {
                var result = new SSHSvc().ExecuteCommandsWithResult(nodeAuth, commands);
                return result.Any(a => a.Contains("Active: active (running)"));
            }
        }

        [Route("forcerefreshconfiguration")]
        public async Task<bool> ForceRefreshConfiguration(string moduleId)
        {
            var module = await ExecuteSqlFirst<ModuleModel>(QueryToSql(new Query("modules").Where("id", moduleId)), Session.AccountId);
            if (string.IsNullOrEmpty(module.InstalledVersion)) return false;

            string endpoint;
            if (string.IsNullOrEmpty(module.WebUrl))
            {
                var url = new Uri(module.Url);
                endpoint = module.Url.Replace(url.Port.ToString(), (url.Port + 1).ToString());
            } else
            {
                endpoint = module.WebUrl;
            }

            var httpClient = new HttpClient();
            var result = await httpClient.GetStringAsync(endpoint);

            return result == "ok";
        }

    }

}
