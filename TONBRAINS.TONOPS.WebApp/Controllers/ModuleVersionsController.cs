using System.Collections.Generic;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Models.ModuleVersions;
using Microsoft.AspNetCore.Mvc;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using SqlKata;
using System.Linq;
using TONBRAINS.TONOPS.WebApp;
using System.IO;
using System.Diagnostics;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using Npgsql;
using System.Threading;
using NpgsqlTypes;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{

    [Route("api/moduleversions")]
    public class ModuleVersionsController : SessionController
    {

        [Route("grid")]
        public async Task<IEnumerable<ModuleVersionModel>> ModuleVersions(string filterByModule = "")
        {
            if (string.IsNullOrEmpty(filterByModule)) filterByModule = ""; //avoid null

            var query = new Query("module_versions");
            if (!string.IsNullOrEmpty(filterByModule)) query.Where("module", filterByModule);

            var modules = await ExecuteQuery<ModuleVersionModel>(query);

            return modules;
        }

        [Route("single")]
        public async Task<ModuleVersionModel> Module(string id)
        {
            var modules = await ExecuteQuery<ModuleVersionModel>(new Query("module_versions").Where("id", id));

            return modules.FirstOrDefault();
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<ModuleVersionModel> AddOrEdit([FromBody] ModuleVersionModel item)
        {
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = IdGenerator.Generate();
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("module_versions"), insert);

            return savedItem;
        }

        private string GetVersionFolderPath() => Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "../versions");

        [Route("uploadversion")]
        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<bool> UploadVersion([FromForm] UploadVersionModel uploadVersionModel)
        {
            using var inputStream = uploadVersionModel.VersionFile.OpenReadStream();

            var moduleVersion = await ExecuteQueryFirst<ModuleVersionModel>(new Query("module_versions").Where("id", uploadVersionModel.Id));

            var fileId = IdGenerator.Generate();
            await UploadFile(fileId, inputStream, ConnectionString);

            moduleVersion.File = fileId;
            await ExecuteInsertOrUpdate(moduleVersion, new Query("module_versions"), insert: false);

            return true;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("module_versions").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }

        [Route("moduletypescount")]
        public async Task<IEnumerable<ModuleTypeCount>> ModuleTypesCount()
        {
            return await ExecuteSql<ModuleTypeCount>(
                "SELECT module_types.id, COUNT(*) FROM module_types JOIN module_versions ON module_versions.module = module_types.id GROUP BY module_types.id",
                Session.AccountId
            );
        }

        [Route("moduletypeslastbuilds")]
        public async Task<IEnumerable<ModuleTypeBuild>> ModuleTypesLastBuilds()
        {
            return await ExecuteSql<ModuleTypeBuild>(
                "SELECT module_types.Id,(SELECT CASE WHEN version IS NOT NULL THEN version WHEN azure_build_id IS NOT NULL THEN azure_build_id END FROM module_versions WHERE module_versions.MODULE = module_types.Id ORDER BY version DESC, azure_build_id DESC LIMIT 1 ) AS build FROM module_types",
                Session.AccountId
            );
        }

        public async Task UploadFile(string id, Stream stream, string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var loManager = new NpgsqlLargeObjectManager(connection);
                    var oid = loManager.Create();
                    using (var dbStream = await loManager.OpenReadWriteAsync(oid, default(CancellationToken)))
                    {
                        var buffer = new byte[262144];
                        while (dbStream.Position < stream.Length)
                        {
                            var count = await stream.ReadAsync(buffer, 0, buffer.Length);
                            await dbStream.WriteAsync(buffer, 0, count);
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "insert into files (id, oid) values(@id, @oid)";
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@oid", NpgsqlDbType.Oid, oid);

                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                    connection.Close();
                }
            }
        }

    }

}
