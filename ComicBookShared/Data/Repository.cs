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

        const string SeriesListKey = "SeriesList";
        /// <summary>
        /// Get the series list
        /// </summary>
        /// <returns></returns>
        public IList<Series> GetSeriesList()
        {
            var seriesList = EntityCache.Get<List<Series>>(SeriesListKey);
            if (seriesList == null)
            {
                seriesList = _context.Series
                    .OrderBy(s => s.Title)
                    .ToList();
                EntityCache.Add(SeriesListKey, seriesList);
            }

            return seriesList;
        }

        public void MarkSeriesModified()
        {
            EntityCache.Remove(SeriesListKey);
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
