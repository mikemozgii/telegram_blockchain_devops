using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.WebApp.Helpers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class SmartAccountNetworkDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public SmartAccountNetworkDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(SmartAccountNetwork e)
        {
             _context.SmartAccountNetworks.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<SmartAccountNetwork> GetByIds(IEnumerable<string> ids)
        {
            return _context.SmartAccountNetworks.Where(q => ids.Contains(q.Id)).ToList();
        }

        public SmartAccountNetwork GetBySmartAccountIdAndTonNetworkId(string smartAccountId, string tonNetworkId)
        {
            return _context.SmartAccountNetworks.FirstOrDefault(q => q.SmartAccountId == smartAccountId && q.NetworkId == tonNetworkId);
        }

        public SmartAccountNetwork GetById(string id)
        {
            return GetByIds(new string[]{ id }).First();
        }

        public IEnumerable<SmartAccountNetwork> GetBySmartAccountIds(IEnumerable<string> ids)
        {
            return _context.SmartAccountNetworks.Where(q => ids.Contains(q.SmartAccountId)).ToList();
        }

        public IEnumerable<SmartAccountNetwork> GetByTonNetworkIds(params string[] tonNetworksids)
        {
            return _context.SmartAccountNetworks.Where(q => tonNetworksids.Contains(q.NetworkId)).ToList();
        }

        public void SaveAccountStateMetric(string smartAccountId, string quantchainId, string raw, string status, long balance)
        {
            var log = new SmartAccountStateLog
            {
                Id = IdGenerator.Generate(),
                Balance = balance,
                Raw = raw,
                Date = DateTime.UtcNow,
                NetworkId = quantchainId,
                SmartAccountId = smartAccountId,
                Status = status
            };
            _context.SmartAccountStateLogs.Add(log);
            _context.SaveChanges();
        }


        public bool Update(params SmartAccountNetwork[] entities)
        {
            _context.SmartAccountNetworks.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }


        public bool Delete(params SmartAccountNetwork[] entities)
        {
            _context.SmartAccountNetworks.RemoveRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteByIds(params string[] ids)
        {
            var es = GetByIds(ids);
            _context.SmartAccountNetworks.RemoveRange(es);
            return _context.SaveChanges() > 0;
        }
    }
}
