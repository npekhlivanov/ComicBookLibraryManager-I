using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data.Queries
{
    public abstract class BaseQuery
    {
        protected Context Context { get; private set; }

        public BaseQuery(Context context)
        {
            Context = context;
        }
    }
}
