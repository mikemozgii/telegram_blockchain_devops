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
    public class SmartKeyDBSvc
    {


        private TonOpsDbContext _context { get; set; }

        public SmartKeyDBSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(SmartKey e)
        {
            _context.SmartKeys.Add(e);
            _context.SaveChanges();
            return true;
        }

        public IEnumerable<SmartKey> GetByIds(IEnumerable<string> ids)
        {
            return _context.SmartKeys.Where(q => ids.Contains(q.Id)).ToList();
        }

        public SmartKey GetById(string id)
        {
            return _context.SmartKeys.FirstOrDefault(q=>q.Id == id);
        }

        public SmartKey GetBySecretAndPublicKey(string secretKey, string publicKey)
        {
            return _context.SmartKeys.FirstOrDefault(q => q.SecretKey == secretKey && q.PublicKey == publicKey);
        }


        public bool Update(params SmartKey[] entities)
        {
            _context.SmartKeys.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }


        public bool Delete(params SmartKey[] entities)
        {
            _context.SmartKeys.RemoveRange(entities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteByIds(params string[] ids)
        {
            var es = GetByIds(ids);
            _context.SmartKeys.RemoveRange(es);
            return _context.SaveChanges() > 0;
        }

    }
}
