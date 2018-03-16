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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the series
            var series = new GetSeriesQuery(Context).Execute(id.Value, includeComicBooks: true);

            if (series == null)
            {
                return HttpNotFound();
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

        [HttpPost]
        public ActionResult Add(Series series)
        {
            ValidateSeries(series);

            if (ModelState.IsValid)
            {
                // Add the series
                Context.Add(Context.Series, series);

                TempData["Message"] = "Your series was successfully added!";

                return RedirectToAction("Detail", new { id = series.Id });
            }

            return View(series);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the series
            var series = new GetSeriesQuery(Context).Execute(id.Value, includeComicBooks: false);

            if (series == null)
            {
                return HttpNotFound();
            }

            return View(series);
        }

        [HttpPost]
        public ActionResult Edit(Series series)
        {
            ValidateSeries(series);

            if (ModelState.IsValid)
            {
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

            return View(series);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the series
            var series = new GetSeriesQuery(Context).Execute(id.Value, includeComicBooks: false);
            if (series == null)
            {
                return HttpNotFound();
            }

            return View(series);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Delete the series
            if (Context.Delete<Series>(Context.Series, id))
            {
                TempData["Message"] = "Your series was successfully deleted!";
            }
            else
            {
                TempData["Message"] = "The artist has been deleted by another user!";
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