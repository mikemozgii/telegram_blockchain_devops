using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class NodeDbSvc
    {

        private TonOpsDbContext _context { get; set; }

        public NodeDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(Node entity)
        {
            _context.Nodes.Add(entity);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<Node> GetByIds(params string[] ids)
        {
            return _context.Nodes.Where(q => ids.Contains(q.Id) && !q.IsDeleted).ToList();
        }

        public bool Update(params Node[] entities)
        {
            _context.Nodes.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }

        public Node GetById(string nodeId)
        {
            return GetByIds(new string[] { nodeId }).First();
        }

        public bool SoftDeleteById(string id)
        {
            var item = _context.Nodes.FirstOrDefault(i => i.Id == id);
            item.IsDeleted = true;
            return _context.SaveChanges() > 0;
        }

        //public bool SoftDeleteById(string id)
        //{
        //    var item = _context.Nodes.FirstOrDefault(i => i.Id == id);
        //    item.IsDeleted = true;
        //    return _context.SaveChanges() > 0;
        //}

        public IEnumerable<Node> GetByTonNeworkIds(IEnumerable<string> ids)
        {
            return _context.Nodes.Where(q => ids.Contains(q.TonNetworkId) && !q.IsDeleted).ToList();
        }

        public IEnumerable<Node> GetByTonNeworkId(string id)
        {
            return GetByTonNeworkIds(new string[] { id });
        }

        public Node GetRandomExecutionNodeByTonNeworkId(string tonNetworkid)
        {
            return GetByTonNeworkIds(new string[] { tonNetworkid }).First();
        }


    }
}
