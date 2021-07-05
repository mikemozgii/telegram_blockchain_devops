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
    public class NodeMetricDbSvc
    {


        private TonOpsDbContext _context { get; set; }

        public NodeMetricDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(NodeMetric e)
        {
            _context.NodesMetrics.Add(e);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<NodeMetric> GetByIds(IEnumerable<string> ids)
        {
            return _context.NodesMetrics.Where(q => ids.Contains(q.Id)).ToList();
        }

        public NodeMetric GetById(string id)
        {
            return GetByIds(new string[] { id }).First();
        }


        public IEnumerable<NodeMetric> GetByNodeIds(params string[] ids)
        {
            return _context.NodesMetrics.Where(q => ids.Contains(q.NodeId)).ToList();
        }

        public bool Update(params NodeMetric[] entities)
        {
            _context.NodesMetrics.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }


        public bool Delete(params NodeMetric[] entities)
        {
            _context.NodesMetrics.RemoveRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteByIds(params string[] ids)
        {
            var es = GetByIds(ids);
            _context.NodesMetrics.RemoveRange(es);
            return _context.SaveChanges() > 0;
        }

    }
}
