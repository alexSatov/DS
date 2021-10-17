using System;
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

        [Test]
        public void AngleTest()
        {
            Tools.Angle(1, 0).Should().Be(0);
            Tools.Angle(0.0, 1.0).Should().Be(Math.PI / 2);
            Tools.Angle(Math.Sqrt(3) / 2, 0.5).Should().BeApproximately(Math.PI / 6, 0.00001);
            Tools.Angle(-Math.Sqrt(3) / 2, 0.5).Should().BeApproximately(5 * Math.PI / 6, 0.00001);
            Tools.Angle(-Math.Sqrt(3) / 2, -0.5).Should().BeApproximately(7 * Math.PI / 6, 0.00001);
        }
    }
}
