using Accord.Math;
using FluentAssertions;
using NUnit.Framework;

namespace DS.Tests
{
    [TestFixture]
    public class AccordTests
    {
        [Test]
        public void AccordPowTest()
        {
            var vector = new []{ 2, 3, 4 };
            vector.Pow(2).Should().BeEquivalentTo(new[] { 4, 9, 16 });
        }
    }
}
