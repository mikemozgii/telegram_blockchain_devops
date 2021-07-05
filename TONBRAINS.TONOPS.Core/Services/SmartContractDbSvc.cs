using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class SmartContractDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public SmartContractDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(SmartContract e)
        {
             _context.SmartContracts.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<SmartContract> GetByIds(IEnumerable<string> ids)
        {
            return _context.SmartContracts.Where(q => ids.Contains(q.Id)).ToList();
        }

        public SmartContract GetById(string id)
        {
            return _context.SmartContracts.FirstOrDefault(q=>q.Id == id);
        }

        public SmartContract GetByName(string name)
        {
            return _context.SmartContracts.FirstOrDefault(q => q.Name == name);
        }

        public bool Update(params SmartContract[] entities)
        {
            _context.SmartContracts.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }
    }
}
