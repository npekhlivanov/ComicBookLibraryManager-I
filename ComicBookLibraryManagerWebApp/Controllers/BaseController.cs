using ComicBookShared.Data;
using System.Net;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    public abstract class BaseController : Controller
    {
        private bool _disposed = false;

        protected Context Context { get; private set; }

        protected Repository Repository { get; private set; }

        public BaseController()
        {
            Context = new Context();
            Repository = new Repository(Context);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Context.Dispose();
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //filterContext.HttpContext.Application.Add("DbId", 1);
            //filterContext.HttpContext.Session.SessionID
            //filterContext.HttpContext.Cache.Add();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected delegate TEntity GetEntityDelegate<TEntity>(int id);

        protected TEntity GetEntity<TEntity>(int? id, GetEntityDelegate<TEntity> getMethod, out ActionResult resultIfNotFound) 
            where TEntity : class
        {
            if (!id.HasValue)
            {
                resultIfNotFound = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return null;
            }

            var entity = getMethod(id.Value);
            if (entity == null)
            {
                resultIfNotFound = HttpNotFound();
                return null;
            }

            resultIfNotFound = null;
            return entity;
        }

    }
}