using ComicBookShared.Models;
using System.Data.Entity;
using System.Linq;

namespace ComicBookShared.Data.Queries
{
    public class GetArtistQuery : BaseQuery
    {
        public GetArtistQuery(Context context) : base (context)
        {
        }

        public Artist Execute(int id, bool includeRelatedEntities)
        {
            var artistQuery = Context.Artists.Where(a => a.Id == id);
            if (includeRelatedEntities)
            {
                artistQuery = artistQuery
                    //.Include(a => a.ComicBooks) -> not needed, because it is included in the next includes
                    .Include(a => a.ComicBooks.Select(cba => cba.Role))
                    .Include(a => a.ComicBooks.Select(cba => cba.ComicBook.Series));
            }

            return artistQuery.SingleOrDefault();
        }
    }
}
