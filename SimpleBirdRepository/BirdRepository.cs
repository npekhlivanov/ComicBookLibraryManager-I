using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBirdWatcher
{
    public class SimpleBirdRepository
    {
        private readonly List<SimpleBird> _birds;

        public SimpleBirdRepository()
        {
            _birds = new List<SimpleBird>
            {
                new SimpleBird { Name = "Cardinal", Color = "Red", Sightings = 2 },
                new SimpleBird { Name = "Dove", Color = "White", Sightings = 3 }
            };

            _birds.Add(new SimpleBird { Name = "Robin", Color = "Red", Sightings = 5 });
        }

        public IList<SimpleBird> GetBirds()
        {
            return (from b in _birds
                   select b).ToList();
        }
    }
}
