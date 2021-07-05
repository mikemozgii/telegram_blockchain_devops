using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class TonNetworkDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public TonNetworkDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(TonNetwork e)
        {
             _context.TonNetworks.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<TonNetwork> GetByIds(IEnumerable<string> ids)
        {
            return _context.TonNetworks.Where(q => ids.Contains(q.Id)).ToList();
        }

        public TonNetwork GetById(string id)
        {
            return _context.TonNetworks.FirstOrDefault(q => q.Id == id);
        }

        public bool Update(params TonNetwork[] entities)
        {
            _context.TonNetworks.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }

        public TonNetwork GetByName(string name)
        {
            return _context.TonNetworks.FirstOrDefault(q => q.Name == name);
        }

        public IEnumerable<TonNetwork> GetBySmartAccountIds(params string[] smartAccountIds)
        {
            return _context.TonNetworks.Where(q => q.SmartAccountNetworks.Any(q1=> smartAccountIds.Contains(q1.SmartAccountId))).ToList();
        }
    }
}
