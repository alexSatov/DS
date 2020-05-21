using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;

namespace DS
{
    public class LcSet : Dictionary<LcType, List<Lc>>
    {
        public LcSet(IEnumerable<Lc> lcH, IEnumerable<Lc> lcV)
        {
            this[LcType.H] = lcH.ToList();
            this[LcType.V] = lcV.ToList();
        }

        public IEnumerable<Segment> GetAllSegments()
        {
            return this[LcType.H].SelectMany(lc => lc.Segments)
                .Concat(this[LcType.V].SelectMany(lc => lc.Segments));
        }

        public IEnumerable<(LcType Type, int LcIndex, int Index, PointX Point)> GetBorderPoints()
        {
            var allSegments = GetAllSegments().ToList();

            foreach (var lcType in Enum.GetValues(typeof(LcType)).Cast<LcType>())
            {
                for (var lcIndex = 0; lcIndex < this[lcType].Count; lcIndex++)
                {
                    for (var index = 0; index < this[lcType][lcIndex].Count; index++)
                    {
                        var point = this[lcType][lcIndex][index];
                        if (IsOutOrBorderPoint(point, allSegments))
                            yield return (lcType, lcIndex, index, point);
                    }
                }
            }
        }

        public List<Segment> GetBorderSegments(bool handleHalfBorders = true, bool closeBorder = false)
        {
            var allSegments = GetAllSegments().ToList();
            var halfBorderSegmentResults = new ConcurrentQueue<IntersectSearchResult>();
            var borderSegments = allSegments
                .AsParallel()
                .Where(s =>
                {
                    var notFoundOnStart = IsOutOrBorderPoint(s.Start, allSegments);
                    var notFoundOnEnd = IsOutOrBorderPoint(s.End, allSegments);
                    var result = new IntersectSearchResult(!notFoundOnStart, !notFoundOnEnd, s);

                    if (result.HalfIntersect)
                        halfBorderSegmentResults.Enqueue(result);

                    return result.NotFound;
                })
                .ToList();

            if (handleHalfBorders)
                AddHalfBorderSegments(borderSegments, allSegments, halfBorderSegmentResults);

            if (closeBorder)
                CloseBorder(borderSegments, allSegments);

            return borderSegments;
        }

        public List<Segment> GetBorderSegments2(bool useFirstMethod = true, bool closeBorder = true)
        {
            var result = new List<Segment>();
            var allSegments = GetAllSegments().ToList();
            var borderPoints = useFirstMethod
                ? GetBorderSegments().SelectMany(bs => bs.GetBoundaryPoints())
                : GetBorderPoints().Select(t => t.Point);

            var segments = TryConnectPoints(borderPoints, allSegments);
            result.AddRange(segments);

            if (closeBorder)
                CloseBorder(result, allSegments);

            return result;
        }

