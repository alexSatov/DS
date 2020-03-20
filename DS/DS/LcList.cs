using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;

namespace DS
{
    public class LcList : List<Lc>
    {
        public LcList(IEnumerable<Lc> lcs) : base(lcs)
        {
        }

        public (bool Found, int LcIndex, int Index) Find(PointX point)
        {
            for (var lcIndex = 0; lcIndex < Count; lcIndex++)
            {
                var index = this[lcIndex].IndexOf(point);
                if (index > -1)
                    return (true, lcIndex, index);
            }

            return (false, -1, -1);
        }

        public List<Segment> GetBorderSegments(bool handleHalfBorders = true, bool closeBorder = true)
        {
            var allSegments = this.SelectMany(lc => lc.Segments).ToList();
            var halfBorderSegmentResults = new ConcurrentQueue<IntersectSearchResult>();
            var borderSegments = allSegments
                .AsParallel()
                .Where(s =>
				{
					var notFoundOnStart = IsBorderPoint(s.Start, allSegments);
					var notFoundOnEnd = IsBorderPoint(s.End, allSegments);
                    var result = new IntersectSearchResult(!notFoundOnStart, !notFoundOnEnd, s);

                    if (result.HalfIntersect)
                        halfBorderSegmentResults.Enqueue(result);

					return result.NotFound;
				})
                .ToList();

			if (handleHalfBorders)
                AddHalfBorderSegments(borderSegments, allSegments, halfBorderSegmentResults);

            if (closeBorder)
                CloseBorder(borderSegments);

            return borderSegments;
        }

        public static bool IsBorderPoint(PointX point, IList<Segment> allSegments)
        {
            const double inf = 1000;

			foreach (var direction in Directions.All)
			{
				var segment = new Segment(point, point + direction * inf);

				if (!allSegments.Any(s => s.GetIntersection(segment, includeOverlap: true).IsIntersect))
					return true;
			}

			return false;
		}

        public static LcList FromAttractor(DeterministicModel model, IList<PointX> attractor, int count,
            int x1 = 20, int x2 = 40, int lcPointsCount = 100, double eps = 0.1)
        {
            Lc lc0X1 = null, lc0X2 = null;

            var lc0X1RawPoints = attractor.Where(p => Math.Abs(p.X2 - x2) < eps).ToList();
            if (lc0X1RawPoints.Count != 0)
            {
                var (minX1, maxX1) = (lc0X1RawPoints.Min(p => p.X1), lc0X1RawPoints.Max(p => p.X1));
                var stepX1 = (maxX1 - minX1) / lcPointsCount;
                var lc0X1Points = Enumerable.Range(0, lcPointsCount + 1)
                    .Select(i => minX1 + i * stepX1)
                    .Select(x1 => new PointX(x1, x2));

                lc0X1 = new Lc(lc0X1Points);
            }

            var lc0X2RawPoints = attractor.Where(p => Math.Abs(p.X1 - x1) < eps).ToList();
            if (lc0X2RawPoints.Count != 0)
            {
                var (minX2, maxX2) = (lc0X2RawPoints.Min(p => p.X2), lc0X2RawPoints.Max(p => p.X2));
                var stepX2 = (maxX2 - minX2) / lcPointsCount;
                var lc0X2Points = Enumerable.Range(0, lcPointsCount + 1)
                    .Select(i => minX2 + i * stepX2)
                    .Select(x2 => new PointX(x1, x2));

                lc0X2 = new Lc(lc0X2Points, false);
            }

            return FromZeroLc(model, lc0X1, lc0X2, count);
        }

        public static LcList FromZeroLc(DeterministicModel model, Lc lc0X1, Lc lc0X2, int count)
        {
            return new LcList(IterateLcs(model, lc0X1, count).Concat(IterateLcs(model, lc0X2, count)));
        }

