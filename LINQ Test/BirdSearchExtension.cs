using SimpleBirdWatcher;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_Test
{
    public class BirdSearch
    {
        public string CommonName { get; set; }
        public List<string> Colors { get; set; }
        public string Country { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    static class BirdSearchExtension
    {
        public static IEnumerable<Bird>Search(this IEnumerable<Bird> source, BirdSearch search)
        {
            return source
                .Where(s => string.IsNullOrEmpty(search.CommonName) || s.CommonName.Contains(search.CommonName))
                .Where(s => string.IsNullOrEmpty(search.Country) || s.Habitats.Any(h => h.Country.Contains(search.Country)))
                .Where(s => search.Colors.Any(c => s.PrimaryColor.Equals(c) || s.SecondaryColor.Equals(c)) ||
                            search.Colors.Join(s.TertiaryColors, sc => sc, tc => tc, (sc, tc) => sc).Any())
                .Skip(search.Page * search.PageSize)
                .Take(search.PageSize);
        }
    }
}
