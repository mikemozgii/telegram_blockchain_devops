using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp.Models.Ecosystems;
using TONBRAINS.TONOPS.WebApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/ecosystems")]
    [ApiController]
    public class EcosystemsController : SessionController
    {

        [Route("grid")]
        public async Task<IEnumerable<EcosystemModel>> Modules() => await ExecuteQuery<EcosystemModel>(new Query("ecosystems"));

        [Route("single")]
        public async Task<EcosystemModel> Module(string id)
        {
            var connectionStrings = await ExecuteQuery<EcosystemModel>(new Query("ecosystems").Where("id", id));

            return connectionStrings.FirstOrDefault();
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<EcosystemModel> AddOrEdit([FromBody] EcosystemModel item)
        {
            var session = Session;
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = IdGenerator.Generate();
            var oldItem = await ExecuteQueryFirst<EcosystemModel>(new Query("ecosystems").Where("id", item.Id));
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("ecosystems"), insert);

            var oldEnvironments = JsonConvert.DeserializeObject<IEnumerable<string>>(oldItem.Environments);
            var newEnvironment = JsonConvert.DeserializeObject<IEnumerable<string>>(item.Environments);

            var deletedEnvironments = oldEnvironments.Except(newEnvironment);
            var appendEnvironments = newEnvironment.Except(oldEnvironments);

            if (!deletedEnvironments.Any() && !appendEnvironments.Any()) return savedItem;

            var modules = (await ExecuteQuery<ModuleModel>(new Query("modules").Where("ecosystem", item.Id), session.AccountId)).Select(a => a.Id);
            var environments = await ExecuteQuery<EnvironmentModel>(new Query("environments").WhereIn("id", deletedEnvironments.Concat(appendEnvironments)));

            foreach (var deletedEnvironment in deletedEnvironments)
            {
                var environment = environments.First(a => a.Id == deletedEnvironment);
                var environmentModules = JsonConvert.DeserializeObject<IEnumerable<string>>(environment.Modules);
                environment.Modules = JsonConvert.SerializeObject(environmentModules.Where(a => !modules.Contains(a)).ToList());
                await ExecuteInsertOrUpdate(environment, new Query("environments"), insert: false);
            }

            foreach (var appendEnvironment in appendEnvironments)
            {
                var environment = environments.First(a => a.Id == appendEnvironment);
                var environmentModules = JsonConvert.DeserializeObject<List<string>>(environment.Modules);
                environmentModules.AddRange(modules);
                environment.Modules = JsonConvert.SerializeObject(environmentModules.Distinct());
                await ExecuteInsertOrUpdate(environment, new Query("environments"), insert: false);
            }

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("ecosystems").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }

        [Route("ecosystemmodules")]
        [HttpGet]
        public async Task<IEnumerable<ModuleTypeModel>> EcosystemModules(string id)
        {
            return await ExecuteSql<ModuleTypeModel>($"SELECT * FROM module_types WHERE ecosystems @> '[\"{id}\"]'", Session.AccountId);
        }

        [Route("saveecosystemmodules")]
        [HttpPost]
        public async Task<bool> SaveEcosystemModules([FromBody] EcosystemModuleModel model)
        {
            var moduleTypes = await ExecuteQuery<ModuleTypeModel>(new Query("module_types"), Session.AccountId);

            foreach (var moduleType in moduleTypes)
            {
                var ecosystemIds = JsonConvert.DeserializeObject<List<string>>(moduleType.Ecosystems);

                if (model.ModuleTypesIds.Contains(moduleType.Id) && !ecosystemIds.Contains(model.EcosystemId)) ecosystemIds.Add(model.EcosystemId);
                if (!model.ModuleTypesIds.Contains(moduleType.Id) && ecosystemIds.Contains(model.EcosystemId)) ecosystemIds.Remove(model.EcosystemId);

                moduleType.Ecosystems = JsonConvert.SerializeObject(ecosystemIds);

                await ExecuteInsertOrUpdate(moduleType, new Query("module_types"), insert: false);
            }

            return true;
        }

    }

}
