using ComicBookShared.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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
        /// Get the comic books list, include the "Series" navigation property
        /// </summary>
        /// <returns></returns>
        public IList<ComicBook> GetComicBooks()
        {
            return _context.ComicBooks
                .Include(cb => cb.Series)
                .OrderBy(cb => cb.Series.Title)
                .ThenBy(cb => cb.IssueNumber)
                .ToList();
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

        ///// <summary>
        ///// Get the comic book, 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public ComicBook GetComicBook(int id)
        //{
        //    return _context.ComicBooks
        //        .Include(cb => cb.Series)
        //        .Include(cb => cb.Artists.Select(a => a.Artist))
        //        .Include(cb => cb.Artists.Select(a => a.Role))
        //        .Where(cb => cb.Id == id)
        //        .SingleOrDefault();
        //}

        /// <summary>
        /// Get the comic book
        /// </summary>
        /// <param name="id">Id of the ComicBook</param>
        /// <param name="includeSeries">Include the "Series" navigation propertiy</param>
        /// <param name="includeArtists">Include the "Artists.Artist" and "Artists.Role" navigation properties</param>
        /// <returns></returns>
        public ComicBook GetComicBook(int id, bool includeSeries, bool includeArtists = false)
        {
            var comicBookQuery = _context.ComicBooks.Where(cb => cb.Id == id);
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
        /// Add the comic book
        /// </summary>
        /// <param name="comicBook"></param>
        public void AddComicBook(ComicBook comicBook)
        {
            _context.ComicBooks.Add(comicBook);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update the comic book
        /// </summary>
        /// <param name="comicBook"></param>
        public void UpdateComicBook(ComicBook comicBook)
        {
            // Variant 1: updates all fields, regardless of which ones were modified
            //_context.Entry(comicBook).State = EntityState.Modified;
 
            // Variant 2: fetch current values from DB and update only changed fields 
            var originalComicBook = _context.ComicBooks.Find(comicBook.Id); //GetComicBook(comicBook.Id, false);
            _context.Entry(originalComicBook).CurrentValues.SetValues(comicBook);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete the comic book
        /// </summary>
        /// <param name="id">Id of the ComicBook</param>
        public void DeleteComicBook(int id)
        {
            var comicBook = new ComicBook() { Id = id };
            _context.Entry(comicBook).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        /// <summary>
        /// Make sure that the provided issue number is unique for the provided series
        /// </summary>
        /// <param name="comicBook"></param>
        /// <returns></returns>
        public bool FindDuplicateIssueNumber(ComicBook comicBook)
        {
            return _context.ComicBooks
                .Any(cb => cb.IssueNumber == comicBook.IssueNumber && 
                           cb.SeriesId == comicBook.SeriesId && 
                           cb.Id != comicBook.Id);
        }

        /// <summary>
        /// Get the comic book artist, include the "ComicBook.Series", "Artist", and "Role" navigation properties
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ComicBookArtist GetComicBookArtist(int id)
        {
            return _context.ComicBookArtists
                .Include(cba => cba.ComicBook.Series)
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Where(cba => cba.Id == id)
                .SingleOrDefault();
        }

        public void AddComicBookArtist(ComicBookArtist artist)
        {
            _context.ComicBookArtists.Add(artist);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete the comic book artist
        /// </summary>
        /// <param name="id"></param>
        public void DeleteComicBookArtist(int id)
        {
            var comicBookArtist = new ComicBookArtist()
            {
                Id = id
            };
            _context.Entry(comicBookArtist).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        /// <summary>
        /// Find if that this artist and role combination doesn't already exist for this comic book
        /// </summary>
        /// <param name="artistId"></param>
        /// <param name="roleId"></param>
        /// <param name="comicBookId"></param>
        /// <returns></returns>
        public bool FindDuplicateArtistRoleCombination(int artistId, int roleId, int comicBookId)
        {
            return _context.ComicBookArtists
                .Any(cba => cba.ArtistId == artistId && 
                            cba.RoleId == roleId && 
                            cba.ComicBookId == comicBookId);
        }
    }
}
