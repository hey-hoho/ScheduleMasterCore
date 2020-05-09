using System;
using Xunit;

namespace Hos.ScheduleMaster.xUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void Test(int value)
        {
            Assert.False(value == 0, $"{value} should not be 0");
        }
    }
}
