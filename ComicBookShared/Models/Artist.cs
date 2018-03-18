using System;
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

        [Required, StringLength(100)]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedOn { get; set; }

        public ICollection<ComicBookArtist> ComicBooks { get; set; }
    }
}
