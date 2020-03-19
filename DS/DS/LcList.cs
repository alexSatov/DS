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
                if (index > 0)
                    return (true, lcIndex, index);
            }

            return (false, -1, -1);
        }

        public List<Segment> GetBorderSegments(bool handleHalfBorders = true)
        {
            var allSegments = this.SelectMany(lc => lc.Segments).ToList();
            var halfBorderSegmentResults = new ConcurrentQueue<IntersectSearchResult>();
            var borderSegments = allSegments
                .AsParallel()
                .Where(s => IsBorderSegment(s, allSegments, CheckEmptyIntersect))
                .ToList();

            bool CheckEmptyIntersect(IntersectSearchResult intersectSearchResult)
            {
                if (intersectSearchResult.NotFound)
                    return true;

                if (intersectSearchResult.HalfIntersect)
                    halfBorderSegmentResults.Enqueue(intersectSearchResult);

                return false;
            }

            if (handleHalfBorders)
                AddHalfBorderSegments(borderSegments, halfBorderSegmentResults);

            return borderSegments;
        }

        public bool IsBorderSegment(Segment s, IList<Segment> allSegments,
            Func<IntersectSearchResult, bool> checkEmptyIntersect = null)
        {
            const double max = double.MaxValue / 2;
            const double min = double.MinValue / 2;

            if (checkEmptyIntersect == null)
                checkEmptyIntersect = r => r.NotFound;

            return (from direction in Directions.All
                    let startX = Directions.ChooseHorizontally(direction, min, max, s.Start.X)
                    let endX = Directions.ChooseHorizontally(direction, min, max, s.End.X)
                    let startY = Directions.ChooseVertically(direction, min, max, s.Start.Y)
                    let endY = Directions.ChooseVertically(direction, min, max, s.End.Y)
                    let startSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(startX, startY))
                    let endSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(endX, endY))
                    select IntersectSearch(s, startSegment, endSegment, allSegments))
                .Any(checkEmptyIntersect);
        }

        public static LcList FromAttractor(DeterministicModel model, IEnumerable<PointX> attractor, int x2,
            int count, int lcPointsCount = 100, double eps = 0.1)
        {
            var lc0RawPoints = attractor.Where(p => Math.Abs(p.X2 - x2) < eps).ToList();
            var (minX1, maxX1) = (lc0RawPoints.Min(p => p.X1), lc0RawPoints.Max(p => p.X1));
            var step = (maxX1 - minX1) / lcPointsCount;
            var lc0Points = Enumerable.Range(0, lcPointsCount + 1)
                .Select(i => minX1 + i * step)
                .Select(x1 => new PointX(x1, Lc.GetX2(model, x1)));

            var lc0 = new Lc(lc0Points);

            return FromZeroLc(model, lc0, count);
        }

        public static LcList FromZeroLc(DeterministicModel model, Lc lc0, int count)
        {
            return new LcList(IterateLcs(model, lc0, count));
        }

        public static IEnumerable<Lc> IterateLcs(DeterministicModel model, Lc lc0, int count)
        {
            var currentLc = lc0;
            for (var i = 0; i < count; i++)
            {
                yield return currentLc;
                currentLc = currentLc.GetNextLc(model);
            }
        }

        private IntersectSearchResult IntersectSearch(Segment segment, Segment startSegment, Segment endSegment,
            IList<Segment> allSegments)
        {
            var foundOnStart = allSegments.Any(s => s.GetIntersection(startSegment).IsIntersect);
            var foundOnEnd = allSegments.Any(s => s.GetIntersection(endSegment).IsIntersect);

            if (foundOnStart || foundOnEnd)
                return new IntersectSearchResult(foundOnStart, foundOnEnd, segment);

            return new IntersectSearchResult(false, false, segment);
        }

        private static void AddHalfBorderSegments(ICollection<Segment> borderSegments,
            IEnumerable<IntersectSearchResult> halfBorderSegmentResults)
        {
            var halfBorderSegmentResultSet = new HashSet<IntersectSearchResult>(halfBorderSegmentResults);

            while (halfBorderSegmentResultSet.Count != 0)
            {
                var halfBorderSegmentResult = halfBorderSegmentResultSet.First();

                halfBorderSegmentResultSet.Remove(halfBorderSegmentResult);

                var halfBorderSegmentIntersection = halfBorderSegmentResultSet
                    .Where(hbs => !hbs.Equals(halfBorderSegmentResult))
                    .Select(hbs => (HalfBorderSegmentResult: hbs,
                        IntersectionPoint: hbs.Segment.GetIntersection(halfBorderSegmentResult.Segment)))
                    .FirstOrDefault(t => t.IntersectionPoint.IsIntersect);

                var start = halfBorderSegmentResult.FoundOnStart
                    ? halfBorderSegmentResult.Segment.End
                    : halfBorderSegmentResult.Segment.Start;

                if (!halfBorderSegmentIntersection.IntersectionPoint.Point.HasValue)
                    continue;

                var end = halfBorderSegmentIntersection.HalfBorderSegmentResult.FoundOnEnd
                    ? halfBorderSegmentIntersection.HalfBorderSegmentResult.Segment.Start
                    : halfBorderSegmentIntersection.HalfBorderSegmentResult.Segment.End;

                borderSegments.Add(new Segment(start, halfBorderSegmentIntersection.IntersectionPoint.Point.Value));
                borderSegments.Add(new Segment(halfBorderSegmentIntersection.IntersectionPoint.Point.Value, end));

                halfBorderSegmentResultSet.Remove(halfBorderSegmentIntersection.HalfBorderSegmentResult);
            }

            CloseBorder(borderSegments);
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

            while (aloneBorderPoints.Count != 0)
            {
                var aloneBorderPoint = aloneBorderPoints.First();
                aloneBorderPoints.Remove(aloneBorderPoint);

                var minLength = double.MaxValue;
                var minLengthPoint = default(PointX);

                foreach (var other in aloneBorderPoints.Where(p => !p.Equals(aloneBorderPoint)))
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
