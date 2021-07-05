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
    public class SmartAccountTransferLogDBSvc
    {


        private TonOpsDbContext _context { get; set; }

        public SmartAccountTransferLogDBSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(SmartAccountBalanceTransferLog e)
        {
            _context.SmartAccountBalanceTransferLogs.Add(e);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<SmartAccountBalanceTransferLog> GetByIds(IEnumerable<string> ids)
        {
            return _context.SmartAccountBalanceTransferLogs.Where(q => ids.Contains(q.Id)).ToList();
        }

        public SmartAccountBalanceTransferLog GetById(string id)
        {
            return GetByIds(new string[] { id }).First();
        }




        public bool Update(params SmartAccountBalanceTransferLog[] entities)
        {
            _context.SmartAccountBalanceTransferLogs.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }


        public bool Delete(params SmartAccountBalanceTransferLog[] entities)
        {
            _context.SmartAccountBalanceTransferLogs.RemoveRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteByIds(params string[] ids)
        {
            var es = GetByIds(ids);
            _context.SmartAccountBalanceTransferLogs.RemoveRange(es);
            return _context.SaveChanges() > 0;
        }

    }
}
