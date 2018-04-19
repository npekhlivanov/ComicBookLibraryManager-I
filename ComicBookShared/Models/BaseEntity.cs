using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        public static int Add<TEntity>(this DbContext context, DbSet<TEntity> dbSet, TEntity entity) 
            where TEntity : BaseEntity
        {
            var createdOnProperty = typeof(TEntity).GetProperty("CreatedOn");
            if (createdOnProperty != null)
            {
                createdOnProperty.SetValue(entity, DateTime.Now);
            }

            dbSet.Add(entity);
            context.SaveChanges();
            return entity.Id;
        }

        public static int Add<TEntity>(this DbContext context, TEntity entity)
           where TEntity : BaseEntity
        {
            var dbSet = context.GetDbSet<TEntity>();
            return Add<TEntity>(context, dbSet, entity);
        }

        public static bool Update<TEntity>(this DbContext context, DbSet<TEntity> dbSet, TEntity entity)
            where TEntity : BaseEntity
        {
            var modifiedOnProperty = typeof(TEntity).GetProperty("ModifiedOn");
            TEntity entryToUpdate;
            if (UpdateAllFields)
            {
                var entry = context.Entry(entity);
                entry.State = EntityState.Modified;
                entryToUpdate = entity;
            }
            else
            {
                var entityInDb = dbSet.Find(entity.Id);
                if (entityInDb == null)
                {
                    return false;
                }

                entryToUpdate = entityInDb;
                var entry = context.Entry(entityInDb);
                entry.CurrentValues.SetValues(entity);

                if (typeof(TEntity).GetProperty("CreatedOn") != null)
                {
                    entry.Property("CreatedOn").IsModified =false;
                }

                if (typeof(TEntity).GetProperty("DeletedOn") != null)
                {
                    entry.Property("DeletedOn").IsModified = false;
                }

                if (modifiedOnProperty != null)
                {
                    entry.Property("ModifiedOn").IsModified = false;
                }

                if (entry.State == EntityState.Unchanged)
                {
                    return false;
                }
            }

            if (modifiedOnProperty != null)
            {
                modifiedOnProperty.SetValue(entryToUpdate, DateTime.Now);
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
                var entry = context.Entry(entityInDb);
                entry.State = EntityState.Modified;
                
                if (typeof(TEntity).GetProperty("CreatedOn") != null)
                {
                    entry.Property("CreatedOn").IsModified = false;
                }

                if (typeof(TEntity).GetProperty("ModifiedOn") != null)
                { 
                    entry.Property("ModifiedOn").IsModified = false;
                }

                var deletedOnProperty = typeof(TEntity).GetProperty("DeletedOn");
                if (deletedOnProperty != null)
                {
                    deletedOnProperty.SetValue(entityInDb, DateTime.Now);
                }
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
