using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class TonConfigurationDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public TonConfigurationDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(TonConfiguration e)
        {
             _context.TonConfigurations.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<TonConfiguration> GetByIds(IEnumerable<string> ids)
        {
            return _context.TonConfigurations.Where(q => ids.Contains(q.Id)).ToList();
        }

        public TonConfiguration GetById(string id)
         {
            return GetByIds(new string[]{ id }).First();
        }

        public bool Update(params TonConfiguration[] entities)
        {
            _context.TonConfigurations.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }
    }
}
