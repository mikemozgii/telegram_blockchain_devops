using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.WebApp.Common.Models;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Route("api/menusecurity")]
    [ApiController]
    public class MenuSecurityController : SessionController
    {

        [Route("single")]
        public async Task<SecurityMenuModel> MenuSecurity(string id)
        {
            var securities = await ExecuteQuery<SecurityMenuModel>(new Query("securities").Where("id", id));

            return securities.First();
        }

        [Route("addoredit")]
        [HttpPost]
        public async Task<SecurityMenuModel> AddOrEdit([FromBody] SecurityMenuModel item)
        {
            var insert = !(await ExecuteQuery<SecurityMenuModel>(new Query("securities").Where("id", item.Id))).Any();
            var savedItem = await ExecuteInsertOrUpdate(item, new Query("securities"), insert);

            return savedItem;
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            var result = false;

            try
            {
                await ExecuteQuery(new Query("securities").Where("id", id).AsDelete());
                return true;
            }
            catch
            {
            }

            return result;
        }
    }
}
