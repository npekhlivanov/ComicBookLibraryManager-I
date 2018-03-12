using ComicBookShared.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ComicBookShared.Data
{
    public class ComicBookArtistsRepository : BaseRepository<ComicBookArtist>
    {
        public ComicBookArtistsRepository(Context context) : base(context)
        {
        }

        /// <summary>
        /// Get the comic book artist, include the "ComicBook.Series", "Artist", and "Role" navigation properties
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ComicBookArtist Get(int id)
        {
            return Context.ComicBookArtists
                .Include(cba => cba.ComicBook.Series)
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Where(cba => cba.Id == id)
                .SingleOrDefault();
        }

        public override IList<ComicBookArtist> GetList()
        {
            throw new System.NotImplementedException();
        }

        public override void Update(ComicBookArtist entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
