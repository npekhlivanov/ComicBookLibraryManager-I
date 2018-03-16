using ComicBookShared.Models;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public abstract class BaseContext : DbContext
    {
        public void Add(BaseEntity entity, DbSet<BaseEntity> dbSet)
        {
            dbSet.Add(entity);
            SaveChanges();
        }
    }


}
