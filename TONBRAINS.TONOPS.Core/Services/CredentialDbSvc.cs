using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class CredentialDbSvc
    {

        private TonOpsDbContext _context { get; set; }

        public CredentialDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(Host node)
        {
            _context.Hosts.Add(node);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Credential> GetByIds(IEnumerable<string> Ids)
        {
            return _context.Credentials.Where(q => Ids.Contains(q.Id)).ToList();
        }

        public Credential GetById(string Id)
        {
            return GetByIds(new string[] { Id }).First();
        }
    }
}
