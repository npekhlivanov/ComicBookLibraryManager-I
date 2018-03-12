using ComicBookShared.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ComicBookShared.Data
{
    public class ComicBooksRepository : BaseRepository<ComicBook>
    {
        public ComicBooksRepository(Context context) : base (context)
        {
        }

        /// <summary>
        /// Get the comic books list, include the "Series" navigation property
        /// </summary>
        /// <returns></returns>
        public override IList<ComicBook> GetList()
        {
            return Context.ComicBooks
                .Include(cb => cb.Series)
                .OrderBy(cb => cb.Series.Title)
                .ThenBy(cb => cb.IssueNumber)
                .ToList();
        }

        /// <summary>
        /// Get the comic book
        /// </summary>
        /// <param name="id">Id of the ComicBook</param>
        /// <param name="includeSeries">Include the "Series" navigation propertiy</param>
        /// <param name="includeArtists">Include the "Artists.Artist" and "Artists.Role" navigation properties</param>
        /// <returns></returns>
        public ComicBook Get(int id, bool includeSeries, bool includeArtists = false)
        {
            var comicBookQuery = Context.ComicBooks.Where(cb => cb.Id == id);
            if (includeSeries)
            {
                comicBookQuery = comicBookQuery.Include(cb => cb.Series);
            }

            if (includeArtists)
            {
                comicBookQuery = comicBookQuery.Include(cb => cb.Artists.Select(a => a.Artist))
                    .Include(cb => cb.Artists.Select(a => a.Role));
            }

            return comicBookQuery.SingleOrDefault();
        }

        /// <summary>
        /// Update the comic book
        /// </summary>
        /// <param name="comicBook"></param>
        public override void Update(ComicBook comicBook)
        {
            // Variant 1: updates all fields, regardless of which ones were modified
            //_context.Entry(comicBook).State = EntityState.Modified;
 
            // Variant 2: fetch current values from DB and update only the changed fields 
            var originalComicBook = Context.ComicBooks.Find(comicBook.Id); //GetComicBook(comicBook.Id, false);
            Context.Entry(originalComicBook).CurrentValues.SetValues(comicBook);
            Context.SaveChanges();
        }

        /// <summary>
        /// Make sure that the provided issue number is unique for the provided series
        /// </summary>
        /// <param name="comicBook"></param>
        /// <returns></returns>
        public bool FindDuplicateIssueNumber(ComicBook comicBook)
        {
            return Context.ComicBooks
                .Any(cb => cb.IssueNumber == comicBook.IssueNumber && 
                           cb.SeriesId == comicBook.SeriesId && 
                           cb.Id != comicBook.Id);
        }

        /// <summary>
        /// Find if that this artist and role combination doesn't already exist for this comic book
        /// </summary>
        /// <param name="comicBookId"></param>
        /// <param name="artistId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool FindDuplicateArtistRoleCombination(int comicBookId, int artistId, int roleId)
        {
            return Context.ComicBookArtists
                .Any(cba => cba.ArtistId == artistId && 
                            cba.RoleId == roleId && 
                            cba.ComicBookId == comicBookId);
        }

        public override ComicBook Get(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
