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

			s1.Intersect(s2, includeBoundaryPoints, includeOverlap).Should().Be(includeOverlap);
		}

		[Test]
		public void CheckIntersection_WhenCollinearAndNotIntersect(
			[Values(true, false)] bool includeBoundaryPoints,
			[Values(true, false)] bool includeOverlap)
		{
			var s1 = new Segment(new PointX(1, 1), new PointX(2, 3));
			var s2 = new Segment(new PointX(3, 5), new PointX(4, 7));

			s1.Intersect(s2, includeBoundaryPoints, includeOverlap).Should().BeFalse();
		}

		[Test]
		public void CheckIntersection_WhenCollinearAndSame(
			[Values(true, false)] bool includeBoundaryPoints,
			[Values(true, false)] bool includeOverlap)
		{
			var s1 = new Segment(new PointX(1, 1), new PointX(2, 3));
			var s2 = new Segment(new PointX(1, 1), new PointX(2, 3));

			s1.Intersect(s2, includeBoundaryPoints, includeOverlap).Should().Be(includeBoundaryPoints);
		}

		[Test]
		public void CheckIntersection_WhenCollinearAndHaveCommonBoundaryPoint(
			[Values(true, false)] bool includeBoundaryPoints,
			[Values(true, false)] bool includeOverlap)
		{
			var s1 = new Segment(new PointX(1, 1), new PointX(2, 3));
			var s2 = new Segment(new PointX(2, 3), new PointX(3, 5));

			s1.Intersect(s2, includeBoundaryPoints, includeOverlap).Should().Be(includeBoundaryPoints);
		}

		[Test]
		public void CheckIntersection_WhenHaveCommonBoundaryPoint(
			[Values(true, false)] bool includeBoundaryPoints,
			[Values(true, false)] bool includeOverlap)
		{
			var s1 = new Segment(new PointX(1, 1), new PointX(1.5, 3));
			var s2 = new Segment(new PointX(1.5, 3), new PointX(1.5, 7));

			s1.Intersect(s2, includeBoundaryPoints, includeOverlap).Should().Be(includeBoundaryPoints);
		}

		[Test]
		public void CheckIntersection_WhenIntersect(
			[Values(true, false)] bool includeBoundaryPoints,
			[Values(true, false)] bool includeOverlap)
		{
			var s1 = new Segment(new PointX(4, 1), new PointX(12, 3));
			var s2 = new Segment(new PointX(5, 5), new PointX(10, 1));

			s1.Intersect(s2, includeBoundaryPoints, includeOverlap).Should().BeTrue();
		}

		[Test]
		public void CheckIntersection_WhenNotIntersect(
			[Values(true, false)] bool includeBoundaryPoints,
			[Values(true, false)] bool includeOverlap)
		{
			var s1 = new Segment(new PointX(4, 1), new PointX(12, 3));
			var s2 = new Segment(new PointX(8, 4), new PointX(8, 8));

			s1.Intersect(s2, includeBoundaryPoints, includeOverlap).Should().BeFalse();
		}
	}
}
