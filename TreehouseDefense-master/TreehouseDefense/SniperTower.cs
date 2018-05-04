using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreehouseDefense
{
    class SniperTower : Tower
    {
        public SniperTower(MapLocation location) : base(location)
        {
        }

        protected override int Range { get; } = 2;

        protected override double Accuracy { get; } = 1.0;
    }
}
