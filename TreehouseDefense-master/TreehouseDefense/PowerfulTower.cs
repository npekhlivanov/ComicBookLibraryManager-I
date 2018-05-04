using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreehouseDefense
{
    class PowerfulTower : Tower
    {
        public PowerfulTower(MapLocation location) : base(location)
        {
        }

        protected override int Power { get; } = 2;
    }
}
