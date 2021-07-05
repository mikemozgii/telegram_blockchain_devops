using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/moduletype")]
    [ApiController]
    public class ConnectionStringController : ControllerBase
    {

        [Route("grid")]
        public async Task<IEnumerable<ModuleTypeModel>> Modules() => await ExecuteQuery<ModuleTypeModel>(new Query("module_types"));

        [Route("single")]
        public async Task<ModuleTypeModel> Module(string id) => (await ExecuteQuery<ModuleTypeModel>(new Query("module_types").Where("id", id))).FirstOrDefault();

        [Route("addoredit")]
        [HttpPost]
        public async Task<ModuleTypeModel> AddOrEdit([FromBody] ModuleTypeModel item)
        {
            if (item.Ecosystems == null) item.Ecosystems = "[]";
            var moduleType = (await ExecuteQuery<ModuleTypeModel>(new Query("module_types").Where("id", item.Id))).FirstOrDefault();
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("module_types"), moduleType == null);

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("module_types").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }

    }

}
