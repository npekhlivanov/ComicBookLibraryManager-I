using ComicBookShared.Data;
using System.Data.Entity;

namespace ComicBookShared.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }

    public abstract class BaseContext : DbContext
    {
        public void Add(BaseEntity entity, DbSet<BaseEntity> dbSet)
        {
            dbSet.Add(entity);
            SaveChanges();
        }
    }

    public static class ContextExtensions
    {
        public static bool UpdateAllFields = false;
        public static bool FindBeforeDelete = true;

        public static void Add<TEntity>(this DbContext context, DbSet<TEntity> dbSet, TEntity entity) 
            where TEntity : BaseEntity
        {
            var entitySet = context.Set<TEntity>();
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public static void Add<TEntity>(this DbContext context, TEntity entity)
           where TEntity : BaseEntity
        {
            var dbSet = context.GetDbSet<TEntity>();
            dbSet.Add(entity);
            context.SaveChanges();
        }

        public static bool Update<TEntity>(this DbContext context, DbSet<TEntity> dbSet, TEntity entity)
            where TEntity : BaseEntity
        {
            if (UpdateAllFields)
            {
                context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var entityInDb = dbSet.Find(entity.Id);
                if (entityInDb == null)
                {
                    return false;
                }

                var entry = context.Entry(entityInDb);
                entry.CurrentValues.SetValues(entity);
                if (entry.State == EntityState.Unchanged)
                {
                    return false;
                }
            }

            context.SaveChanges();
            return true;
        }

        public static bool Update<TEntity>(this DbContext context, TEntity entity)
           where TEntity : BaseEntity
        {
            if (UpdateAllFields)
            {
                context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var dbSet = context.GetDbSet<TEntity>();
                var entityInDb = dbSet.Find(entity.Id);
                if (entityInDb == null)
                {
                    return false;
                }

                var entry = context.Entry(entityInDb);
                entry.CurrentValues.SetValues(entity);
                if (entry.State == EntityState.Unchanged)
                {
                    return false;
                }
            }

            context.SaveChanges();
            return true;
        }

        public static bool Delete<TEntity>(this DbContext context, DbSet<TEntity> dbSet, int id)
            where TEntity : BaseEntity, new()
        {
            if (FindBeforeDelete)
            {
                var entityInDb = dbSet.Find(id);
                if (entityInDb == null)
                {
                    return false;
                }

                dbSet.Remove(entityInDb);
            }
            else
            {
                var entity = new TEntity() { Id = id };
                context.Entry(entity).State = EntityState.Deleted;
            }

            context.SaveChanges();
            return true;
        }

        public static bool Delete<TEntity>(this DbContext context, int id)
            where TEntity : BaseEntity, new()
        {
            if (FindBeforeDelete)
            {
                var dbSet = context.GetDbSet<TEntity>();
                var entityInDb = dbSet.Find(id);
                if (entityInDb == null)
                {
                    return false;
                }

                dbSet.Remove(entityInDb);
            }
            else
            {
                var entity = new TEntity() { Id = id };
                context.Entry(entity).State = EntityState.Deleted;
            }

            context.SaveChanges();
            return true;
        }

        private static DbSet<TEntity> GetDbSet<TEntity>(this DbContext context) 
            where TEntity : class
        {
            var dbSet = context.Set<TEntity>();
            if (dbSet == null)
            {
                throw new System.ArgumentException(string.Format("The DbContext \"{0}\" has no DbSet of type \"{1}\"", context.ToString(), typeof(TEntity).FullName));
            }

            return dbSet;
        }
        //public static void AddEntity(this DbContext context, DbSet<BaseEntity> dbSet, BaseEntity entity)
        //{
        //    dbSet.Add(entity);
        //    context.SaveChanges();
        //}

        //public static bool UpdateEntity(this DbContext context, DbSet<BaseEntity> dbSet, BaseEntity entity)
        //{
        //    if (UpdateAllFields)
        //    {
        //        context.Entry(entity).State = EntityState.Modified;
        //    }
        //    else
        //    {
        //        var entityInDb = dbSet.Find(entity.Id);
        //        if (entityInDb == null)
        //        {
        //            return false;
        //        }

        //        var entry = context.Entry(entityInDb);
        //        entry.CurrentValues.SetValues(entity);
        //        if (entry.State == EntityState.Unchanged)
        //        {
        //            return false;
        //        }
        //    }

        //    context.SaveChanges();
        //    return true;
        //}

        //public static bool DeleteEntity(this DbContext context, DbSet<BaseEntity> dbSet, int id)
        //{
        //    if (FindBeforeDelete)
        //    {
        //        var entityInDb = dbSet.Find(id);
        //        if (entityInDb == null)
        //        {
        //            return false;
        //        }

        //        dbSet.Remove(entityInDb);
        //    }
        //    else
        //    {
        //        var entity = new BaseEntity() { Id = id };
        //        context.Entry(entity).State = EntityState.Deleted;
        //    }

        //    context.SaveChanges();
        //    return true;
        //}
    }
}
