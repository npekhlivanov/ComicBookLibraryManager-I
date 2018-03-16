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
