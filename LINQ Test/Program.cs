using SimpleBirdWatcher;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_Test
{
    static class ListExtensions
    {
        public static IList<T> ReverseOrder<T>(this List<T> list) => 
            list
            .OrderByDescending(n => n)
            .ToList();
    }

    class Program
    {
        delegate void SayGreeting(string name);

        static void SayHello(string name)
        {
            Console.WriteLine(string.Format("Hello, {0}", name));
        }

        //public Func<int, int> Square = delegate (int number) { return number * number; };
        public Func<int, int> Square = (number) => number * number; 

        static void Main(string[] args)
        {


            List<int> numbers = new List<int> { 2, 4, 8, 16, 32 };
            var numbersReversed = numbers.ReverseOrder<int>();
            //BasicQuerying();
            //TestMethodSyntax(); 
            TestBirds();
        }

        static void TestDelegates()
        {
            // these three delegate declarations produce exactly the same code
            var sayGreeting = new SayGreeting(SayHello);
            SayGreeting sayGreeting1 = SayHello;
            Action<string> sayGreeting2 = SayHello;

            sayGreeting("Nick");

            // use an anonymous method instead of a named one
            Action<string> sayBye = (name) => Console.WriteLine(string.Format("Bye, {0}", name));
   
            sayBye("Nick");
        }

        static void TestQuerySyntax()
        {
            List<int> numbers = new List<int> { 2, 4, 8, 16, 32 };

            IEnumerable<int> evenNumbers = (from number in numbers
                               where number > 10
                               select number);

            // collection of objects
            var birds = new SimpleBirdRepository().GetBirds();

            // query projection - use anonymous type for the result
            var redBirdds = from b in birds
                            where b.Color.Equals("Red", StringComparison.OrdinalIgnoreCase) && b.Sightings > 2
                            select new { BirdName = b.Name, b.Sightings };

            // create object of anonymous type; IMPORTANT: properties are R/O!
            var anonymousPidgeon = new { Name = "Pidgeon", Color = "White", Sightings = 10 };

            var canary = new SimpleBird { Name = "Canary", Color = "Yellow", Sightings = 0 };
            birds.Add(canary);

            var matchingBirds = from b in birds
                                where b.Color != anonymousPidgeon.Color
                                orderby b.Name descending, b.Color
                                select new { BirdName = b.Name };

            // grouping; use "into" to include grouping result in a "where" clause
            var birdCol = from b in birds
                          group b by b.Color
                          into birdsByColor
                          where birdsByColor.Count() > 1
                          select new { Color = birdsByColor.Key, Count = birdsByColor.Count() };
        }

        static void TestMethodSyntax()
        {
            var birds = new SimpleBirdRepository().GetBirds();

            // use method syntax instead of query syntax
            var redBirds = birds
                .Where(b => b.Color.Equals("red", StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.Name)
                .ThenByDescending(b => b.Sightings)
                .Select(b => new { BirdName = b.Name, b.Sightings });

            // quantifiers: Any(), All()
            // take elements: First[OrDefault](), Single[OrDefault](), Last[OrDefault](), ElementAt[OrDefault]()

            
            // partitioning operators: Skip(), Take(), SkipWhile(), TakeWhile()
            var birdsWithLongName = birds
                .OrderBy(b => b.Name.Length)
                .SkipWhile(b => b.Name.Length < 6);


            // join operators: Join(), GroupJoin()
            var colors = new List<string> { "Red", "White", "Purple" };
            // use query syntax
            var favColorBirds = from b in birds
                                join c in colors on b.Color equals c
                                select b;
            // use method syntax
            var theSameBirds = birds.Join(colors, b => b.Color, c => c, (b, c) => b);

            var groupedBirds = colors.GroupJoin(
                birds,
                c => c,
                b => b.Color,
                (c, b) => new { Color = c, Birds = b });

            var allGroupedBirds = groupedBirds.SelectMany(g => g.Birds);


            // aggregations: Count(), Sum(), Average(), Min(), Max()
            var birdCountByColor = birds
                .GroupBy(b => b.Color)
                .Select(g => new { Color = g.Key, Count = g.Count(), Sightings = g.Sum(b => b.Sightings) }); // note the use of property "Key" - the grouping field

            
            // set operators: Distinct(), Except(), Union(), Intersect(), Concat()
            birds.Add(new SimpleBird { Name = "Pelican", Color = "Pink", Sightings = 6 });
            var birdColors = birds.Select(b => b.Color).Distinct(); // remove duplicates
            var colorWithNoBirds = colors.Except(birds.Select(b => b.Color).Distinct());
            var allColors = colors.Union(birds.Select(b => b.Color).Distinct());
            var matchingColors = colors.Intersect(birds.Select(b => b.Color).Distinct());
            var combinedColors = colors.Concat(birds.Select(b => b.Color).Distinct()); // does not remove duplicates in the sets

            
            // generation operators: Range(), Repeat(), Empty(), DefaultIfEmpty()
            var numbers = Enumerable.Range(0, 10);
            var blankBirds = Enumerable.Repeat(new SimpleBird(), 5);
            var emptyBirds = Enumerable.Empty<SimpleBird>();
            var emptyBird = emptyBirds.DefaultIfEmpty(); // returns null

            // conversion operators: ToList(), ToArray(), AsEnumerable(), ToDictionary(), ...
        }

        private class BirdComparer : EqualityComparer<Bird>
        {
            public override bool Equals(Bird x, Bird y)
            {
                return x.CommonName.Equals(y.CommonName);
            }

            public override int GetHashCode(Bird obj)
            {
                return obj.CommonName.GetHashCode();
            }
        }
        static void TestBirds()
        {
            var birds = BirdRepository.LoadBirds();
            var sightings = birds.SelectMany(b => b.Sightings);
            var avgS = birds.Select(b => b.Sightings.Count()).Average();
            var sightingsByCountry = sightings
                .GroupBy(s => s.Place.Country)
                .Select(g => new { Country = g.Key, Sightings = g.Count() });
            var birdCountByStatus = birds
                .Where(b => b.ConservationStatus != "LeastConcern" && b.ConservationStatus != "NearThreatened")
                .GroupBy(b => b.ConservationStatus)
                .Select(selector: g => new { Status = g.Key, Count = g.Count(), Sightings = g.Sum(b => b.Sightings.Count) });
            var statuses = birds
                .Select(b => b.ConservationStatus)
                .Where(s => s != "LeastConcern" && s != "NearThreatened")
                .Distinct();
            var endangeredSightings = birds
                .Join(statuses, b => b.ConservationStatus, s => s, (b, s) => new { Status = s, Sigthings = b.Sightings })
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Sightings = g.Sum(selector: s => s.Sigthings.Count) });

            var importedBirds = BirdRepository.LoadImportedBirds();
            var matchingBirds = birds.Intersect(importedBirds, new BirdComparer());
            // Perform outer join to get elements that exist in one enumerable and not in the other
            var newBirds = importedBirds.GroupJoin(birds,
                ib => ib.CommonName,
                b => b.CommonName,
                (ib, b) => new { ImportedBird = ib, Birds = b })
                // Variant 1: flatten the grouping first, providing default value for elements with no matches, then filter records
                //.SelectMany(gb => gb.Birds.DefaultIfEmpty(), (gb, b) => new { ImportedBird = gb.ImportedBird, Bird = b })
                //.Where(a => a.Bird == null)

                // Variant 2: filter the grouping directly - my option
                .Where(a => a.Birds.Count() == 0)

                .Select(a => a.ImportedBird)
                .ToList();

            var searchParams = new BirdSearch
            {
                Country = "United States",
                Colors = new List<string> { "White", "Brown", "Black" },
                Page = 0,
                PageSize = 5
            };
            var foundBirds = birds.Search(searchParams);
        }
    }


}
