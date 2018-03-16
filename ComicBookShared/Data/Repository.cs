using ComicBookShared.Models;
using System.Collections.Generic;
using System.Linq;

namespace ComicBookShared.Data
{
    public class Repository 
    {
        private Context _context = null;
        public Repository(Context context) 
        {
            _context = context;
        }

        /// <summary>
        /// Get the series list
        /// </summary>
        /// <returns></returns>
        public IList<Series> GetSeriesList()
        {
            return _context.Series
                .OrderBy(s => s.Title)
                .ToList();
        }

        /// <summary>
        /// Get the roles list
        /// </summary>
        /// <returns></returns>
        public IList<Artist> GetArtists()
        {
            return _context.Artists
                .OrderBy(a => a.Name)
                .ToList();
        }

        /// <summary>
        /// Get the artitsts list
        /// </summary>
        /// <returns></returns>
        public IList<Role> GetRoles()
        {
            return _context.Roles
                .OrderBy(r => r.Name)
                .ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
