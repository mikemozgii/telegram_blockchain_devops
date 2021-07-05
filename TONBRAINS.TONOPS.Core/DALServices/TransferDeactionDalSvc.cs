using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Extensions;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.DALServices
{
    public class TransferDeactionDalSvc
    {

        private TonOpsDbContext _context { get; set; }

        public TransferDeactionDalSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(params TransferDeaction[] es)
        {
            _context.TransferDeactions.AddRange(es);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<TransferDeaction> GetByIds(IEnumerable<string> ids)
        {
            return _context.TransferDeactions.Where(q => ids.Contains(q.Id)).ToList();
        }


        public IEnumerable<TransferDeaction> GetByInitSmartAccountNetworkIds(IEnumerable<string> initSmartAccountNetworkIds)
        {
            return _context.TransferDeactions
                .Where(q =>
                    //transfer
                    initSmartAccountNetworkIds.Contains(q.InitSmartAccountNetworkId) ||
                    //sent or payment
                    initSmartAccountNetworkIds.Contains(q.CompletedSmartAccountNetworkId))
                .ToList();
        }

        public TransferDeaction GetById(string id)
        {
            return _context.TransferDeactions.FirstOrDefault(q => q.Id == id); ;
        }

        public TransferDeaction GetByAuthToken(string autToken)
        {
            return _context.TransferDeactions.FirstOrDefault(q => q.AuthToken == autToken);
        }

        public bool Update(params TransferDeaction[] entities)
        {
            _context.TransferDeactions.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }


        public bool Delete(params TransferDeaction[] entities)
        {
            _context.TransferDeactions.RemoveRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteByIds(params string[] ids)
        {
            var es = GetByIds(ids);
            _context.TransferDeactions.RemoveRange(es);
            return _context.SaveChanges() > 0;
        }

    }
}
