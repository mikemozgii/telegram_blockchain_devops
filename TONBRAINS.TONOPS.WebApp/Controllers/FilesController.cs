using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TONBRAINS.TONOPS.Core.DAL;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TONBRAINS.TONOPS.Core.Services;
using System.Text;

namespace TONBRAINS.TONOPS.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/files")]
    public class FilesController : Controller
    {
        private readonly TonOpsDbContext _context;

        public FilesController(TonOpsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        [RequestSizeLimit(500 * 1024 * 1024)]
        [Route("")]
        public async Task<IActionResult> UploadNewFile(IFormFile content)
        {
            using (var stream = new MemoryStream())
            {
                await content.CopyToAsync(stream);
                stream.Position = 0;
                var id = await new FileSvc().CreateAndUploadFileAsync(stream, content.FileName);

                var result = await _context.Files.FirstOrDefaultAsync(q => q.Id == id);

                return Json(result);
            }
        }

        [HttpPost]
        [Route("fileentities")]
        public async Task<JsonResult> GetFileEntities([FromBody] IList<string> ids, [FromQuery] string module)
        {
            var result = await _context.Files.Where(i => ids.Contains(i.Id)).ToListAsync();
            return Json(result);
        }

        [HttpPost]
        [Route("deletefiles")]
        public async Task<bool> DeleteMultipleFiles([FromBody] IList<string> ids)
        {
            if (ids == null || !ids.Any()) return true;

            var rows = await _context.Files.Where(i => ids.Contains(i.Id)).ToListAsync();

            _context.Files.RemoveRange(rows);
            await _context.SaveChangesAsync();

            return rows.Any();
        }

        [HttpGet]
        [Route("getfiletext")]
        public async Task<string> GetFileText(string id)
        {
            var abiFile = await new FileSvc().GetFileAsync(id);

            return Encoding.UTF8.GetString(abiFile);
        }
    }
}