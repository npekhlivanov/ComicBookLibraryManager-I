using ComicBookShared.Data;
using ComicBookShared.Models;
using System.Web.Mvc;

namespace ComicBookLibraryManagerWebApp.ViewModels
{
    /// <summary>
    /// Base view model class for the "Add Comic Book" 
    /// and "Edit Comic Book" views.
    /// </summary>
    public abstract class ComicBooksBaseViewModel
    {
        public ComicBook ComicBook { get; set; } = new ComicBook();

        public SelectList SeriesSelectListItems { get; set; }

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        public virtual void Init(Repository repository)
        {
            SeriesSelectListItems = new SelectList(
                repository.GetSeriesList(),
                "Id", "Title");
        }
    }
}