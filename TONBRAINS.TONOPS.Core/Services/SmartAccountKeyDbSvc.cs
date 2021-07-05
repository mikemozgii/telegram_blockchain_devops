using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class SmartAccountKeyDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public SmartAccountKeyDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(SmartAccountKey e)
        {
             _context.SmartAccountKeys.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<SmartAccountKey> GetByIds(IEnumerable<string> ids)
        {
            return _context.SmartAccountKeys.Where(q => ids.Contains(q.Id)).ToList();
        }

        public SmartAccountKey GetById(string id)
        {
            return GetByIds(new string[]{ id }).First();
        }

        public bool Update(params SmartAccountKey[] entities)
        {
            _context.SmartAccountKeys.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(params SmartAccountKey[] entities)
        {
            _context.SmartAccountKeys.RemoveRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteByIds(params string[] ids)
        {
            var es = GetByIds(ids);    
            _context.SmartAccountKeys.RemoveRange(es);
            return _context.SaveChanges() > 0;
        }
    }
}
