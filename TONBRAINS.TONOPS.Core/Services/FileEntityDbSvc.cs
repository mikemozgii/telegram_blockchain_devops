using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.DAL;
using TONBRAINS.TONOPS.Core.Handlers;

namespace TONBRAINS.TONOPS.Core.Services
{
    public class FileEntityDbSvc
    {
        private TonOpsDbContext _context { get; set; }

        public FileEntityDbSvc()
        {
            _context = new TonOpsDbContext(GlobalAppConfHandler.GetTonOpsDbContextOption());
        }

        public bool Add(FileEntity e)
        {
             _context.Files.Add(e);
             _context.SaveChanges();
            return true;
        }

        public IEnumerable<FileEntity> GetByIds(IEnumerable<string> ids)
        {
            return _context.Files.Where(q => ids.Contains(q.Id)).ToList();
        }

        public FileEntity GetById(string id)
        {
            return GetByIds(new string[]{ id }).First();
        }

        public bool Update(params FileEntity[] entities)
        {
            _context.Files.UpdateRange(entities);
            return _context.SaveChanges() > 0;
        }
    }
}
