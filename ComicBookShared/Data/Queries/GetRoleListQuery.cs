using ComicBookShared.Models;
using System.Collections.Generic;
using System.Linq;

namespace ComicBookShared.Data.Queries
{
    public class GetRoleListQuery : BaseQuery
    {
        public GetRoleListQuery(Context context) : base(context)
        {
        }

        public IList<Role> Execute()
        {
            return Context.Roles
                .OrderBy(a => a.Name)
                .ToList();
        }
    }
}
