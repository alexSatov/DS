using DS.MathStructures;
using FluentAssertions;
using NUnit.Framework;

namespace DS.Tests
{
    [TestFixture]
    public class Segment_Should
    {
        [Test]
        public void CheckIntersection_WhenCollinearAndIntersect(
            [Values(true, false)] bool includeBoundaryPoints,
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(1, 1), new PointX(2, 3));
            var s2 = new Segment(new PointX(1.5, 2), new PointX(2.5, 4));

            s1.GetIntersection(s2, includeBoundaryPoints, includeOverlap).Should()
                .Be(new IntersectPoint(includeOverlap, null));
        }

        [Test]
        public void CheckIntersection_WhenCollinearAndNotIntersect(
            [Values(true, false)] bool includeBoundaryPoints,
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(1, 1), new PointX(2, 3));
            var s2 = new Segment(new PointX(3, 5), new PointX(4, 7));

            s1.GetIntersection(s2, includeBoundaryPoints, includeOverlap).Should()
                .Be(new IntersectPoint(false, null));
        }

        [Test]
        public void CheckIntersection_WhenCollinearAndSame(
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(1, 1), new PointX(2, 3));
            var s2 = new Segment(new PointX(1, 1), new PointX(2, 3));

            s1.GetIntersection(s2, true, includeOverlap).Should()
                .Be(new IntersectPoint(true, new PointX(1, 1)));
            s1.GetIntersection(s2, false, includeOverlap).Should()
                .Be(new IntersectPoint(false, null));
        }

        [Test]
        public void CheckIntersection_WhenCollinearAndHaveCommonBoundaryPoint(
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(1, 1), new PointX(2, 3));
            var s2 = new Segment(new PointX(2, 3), new PointX(3, 5));

            s1.GetIntersection(s2, true, includeOverlap).Should()
                .Be(new IntersectPoint(true, new PointX(2, 3)));
            s1.GetIntersection(s2, false, includeOverlap).Should()
                .Be(new IntersectPoint(false, null));
        }

        [Test]
        public void CheckIntersection_WhenHaveCommonBoundaryPoint(
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(1, 1), new PointX(1.5, 3));
            var s2 = new Segment(new PointX(1.5, 3), new PointX(1.5, 7));

            s1.GetIntersection(s2, true, includeOverlap).Should()
                .Be(new IntersectPoint(true, new PointX(1.5, 3)));
            s1.GetIntersection(s2, false, includeOverlap).Should()
                .Be(new IntersectPoint(false, null));
        }

        [Test]
        public void CheckIntersection_WhenIntersect(
            [Values(true, false)] bool includeBoundaryPoints,
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(1, 1), new PointX(5, 5));
            var s2 = new Segment(new PointX(1, 5), new PointX(5, 1));

            s1.GetIntersection(s2, includeBoundaryPoints, includeOverlap).Should()
                .Be(new IntersectPoint(true, new PointX(3, 3)));
        }

        [Test]
        public void CheckIntersection_WhenNotIntersect(
            [Values(true, false)] bool includeBoundaryPoints,
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(4, 1), new PointX(12, 3));
            var s2 = new Segment(new PointX(8, 4), new PointX(8, 8));

            s1.GetIntersection(s2, includeBoundaryPoints, includeOverlap).Should()
                .Be(new IntersectPoint(false, null));
        }

        [Test]
        public void CheckIntersection_WhenTouch(
            [Values(true, false)] bool includeBoundaryPoints,
            [Values(true, false)] bool includeOverlap)
        {
            var s1 = new Segment(new PointX(3, 2), new PointX(4, 3));
            var s2 = new Segment(new PointX(3, 4), new PointX(5, 2));

            s1.GetIntersection(s2, includeBoundaryPoints, includeOverlap).Should()
                .Be(new IntersectPoint(true, new PointX(4, 3)));
        }
    }
}
