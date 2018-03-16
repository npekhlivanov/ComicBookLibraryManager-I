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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the artist
            var artist = new GetArtistQuery(Context).Execute(id.Value, includeRelatedEntities: true);

            if (artist == null)
            {
                return HttpNotFound();
            }

            // Sort the comic books.
            artist.ComicBooks = artist.ComicBooks
                .OrderBy(cb => cb.ComicBook.Series.Title)
                .OrderByDescending(cb => cb.ComicBook.IssueNumber)
                .ToList();

            return View(artist);
        }

        public ActionResult Add()
        {
            var artist = new Artist();

            return View(artist);
        }

        [HttpPost]
        public ActionResult Add(Artist artist)
        {
            ValidateArtist(artist);

            if (ModelState.IsValid)
            {
                // Add the artist
                Context.Add<Artist>(artist);
                //Context.Artists.Add(artist);
                //Context.SaveChanges();

                TempData["Message"] = "Your artist was successfully added!";

                return RedirectToAction("Detail", new { id = artist.Id });
            }

            return View(artist);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the artist
            var artist = new GetArtistQuery(Context).Execute(id.Value, includeRelatedEntities: false);

            if (artist == null)
            {
                return HttpNotFound();
            }

            return View(artist);
        }

        [HttpPost]
        public ActionResult Edit(Artist artist)
        {
            ValidateArtist(artist);

            if (ModelState.IsValid)
            {
                // Update the artist
                if (Context.Update<Artist>(artist))
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

            return View(artist);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the artist
            var artist = new GetArtistQuery(Context).Execute(id.Value, includeRelatedEntities: false);
            if (artist == null)
            {
                return HttpNotFound();
            }

            return View(artist);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Delete the artist
            if (!Context.Delete<Artist>(id))
            {
                TempData["Message"] = "The artist has been deleted by another user!";
            }
            else
            {
                TempData["Message"] = "Your artist was successfully deleted!";
            }

            //var artist = Context.Artists.Find(id);
            //if (!CheckIfArtistExists(artist))
            //{
            //    return RedirectToAction("Index");
            //    //return View(new Artist());
            //}

            //Context.Artists.Remove(artist);
            //Context.SaveChanges();

            //TempData["Message"] = "Your artist was successfully deleted!";

            return RedirectToAction("Index");
        }

        //private bool CheckIfArtistExists(Artist artist)
        //{
        //    if (artist == null)
        //    {
        //        TempData["Message"] = "The artist has been deleted by another user!";
        //        return false;
        //    }

        //    return true;
        //}

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
