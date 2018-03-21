using ComicBookShared.Data.Queries;
using ComicBookShared.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Series" section of the website.
    /// </summary>
    public class SeriesController : BaseController
    {
        public ActionResult Index()
        {
            // Get the series list
            var series = Context.Series.ToList();

            return View(series);
        }

        public ActionResult Detail(int? id)
        {
            Series getMethod(int x) => new GetSeriesQuery(Context).Execute(x, includeComicBooks: true);
            var series = GetEntity<Series>(id, getMethod, out ActionResult resultIfNotFound);
            if (series == null)
            {
                return resultIfNotFound;
            }

            // Sort the comic books
            series.ComicBooks = series.ComicBooks
                .OrderByDescending(cb => cb.IssueNumber)
                .ToList();

            return View(series);
        }

        public ActionResult Add()
        {
            var series = new Series();

            return View(series);
        }

        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title")] Series series)
        {
            ValidateSeries(series);
            if (!ModelState.IsValid)
            {
                return View(series);
            }

            // Add the series
            Context.Add(Context.Series, series);

            TempData["Message"] = "Your series was successfully added!";

            return RedirectToAction("Detail", new { id = series.Id });
       }

        public ActionResult Edit(int? id)
        {
            Series getMethod(int x) => new GetSeriesQuery(Context).Execute(x, includeComicBooks: false);
            var series = GetEntity<Series>(id, getMethod, out ActionResult resultIfNotFound);
            if (series == null)
            {
                return resultIfNotFound;
            }

            return View(series);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "Id, Title, Description")] Series series)
        {
            ValidateSeries(series);
            if (!ModelState.IsValid)
            {
               return View(series);
            }
            
            // Update the series
            if (Context.Update<Series>(Context.Series, series))
            {
                TempData["Message"] = "Your series was successfully updated!";
            }
            else
            {
                TempData["Message"] = "No changes to save!";
            }

            return RedirectToAction("Detail", new { id = series.Id });
        }


        public ActionResult Delete(int? id)
        {
            Series getMethod(int x) => new GetSeriesQuery(Context).Execute(x, includeComicBooks: false);
            var series = GetEntity<Series>(id, getMethod, out ActionResult resultIfNotFound);
            if (series == null)
            {
                return resultIfNotFound;
            }

            return View(series);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Delete the series
            if (Context.Delete<Series>(Context.Series, id))
            {
                TempData["Message"] = string.Format("The series \"{0}\" was successfully deleted!", TempData["ItemName"]);
            }
            else
            {
                TempData["Message"] = string.Format("The series \"{0}\" has been deleted by another user!", TempData["ItemName"]);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Validates a series on the server
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="series">The series to validate.</param>
        private void ValidateSeries(Series series)
        {
            // If there aren't any "Title" field validation errors...
            if (ModelState.IsValidField("Title"))
            {
                // Then make sure that the provided title is unique.
                var exists = Context.Series.Any(s => s.Title.Equals(series.Title, System.StringComparison.OrdinalIgnoreCase) && s.Id != series.Id);
                if (exists)
                {
                    ModelState.AddModelError("Title", "The provided Title is in use by another series.");
                }
            }
        }
    }
}