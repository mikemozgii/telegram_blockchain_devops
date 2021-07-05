using TONBRAINS.TONOPS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{

    public class ComponentsController : Controller
    {

        private string Hash(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash) sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        [HttpGet]
        [Route("/component/{*path}")]
        public IActionResult Common(string path)
        {
            HttpContext.Items["ComponentId"] = Hash(path);
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);

            var model = new ComponentModel
            {
            };

            return View($"~/Views/Components/{path}.cshtml", model);
        }

    }

}
