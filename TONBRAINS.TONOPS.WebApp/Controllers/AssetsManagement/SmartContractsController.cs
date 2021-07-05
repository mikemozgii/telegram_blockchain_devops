using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Services;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.WebApp.Controllers.AssetsManagement
{
    [Route("api/smartcontracts")]
    public class SmartContractsController
    {
        private readonly TonOpsDbContext _context;

        public SmartContractsController(TonOpsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("data")]
        public async Task<object> GetData(string libId)
        {
            var gridData = await _context.ViewSmartContracts.Where(i => i.LibId == libId).ToListAsync();
            var types = await _context.SmartTypes.ToListAsync();
            var libs = await _context.SmartContractsLibs.ToListAsync();

            return new
            {
                GridData = gridData,
                Types = types.Select(i => new { i.Id, Title = i.Name }),
                Libs = libs.Select(i => new { i.Id, Title = i.Name })
            };
        }

        [HttpGet("single")]
        public async Task<SmartContract> SmartContract(string id) => await _context.SmartContracts.FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted);

        [HttpPost("add")]
        public async Task<SmartContract> AddSmartContract([FromBody] SmartContract item)
        {
            item.Id = IdGenerator.Generate();

            var fileSvc = new FileSvc();

            var abiFile = await fileSvc.GetFileAsync(item.AbiFileId);
            item.AbiJson = Encoding.UTF8.GetString(abiFile);

            if (!string.IsNullOrEmpty(item.SolFileId))
            {
                var solFile = await fileSvc.GetFileAsync(item.SolFileId);
                item.SolJson = Encoding.UTF8.GetString(solFile);
            }

            await _context.SmartContracts.AddAsync(item);
            await _context.SaveChangesAsync();

            return item;
        }

        [HttpPost("edit")]
        public async Task<SmartContract> UpdateSmartContract([FromBody] SmartContract item)
        {
            var fileSvc = new FileSvc();

            var abiFile = await fileSvc.GetFileAsync(item.AbiFileId);
            item.AbiJson = Encoding.UTF8.GetString(abiFile);
            if (!string.IsNullOrEmpty(item.SolFileId))
            {
                var solFile = await fileSvc.GetFileAsync(item.SolFileId);
                item.SolJson = Encoding.UTF8.GetString(solFile);
            }
            else
            { 
                item.SolJson = null;
            }

            _context.SmartContracts.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [HttpDelete("soft-delete")]
        public async Task<bool> SoftDeleteSmartContract(string id)
        {
            var rows = await _context.SmartContracts.Where(i => i.Id == id).ToListAsync();
            rows.ForEach(q => q.IsDeleted = true);

            _context.SmartContracts.UpdateRange(rows);
            await _context.SaveChangesAsync();

            return rows.Any();
        }

        [HttpGet("options")]
        public async Task<object> GetOptions()
        {
            var smartContracts = await _context.SmartContracts.Where(i => !i.IsDeleted).ToListAsync();

            return new
            {
                SmartContracts = smartContracts.Select(i => new { i.Id, Title = i.Name })
            };
        }
    }
}
