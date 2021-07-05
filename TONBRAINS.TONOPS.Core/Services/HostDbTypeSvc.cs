using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class HostDbTypeSvc
    {
        private TonOpsDbContext _context { get; set; }

        public HostDbTypeSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(HostType e)
        {
             _context.HostTypes.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<HostType> GetByIds(IEnumerable<string> nodeIds)
        {
            return _context.HostTypes.Where(q => nodeIds.Contains(q.Id)).ToList();
        }

        public HostType GetById(string nodeId)
        {
            return GetByIds(new string[]{ nodeId }).First();
        }
    }
}
