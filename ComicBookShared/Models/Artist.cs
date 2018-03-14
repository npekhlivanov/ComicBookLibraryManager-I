using ComicBookShared.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComicBookShared.Models
{
    /// <summary>
    /// Represents a comic book artist.
    /// </summary>
    public class Artist : BaseEntity
    {
        public Artist()
        {
            ComicBooks = new List<ComicBookArtist>();
        }

        //public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
        
        public ICollection<ComicBookArtist> ComicBooks { get; set; }
    }
}
