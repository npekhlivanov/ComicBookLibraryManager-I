﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ComicBookShared.Models
{
    /// <summary>
    /// Represents a comic book series.
    /// </summary>
    public class Series : BaseEntity
    {
        public Series()
        {
            ComicBooks = new List<ComicBook>();
        }

        //public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<ComicBook> ComicBooks { get; set; }
    }
}
