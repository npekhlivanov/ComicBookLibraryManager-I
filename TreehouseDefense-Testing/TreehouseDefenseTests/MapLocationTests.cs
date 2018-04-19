using Xunit;

namespace TreehouseDefense.Tests
{
    public class MapLocationTests
    {
        [Fact()]
        public void ShouldThrowIfNotOnMap()
        {
            var map = new Map(3, 3);
            Assert.Throws<OutOfBoundsException>(() => new MapLocation(3, 3, map));
        }

        [Fact()]
        public void InRangeOfTest()
        {
            var map = new Map(3, 3);
            var target = new MapLocation(0, 0, map);
            Assert.True(target.InRangeOf(new MapLocation(1, 0, map), 1));
        }
    }
}