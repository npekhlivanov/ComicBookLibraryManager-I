using System.Collections.Generic;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public abstract class BaseRepository<TEntity> 
        where TEntity : class
        //IEntity, // enforces that the generic type implement the IEntity interface
        //new() // enforces that the generic type define a default constructor
    {
        protected Context Context { get; private set; }

        public BaseRepository(Context context)
        {
            Context = context;
        }

        public abstract IList<TEntity> GetList();

        public abstract TEntity Get(int id);

        public virtual void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
        }

        public virtual void Update(TEntity entity)
        {
            //var set = Context.Set<TEntity>();
            //var entityInDb = set.Find(entity.Id);
            //Context.Entry(entityInDb).CurrentValues.SetValues(entity);
            Context.Entry<TEntity>(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            //var entity = new TEntity() { Id = id };
            //Context.Entry<TEntity>(entity).State = EntityState.Deleted;
            var set = Context.Set<TEntity>();
            var entity = set.Find(id); // produces a query to the DB
            set.Remove(entity);
            Context.SaveChanges();
        }
    }
}