        public static IEnumerable<Lc> IterateLcs(DeterministicModel model, Lc lc0, int count)
        {
            if (lc0 == null)
                yield break;

            var currentLc = lc0;
            for (var i = 0; i < count; i++)
            {
                yield return currentLc;
                currentLc = currentLc.GetNextLc(model);
            }
        }

		private static void AddHalfBorderSegments(ICollection<Segment> borderSegments, IList<Segment> allSegments,
            IEnumerable<IntersectSearchResult> halfBorderSegmentResults)
        {
            var halfBorderSegmentResultSet = new HashSet<IntersectSearchResult>(halfBorderSegmentResults);

            while (halfBorderSegmentResultSet.Count != 0)
            {
                var halfBorderSegmentResult = halfBorderSegmentResultSet.First();

                halfBorderSegmentResultSet.Remove(halfBorderSegmentResult);

                var halfBorderSegmentIntersections = halfBorderSegmentResultSet
                    .Select(hbs => (HalfBorderSegmentResult: hbs,
                        IntersectionPoint: hbs.Segment.GetIntersection(halfBorderSegmentResult.Segment)))
                    .Where(t => t.IntersectionPoint.IsIntersect
						&& t.IntersectionPoint.Point.HasValue
						&& IsBorderPoint(t.IntersectionPoint.Point.Value, allSegments))
					.ToList();

                var start = halfBorderSegmentResult.FoundOnStart
                    ? halfBorderSegmentResult.Segment.End
                    : halfBorderSegmentResult.Segment.Start;

				foreach (var halfBorderSegmentIntersection in halfBorderSegmentIntersections)
				{
					var end = halfBorderSegmentIntersection.HalfBorderSegmentResult.FoundOnEnd
						? halfBorderSegmentIntersection.HalfBorderSegmentResult.Segment.Start
						: halfBorderSegmentIntersection.HalfBorderSegmentResult.Segment.End;

					borderSegments.Add(new Segment(start, halfBorderSegmentIntersection.IntersectionPoint.Point.Value));
					borderSegments.Add(new Segment(halfBorderSegmentIntersection.IntersectionPoint.Point.Value, end));
				}
            }
		}

        private static void CloseBorder(ICollection<Segment> borderSegments)
        {
            var borderPointsUsingCount = new Dictionary<PointX, int>();

            foreach (var borderSegment in borderSegments)
            foreach (var boundaryPoint in borderSegment.GetBoundaryPoints())
            {
                if (!borderPointsUsingCount.ContainsKey(boundaryPoint))
                    borderPointsUsingCount[boundaryPoint] = 1;
                else
                    borderPointsUsingCount[boundaryPoint]++;
            }

            var aloneBorderPoints = borderPointsUsingCount.Keys
                .Where(p => borderPointsUsingCount[p] == 1)
                .ToHashSet();

            while (aloneBorderPoints.Count > 1)
            {
                var aloneBorderPoint = aloneBorderPoints.First();
                aloneBorderPoints.Remove(aloneBorderPoint);

                var minLength = double.MaxValue;
                var minLengthPoint = default(PointX);

                foreach (var other in aloneBorderPoints)
                {
                    var length = aloneBorderPoint.GetDistanceWith(other);
                    if (length < minLength)
                    {
                        minLength = length;
                        minLengthPoint = other;
                    }
                }

                borderSegments.Add(new Segment(aloneBorderPoint, minLengthPoint));

                aloneBorderPoints.Remove(minLengthPoint);
            }
        }

        public struct IntersectSearchResult
        {
            public bool FoundOnStart { get; }
            public bool FoundOnEnd { get; }
            public Segment Segment { get; }

            public bool NotFound => !FoundOnStart && !FoundOnEnd;
            public bool HalfIntersect => FoundOnStart != FoundOnEnd;

            public IntersectSearchResult(bool foundOnStart, bool foundOnEnd, Segment segment)
            {
                FoundOnStart = foundOnStart;
                FoundOnEnd = foundOnEnd;
                Segment = segment;
            }
        }
    }
}
