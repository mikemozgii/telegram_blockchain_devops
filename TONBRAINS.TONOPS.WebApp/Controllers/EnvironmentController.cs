using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using SqlKata;
using System.Linq;
using System;
using TONBRAINS.TONOPS.WebApp.Common.Models;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/environment")]
    [ApiController]
    public class EnvironmentController : SessionController
    {

        [Route("checkuniquedomain")]
        public async Task<bool> CheckUniqueDomain(string domain)
        {
            var environments = await ExecuteQuery<EnvironmentModel>(new Query("environments").Where("domain", domain));

            return environments.Any();
        }

        [Route("grid")]
        public async Task<IEnumerable<EnvironmentModel>> Environments() => await ExecuteQuery<EnvironmentModel>(new Query("environments"));

        [Route("single")]
        public async Task<EnvironmentModel> Environment(string id)
        {
            var environments = await ExecuteQuery<EnvironmentModel>(new Query("environments").Where("id", id));

            return environments.First();
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<EnvironmentModel> AddOrEdit([FromBody] EnvironmentModel EnvironmentModel)
        {
            var insert = string.IsNullOrEmpty(EnvironmentModel.Id);
            if (insert) EnvironmentModel.Id = Guid.NewGuid().ToString();
            var savedItem = await ExecuteInsertOrUpdate(EnvironmentModel, new Query("environments"), insert);

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("environments").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
                //TODO: Need logging????
            }

            return result;
        }

    }
}
