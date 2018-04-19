using System;
using Xunit;

namespace TreehouseDefense.Tests
{
    public class MapTests
    {
        [Fact()]
        public void MapTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Map(1, 0));
        }
    }
}