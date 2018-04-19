using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TreehouseDefenseTests1.Mocks;

namespace TreehouseDefense.Tests
{
    [TestClass()]
    public class TowerTests
    {
        [TestMethod()]
        [DataRow(3, 3)]
        public void FireOnInvadersDecreasesInvadersHealth(int x, int y)
        {
            var map = new Map(x, y);
            var target = new Tower(new MapLocation(0, 0, map));
            var invaders = new InvaderMock[]
            {
                new InvaderMock() { Location = new MapLocation(0, 0, map) },
                new InvaderMock() { Location = new MapLocation(0, 0, map) }
            };

            target.FireOnInvaders(invaders);

            Assert.IsTrue(invaders.All(i => i.Health == 1));
        }
    }
}