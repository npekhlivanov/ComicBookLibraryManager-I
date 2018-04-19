using Xunit;
using TreehouseDefense;

namespace TreehouseDefense.Tests
{
    public class CalculatorTests
    {
        [Fact]
        public void Initialization()
        {
            var expected = 1.1;
            var target = new Calculator(1.1);
            Assert.Equal(expected, target.Result, 1);
        }

        [Fact]
        public void BasicAdd()
        {
            var target = new Calculator(1.1);
            target.Add(2.2);
            var expected = 3.3;
            Assert.Equal(expected, target.Result, 1);
        }

        [Theory]
        [InlineData(3.3, 2.2, 1.1)]
        [InlineData(0.003, 0.004, -0.001)]
        public void SubtractTest(double val1, double val2, double result)
        {
            var target = new Calculator(val1);
            target.Subtract(val2);
            var expected = result;
            Assert.Equal(expected, target.Result, 3);
        }
    }

}
