using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using TONBRAINS.TONOPS.WebApp;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/credentials")]
    public class CredentialController : SessionController
    {
        [Route("grid")]
        public async Task<IEnumerable<Credential>> Credentials()
            => await ExecuteQuery<Credential>(new Query("credentials"));

        [Route("single")]
        public async Task<Credential> Module(string id)
        {
            var connectionStrings = await ExecuteQuery<Credential>(new Query("credentials").Where("id", id));

            return connectionStrings.FirstOrDefault();
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<Credential> AddOrEdit([FromBody] Credential item)
        {
            var insert = string.IsNullOrEmpty(item.Id);
            if (insert) item.Id = IdGenerator.Generate();
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("credentials"), insert);

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("credentials").Where("id", id).AsDelete());
                result = true;
            }
            catch
            {
            }

            return result;
        }
    }
}
