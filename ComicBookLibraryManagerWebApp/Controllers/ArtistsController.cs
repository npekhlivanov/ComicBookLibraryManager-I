using ComicBookShared.Data.Queries;
using ComicBookShared.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Artists" section of the website.
    /// </summary>
    public class ArtistsController : BaseController
    {
        public ActionResult Index()
        {
            // Get the artists list
            var artists = new GetArtistListQuery(Context).Execute();
            return View(artists);
        }

        public ActionResult Detail(int? id)
        {
            // use Lambda expression with multiple statements in its body to sort and prepare the view
            return PrepareView<Artist>(id, x => new GetArtistQuery(Context).Execute(x, includeRelatedEntities: true), artist =>
            {
                // Sort the comic books
                artist.ComicBooks = artist.ComicBooks
                    .OrderBy(cb => cb.ComicBook.Series.Title)
                    .OrderByDescending(cb => cb.ComicBook.IssueNumber)
                    .ToList();

                return View(artist);
            });
        }

        public ActionResult Add()
        {
            var artist = new Artist();

            return View(artist);
        }

        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] Artist artist)
        {
            ValidateArtist(artist);
            if (!ModelState.IsValid)
            {
                return View(artist);
            }

            // Add the artist
            Context.Add<Artist>(artist);
            //Context.Artists.Add(artist);
            //Context.SaveChanges();

            TempData["Message"] = "Your artist was successfully added!";

            return RedirectToAction("Detail", new { id = artist.Id });
        }

        public ActionResult Edit(int? id)
        {
            return PrepareView<Artist>(id, x => new GetArtistQuery(Context).Execute(x, includeRelatedEntities: false), x => View(x));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "Id,Name")] Artist artist)
        {
            ValidateArtist(artist);
            if (!ModelState.IsValid)
            {
                return View(artist);
            }

            // Update the artist
            if (Context.Update<Artist>(artist)) // TODO: implement 3-state logic
            {
                TempData["Message"] = "Your artist was successfully updated!";
            }
            else
            {
                TempData["Message"] = "No changes to save!";
            }

            //var unmodifiedArtist = Context.Artists.Find(artist.Id);
            //if (!CheckIfArtistExists(unmodifiedArtist))
            //{
            //    return RedirectToAction("Index");
            //}

            //Context.Entry(unmodifiedArtist).CurrentValues.SetValues(artist);
            //if (Context.Entry(unmodifiedArtist).State != System.Data.Entity.EntityState.Unchanged)
            //{
            //    Context.SaveChanges();
            //    TempData["Message"] = "Your artist was successfully updated!";
            //}
            //else
            //{
            //    TempData["Message"] = "No changes to save!";
            //}

            return RedirectToAction("Detail", new { id = artist.Id });
        }

        public ActionResult Delete(int? id)
        {
            return PrepareView<Artist>(id, x => new GetArtistQuery(Context).Execute(x, includeRelatedEntities: false));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Delete the artist
            if (!Context.Delete<Artist>(id))
            {
                TempData["Message"] = string.Format("The artist \"{0}\" has been deleted by another user!", TempData["ItemName"]);
            }
            else
            {
                TempData["Message"] = string.Format("The artist \"{0}\" was successfully deleted!", TempData["ItemName"]);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Validates an artist on the server 
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="artist">The artist to validate.</param>
        private void ValidateArtist(Artist artist)
        {
            // If there aren't any "Name" field validation errors...
            if (ModelState.IsValidField("Name"))
            {
                // Then make sure that the provided name is unique.
                var exists = Context.Artists.Any(a => a.Name.Equals(artist.Name, StringComparison.InvariantCultureIgnoreCase) && a.Id != artist.Id && !a.IsDeleted);
                if (exists)
                {
                    ModelState.AddModelError("Name",
                        "The provided Name is in use by another artist.");
                }
            }
        }
    }
}