        public static bool IsOutOrBorderPoint(PointX point, IList<Segment> allSegments, double e = 1000)
        {
            foreach (var direction in Directions.All)
            {
                var segment = new Segment(point, point + direction * e);

                if (!allSegments
                    .Where(s => !s.Contains(point))
                    .Any(s => s.GetIntersection(segment).IsIntersect))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsOutPoint(PointX point, IList<Segment> allSegments, double e = 0.001)
        {
            return Directions.All
                .Select(direction => point + direction * e)
                .All(deltaPoint => IsOutOrBorderPoint(deltaPoint, allSegments));
        }

        public static LcSet FromAttractor(DeterministicModel model, IList<PointX> attractor, int count,
            int x1 = 20, int x2 = 40, int lcPointsCount = 100, double eps = 0.1)
        {
            Lc lc0H = null, lc0V = null;

            var lc0HRawPoints = attractor.Where(p => Math.Abs(p.X2 - x2) < eps).ToList();
            if (lc0HRawPoints.Count != 0)
            {
                var (minX1, maxX1) = (lc0HRawPoints.Min(p => p.X1), lc0HRawPoints.Max(p => p.X1));
                var stepX1 = (maxX1 - minX1) / lcPointsCount;
                var lc0HPoints = Enumerable.Range(0, lcPointsCount + 1)
                    .Select(i => minX1 + i * stepX1)
                    .Select(x1 => new PointX(x1, x2));

                lc0H = new Lc(lc0HPoints);
            }

            var lc0VRawPoints = attractor.Where(p => Math.Abs(p.X1 - x1) < eps).ToList();
            if (lc0VRawPoints.Count != 0)
            {
                var (minX2, maxX2) = (lc0VRawPoints.Min(p => p.X2), lc0VRawPoints.Max(p => p.X2));
                var stepX2 = (maxX2 - minX2) / lcPointsCount;
                var lc0VPoints = Enumerable.Range(0, lcPointsCount + 1)
                    .Select(i => minX2 + i * stepX2)
                    .Select(x2 => new PointX(x1, x2));

                lc0V = new Lc(lc0VPoints, LcType.V);
            }

            return FromZeroLc(model, lc0H, lc0V, count);
        }

        public static LcSet FromZeroLc(DeterministicModel model, Lc lc0H, Lc lc0V, int count)
        {
            return new LcSet(IterateLcs(model, lc0H, count), IterateLcs(model, lc0V, count));
        }

        public static IEnumerable<Lc> IterateLcs(DeterministicModel model, Lc lc0, int count)
        {
            if (lc0 == null)
                yield break;

            var currentLc = lc0;

            yield return currentLc;

            for (var i = 0; i < count; i++)
            {
                currentLc = currentLc.GetNextLc(model);
                yield return currentLc;
            }
        }

        private static void AddHalfBorderSegments(IList<Segment> borderSegments, IList<Segment> allSegments,
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
                        && IsOutOrBorderPoint(t.IntersectionPoint.Point.Value, allSegments))
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

        private static void CloseBorder(List<Segment> borderSegments, IList<Segment> allSegments)
        {
            var borderPointsUsingCount = new Dictionary<PointX, (int Count, PointX Neighbor)>();

            foreach (var borderSegment in borderSegments)
            {
                if (!borderPointsUsingCount.ContainsKey(borderSegment.Start))
                    borderPointsUsingCount[borderSegment.Start] = (1, borderSegment.End);
                else
                    borderPointsUsingCount[borderSegment.Start] = 
                        (borderPointsUsingCount[borderSegment.Start].Count + 1,
                            borderPointsUsingCount[borderSegment.Start].Neighbor);

                if (!borderPointsUsingCount.ContainsKey(borderSegment.End))
                    borderPointsUsingCount[borderSegment.End] = (1, borderSegment.Start);
                else
                    borderPointsUsingCount[borderSegment.End] = 
                        (borderPointsUsingCount[borderSegment.End].Count + 1,
                            borderPointsUsingCount[borderSegment.End].Neighbor);
            }

            var aloneBorderPoints = borderPointsUsingCount.Keys
                .Where(p => borderPointsUsingCount[p].Count == 1);

            var segments = TryConnectPoints(aloneBorderPoints, allSegments);
            borderSegments.AddRange(segments);
        }

        private static IEnumerable<Segment> TryConnectPoints(IEnumerable<PointX> points, IList<Segment> allSegments)
        {
            var pointSet = points.ToHashSet();
            var currentPoint = pointSet.First();
            pointSet.Remove(currentPoint);

            while (pointSet.Count > 0)
            {
                var segment = pointSet
                    .Select(p => (Point: p, Distance: currentPoint.GetDistanceWith(p)))
                    .OrderBy(t => t.Distance)
                    .Select(t => new Segment(currentPoint, t.Point))
                    .FirstOrDefault(s => IsOutOrBorderPoint(s.Center, allSegments));

                if (segment.Equals(default(Segment)))
                {
                    Console.WriteLine($"Error: point {currentPoint} haven't neighbor!");

                    currentPoint = pointSet.First();
                    pointSet.Remove(currentPoint);

                    continue;
                }

                yield return segment;

                currentPoint = segment.End;
                pointSet.Remove(currentPoint);
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

        //public List<Segment> GetBorderSegments3(bool closeBorder = true)
        //{
        //    var borderSegments = new List<Segment>();
        //    var pointSet = GetBorderPoints()
        //        .GroupBy(t => t.Type)
        //        .ToDictionary(g => g.Key, g => g
        //            .GroupBy(t => t.LcIndex)
        //            .ToDictionary(g => g.Key, g => g.Select(t => (t.Index, t.Point)).ToList()));

        //    foreach (var lcType in Enum.GetValues(typeof(LcType)).Cast<LcType>())
        //    {
        //        if (!pointSet.ContainsKey(lcType))
        //            continue;

        //        for (var lcIndex = 0; lcIndex < pointSet[lcType].Count; lcIndex++)
        //        {
        //            for (var i = 0; i < pointSet[lcType][lcIndex].Count - 1; i++)
        //            {
        //                var start = pointSet[lcType][lcIndex][i];
        //                var end = pointSet[lcType][lcIndex][i + 1];

        //                if (end.Index - start.Index == 1)
        //                    borderSegments.Add(new Segment(start.Point, end.Point));
        //            }
        //        }
        //    }

        //    if (closeBorder)
        //        CloseBorder(borderSegments);

        //    return borderSegments;
        //}
    }
}
