using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TONBRAINS.TONOPS.Core.Extensions
{
    public static class DbContextExt
    {
        public static bool Exists<TContext, TEntity>(this TContext context, TEntity entity)
    where TContext : DbContext
    where TEntity : class
        {
            return context.Set<TEntity>().Local.Any(e => e == entity);
        }


        public static bool ExistsRange<TContext, TEntity>(this TContext context, IEnumerable<TEntity> entity)
where TContext : DbContext
where TEntity : class
        {
            return context.Set<TEntity>().Local.Any(e => entity.Contains(e));
        }


        public static void IfNotExistsAttachRange<TContext, TEntity>(this TContext context, IEnumerable<TEntity> entity)
where TContext : DbContext
where TEntity : class
        {
            if (!context.ExistsRange(entity))
            {
                context.AttachRange(entity);
            }

            //return context.Set<TEntity>().Local.Any(e => entity.Contains(e));
        }
    }
}
