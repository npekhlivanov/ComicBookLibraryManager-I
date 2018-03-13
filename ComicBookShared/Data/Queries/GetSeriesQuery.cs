using ComicBookShared.Models;
using System.Data.Entity;
using System.Linq;

namespace ComicBookShared.Data.Queries
{
    public class GetSeriesQuery : BaseQuery
    {
        public GetSeriesQuery(Context context) : base(context)
        {
        }

        public Series Execute(int id, bool includeComicBooks)
        {
            var series = Context.Series.Where(s => s.Id == id);
            if (includeComicBooks)
            {
                series = series.Include(s => s.ComicBooks);
            }

            return series.SingleOrDefault();
        }
    }
}