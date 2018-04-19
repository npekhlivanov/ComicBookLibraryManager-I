using ComicBookShared.Data;
using System;
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

        // Declare a delagate to use the traditional way; alternatively, use Func<> 
        //protected delegate TEntity GetEntityDelegate<TEntity>(int id);

        protected TEntity GetModel<TEntity>(int? id, Func<int, TEntity> getMethod, out ActionResult resultIfNotFound) 
            where TEntity : class
        {
            if (!id.HasValue)
            {
                resultIfNotFound = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return null;
            }

            var model = getMethod(id.Value);
            if (model == null)
            {
                resultIfNotFound = HttpNotFound();
                return null;
            }

            resultIfNotFound = null;
            return model;
        }

        /// <summary>
        /// Method to prepare a View with the data from a data entity of specified type
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity</typeparam>
        /// <param name="id">The Id of the entity</param>
        /// <param name="fetchMethod">Method to fetch the entity, e.g. from a DB/Repository/Service</param>
        /// <param name="prepareMethod">Method to prepare the view from the entity fetched (optional)</param>
        /// <returns>An ActionResult object to display</returns>
        protected ActionResult PrepareView<TEntity>(int? id, Func<int, TEntity> fetchMethod, Func<TEntity, ActionResult> prepareMethod = null)
            where TEntity : class
        {
            var model = GetModel(id, fetchMethod, out ActionResult resultIfNotFound);
            if (model == null)
            {
                return resultIfNotFound;
            }

            if (prepareMethod == null)
            {
                return View(model);
            }

            // Invoke the method to prepare the view, e.g. by filling a view model
            return prepareMethod(model);
        }
    }
}