using TONBRAINS.TONOPS.WebApp.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlKata;
using System.Linq;
using System.Threading.Tasks;
using static TONBRAINS.TONOPS.Core.Handlers.KataHelpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.ComponentServerAddress = "/";

            var moduleTypes = (await ExecuteQuery<ModuleTypeModel>(new Query("module_types"))).Select(a => new { a.Id, Title = a.Name });
            ViewBag.ModuleTypes = JsonConvert.SerializeObject(moduleTypes, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });

            return View();
        }

    }
}
