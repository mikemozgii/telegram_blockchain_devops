using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Common.Models.ConfigurationTemplate;
using TONBRAINS.TONOPS.WebApp.Services;
using TONBRAINS.TONOPS.WebApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/azuredevops")]
    [ApiController]
    public class AzureDevOpsController : Controller
    {
        private readonly ISshStream _sshService;

        public AzureDevOpsController(ISshStream sshService)
            => _sshService = sshService ?? throw new ArgumentNullException(nameof(sshService));

        private (VssConnection connection, BuildHttpClient client) GetVssConnection()
        {
            var collectionUri = "https://dev.azure.com/dealerpilot";
            var connection = new VssConnection(new Uri(collectionUri), new VssBasicCredential("pat", "asx674oid37kst5m7zmwbvp32tnkhdmkq7xa5ipbtbodcpk65i3q"));
            var buildClient = connection.GetClient<BuildHttpClient>();
            return (connection, buildClient);
        }

        private Stream BashForInstallFromGit(string serviceId, string version, string commitId, string modulePath)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write($"#!/bin/bash\n");
            var tempDir = Guid.NewGuid().ToString();
            writer.Write($"mkdir /root/{tempDir}\n");
            writer.Write($"cd /root/{tempDir}\n");
            writer.Write($"git clone https://rvladimirov:i7g6lbfl7downwvaialj5zrdugl6jwd4qelb5v3dyoyzcozj6lta@dev.azure.com/dealerpilot/Lotus/_git/lotusInit\n");
            writer.Write($"cd lotusInit\n");
            writer.Write($"git checkout {commitId}\n");
            writer.Write($"cd {modulePath}\n");
            writer.Write($"/snap/bin/dotnet-sdk.dotnet publish -o bin/publish --self-contained true -c Release -r linux-x64 -p:PublishSingleFile=true\n");
            writer.Write($"cd bin/publish/\n");
            writer.Write($"zip -r publish.zip .\n");
            writer.Write($"cp publish.zip /root/versions/{serviceId}{version}\n");
            writer.Write($"cd /root\n");
            writer.Write($"rm -rf {tempDir}\n");

            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        [HttpGet]
        [Route("getbuilds")]
        public async Task<IEnumerable<DevOpsReleaseModel>> GetBuilds()
        {
            var (connection, buildClient) = GetVssConnection();

            using (connection)
            {
                var builds = await buildClient.GetBuildsAsync(
                    new Guid("165e6bc4-846e-4bb6-9e27-f051f9a2aa3e"),
                    definitions: new List<int> { 10 },
                    resultFilter: BuildResult.Succeeded,
                    top: 15
                );

                return builds.Select(
                    a => new DevOpsReleaseModel
                    {
                        Name = a.BuildNumber,
                        Id = a.Id.ToString()
                    }
                );
            }
        }

        [HttpGet]
        [Route("addbuild")]
        public async Task<bool> AddBuilds(string releaseId, string serviceId, string description, string buildId)
        {
            var (connection, buildClient) = GetVssConnection();

            var commitId = "";

            using (connection)
            {
                var gitClient = connection.GetClient<GitHttpClient>();

                var build = (
                    await buildClient.GetBuildsAsync(
                        new Guid("165e6bc4-846e-4bb6-9e27-f051f9a2aa3e"),
                        buildIds: new List<int> { Convert.ToInt32(releaseId) }
                    )
                ).FirstOrDefault();
                commitId = build.SourceVersion;
            }

            //TODO: move to configuration
            var sshAuthorization = new ServerTemplate
            {
                Host = "172.17.1.92",
                User = "root",
                Password = "temp3232",
                Port = 22
            };
            
            var version = DateTime.UtcNow.ToString("yyyyMddHHmmss");

            var moduleType = (await ExecuteQuery<ModuleTypeModel>(new Query("module_types").Where("id", serviceId))).FirstOrDefault();
            if (moduleType == null) return false;
            using var client = _sshService.GetSshClient(sshAuthorization);
            client.Connect();
            await _sshService.ExecuteBashOnHost(client, BashForInstallFromGit(serviceId, version, commitId, moduleType.Path));
            client.Disconnect();
            await ExecuteInsertOrUpdate(
                new ModuleVersionModel
                {
                    Id = IdGenerator.Generate(),
                    Module = serviceId,
                    Version = version,
                    Description = description,
                    AzureBuildId = buildId
                },
                new Query("module_versions"),
                true
            );

            return true;
        }

    }
}
