using ComicBookShared.Models;
using System.Collections.Generic;
using System.Linq;

namespace ComicBookShared.Data.Queries
{
    public class GetArtistListQuery : BaseQuery
    {
        public GetArtistListQuery(Context context) : base(context)
        {
        }

        public IList<Artist> Execute()
        {
            return Context.Artists
                .OrderBy(a => a.Name)
                .ToList();
        }
    }
}
