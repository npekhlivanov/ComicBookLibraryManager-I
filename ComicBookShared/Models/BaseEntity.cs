using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Reflection;

namespace ComicBookShared.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        //[NotMapped]
        //public virtual bool EnableLogicalDelete { get => false; }
    }

    public static class ContextCrudExtensions
    {
        public static bool UpdateAllFields = false;
        public static bool FindBeforeDelete = true;

        //public static IQueryable<TEntity> GetList<TEntity>(this DbContext context, DbSet<TEntity> dbSet)
        //    where TEntity: BaseEntity
        //{
        //    var list = dbSet.AsQueryable();
        //    var entity = list.FirstOrDefault();
        //    if (entity != null && entity.EnableLogicalDelete)
        //    {
        //        var property = typeof(TEntity).GetProperty("IsDeleted");
        //        list = list.Where(e => (bool)property.GetValue(e) == false);
        //    }

        //    return list;
        //}

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
            Add<TEntity>(context, dbSet, entity);
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
            var dbSet = UpdateAllFields ? null : context.GetDbSet<TEntity>();
            return Update<TEntity>(context, dbSet, entity);

        }

        private static bool FindAndDelete<TEntity>(this DbContext context, DbSet<TEntity> dbSet, int id, PropertyInfo isDeletedProperty)
            where TEntity : BaseEntity, new()
        {
            var entityInDb = dbSet.Find(id);
            if (entityInDb == null)
            {
                return false;
            }

            if (isDeletedProperty != null)
            {
                isDeletedProperty.SetValue(entityInDb, true);
                context.Entry(entityInDb).State = EntityState.Modified;
            }
            else
            {
                dbSet.Remove(entityInDb);
            }

            context.SaveChanges();
            return true;
        }

        public static bool Delete<TEntity>(this DbContext context, DbSet<TEntity> dbSet, int id)
            where TEntity : BaseEntity, new()
        {
            var isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");
            if (FindBeforeDelete || isDeletedProperty != null)
            {
                return FindAndDelete<TEntity>(context, dbSet, id, isDeletedProperty);
            }

            var entity = new TEntity() { Id = id };
            context.Entry(entity).State = EntityState.Deleted;
            context.SaveChanges();
            return true;
        }

        public static bool Delete<TEntity>(this DbContext context, int id)
            where TEntity : BaseEntity, new()
        {
            var dbSet = context.GetDbSet<TEntity>();
            return Delete<TEntity>(context, dbSet, id);
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
