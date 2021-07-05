using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class HostDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public HostDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(Host node)
        {
             _context.Hosts.Add(node);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<Host> GetByIds(IEnumerable<string> ids)
        {
            return _context.Hosts.Where(q => ids.Contains(q.Id)).ToList();
        }

        public Host GetById(string id)
        {
            return GetByIds(new string[]{ id }).First();
        }


    }
}
