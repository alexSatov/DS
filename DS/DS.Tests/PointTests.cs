using DS.MathStructures;
using FluentAssertions;
using NUnit.Framework;

namespace DS.Tests
{
    [TestFixture]
    public class Point_Should
    {
        [Test]
        public void GetCenterPoint()
        {
            var p1 = new PointX(1, 1);
            var p2 = new PointX(3, 3);

            p1.GetCenterPointWith(p2).Should().BeEquivalentTo(new PointX(2, 2));
        }
    }
}
