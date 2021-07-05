using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class NodeCoreTypeDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public NodeCoreTypeDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        //public bool Add(NodeCoreType e)
        //{
        //     _context.NodeCoreType.Add(e);
        //     _context.SaveChanges();
        //    return true;
        //}

        public IEnumerable<NodeCoreType> GetByIds(IEnumerable<string> ids)
        {
            return _context.NodeCoreTypes.Where(q => ids.Contains(q.Id)).ToList();
        }

        public NodeCoreType GetById(string id)
        {
            return GetByIds(new string[]{ id }).First();
        }


    }
}
