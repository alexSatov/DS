using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DS.Extensions;
using DS.MathStructures;
using DS.MathStructures.Points;
using DS.Models;

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

        public IEnumerable<LcPoint> GetBorderPoints()
        {
            var allSegments = GetAllSegments().ToList();

            foreach (var lcType in Keys)
            {
                for (var lcIndex = 0; lcIndex < this[lcType].Count; lcIndex++)
                {
                    for (var index = 0; index < this[lcType][lcIndex].Count; index++)
                    {
                        var point = this[lcType][lcIndex][index];
                        if (IsOutOrBorderPoint(point, allSegments))
                            yield return new LcPoint(lcType, lcIndex, index, point);
                    }
                }
            }
        }

        /// <summary>
        /// Accurate border segments algorithm
        /// </summary>
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

        /// <summary>
        /// Greedy border segments algorithm (can use accurate on input)
        /// </summary>
        public List<Segment> GetBorderSegments2(bool useAccurateAlgorithm = true, bool closeBorder = false)
        {
            var result = new List<Segment>();
            var allSegments = GetAllSegments().ToList();
            var borderPoints = useAccurateAlgorithm
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
            foreach (var direction in Direction.GetAll())
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
            return Direction.GetAll()
                .Select(direction => point + direction * e)
                .All(deltaPoint => IsOutOrBorderPoint(deltaPoint, allSegments));
        }

        public static LcSet FromAttractor(BaseModel model, IList<PointX> attractor, int count,
            int lcPointsCount = 100, double eps = 0.1, Lc lc0H = null, Lc lc0V = null, bool withoutIntersection = false)
        {
            if (model.LcH == null && model.LcV == null)
                throw new InvalidOperationException("Model must implement at least one LC_-1 function");

            if (model.LcH != null && lc0H == null)
            {
                var lc0HRawPoints = attractor
                    .Where(p => Math.Abs(p.X2 - model.LcH(p.X1)) < eps)
                    .ToList();

                if (lc0HRawPoints.Count != 0)
                {
                    var (minX1, maxX1) = (lc0HRawPoints.Min(p => p.X1), lc0HRawPoints.Max(p => p.X1));
                    lc0H = model.GetLc0H(minX1, maxX1, lcPointsCount);
                }
            }

            if (model.LcV != null && lc0V == null)
            {
                var lc0VRawPoints = attractor
                    .Where(p => Math.Abs(p.X1 - model.LcV(p.X2)) < eps)
                    .ToList();

                if (lc0VRawPoints.Count != 0)
                {
                    var (minX2, maxX2) = (lc0VRawPoints.Min(p => p.X2), lc0VRawPoints.Max(p => p.X2));
                    lc0V = model.GetLc0V(minX2, maxX2, lcPointsCount);
                }
            }

            if (withoutIntersection)
            {
                (lc0H, lc0V) = HandleIntersection(lc0H, lc0V, model);
            }

            return FromZeroLc(model, lc0H, lc0V, count);
        }

        private static (Lc lc0H, Lc lc0V) HandleIntersection(Lc lc0H, Lc lc0V, BaseModel model)
        {
            var lc0HSegment = new Segment(lc0H[0], lc0H[^1]);
            var lc0VSegment = new Segment(lc0V[0], lc0V[^1]);
            var intersection = lc0HSegment.GetIntersection(lc0VSegment);

            if (!intersection.IsIntersect || !intersection.Point.HasValue)
            {
                return (lc0H, lc0V);
            }

            var lc0HLeftDelta = Math.Abs(intersection.Point.Value.X - lc0HSegment.Start.X);
            var lc0HRightDelta = Math.Abs(intersection.Point.Value.X - lc0HSegment.End.X);
            var lc0VBottomDelta = Math.Abs(intersection.Point.Value.Y - lc0VSegment.Start.Y);
            var lc0VTopDelta = Math.Abs(intersection.Point.Value.Y - lc0VSegment.End.Y);

            return (lc0HLeftDelta > lc0HRightDelta
                    ? model.GetLc0H(lc0HSegment.Start.X, intersection.Point.Value.X, lc0H.Count)
                    : model.GetLc0H(intersection.Point.Value.X, lc0HSegment.End.X, lc0H.Count),
                lc0VBottomDelta > lc0VTopDelta
                    ? model.GetLc0V(lc0VSegment.Start.Y, intersection.Point.Value.Y, lc0V.Count)
                    : model.GetLc0V(intersection.Point.Value.Y, lc0VSegment.End.Y, lc0V.Count));
        }

        private static LcSet FromZeroLc(BaseModel baseModel, Lc lc0H, Lc lc0V, int count)
        {
            return new LcSet(IterateLcs(baseModel, lc0H, count), IterateLcs(baseModel, lc0V, count));
        }

        private static IEnumerable<Lc> IterateLcs(BaseModel baseModel, Lc lc0, int count)
        {
            if (lc0 == null)
                yield break;

            var currentLc = lc0;

            yield return currentLc;

            for (var i = 0; i < count; i++)
            {
                currentLc = currentLc.GetNextLc(baseModel);
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

                if (halfBorderSegmentIntersections.Count > 1)
                {
                    Console.WriteLine($"Warning: multiple halfBorderSegmentIntersections!\r\n" +
                        $"segment: ${halfBorderSegmentResult.ToJson()}\r\n" +
                        $"intersections: ${halfBorderSegmentIntersections.ToJson()}\r\n");
                }

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

                    halfBorderSegmentResultSet.Remove(halfBorderSegmentIntersection.HalfBorderSegmentResult);
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
            var startPoint = currentPoint;

            while (pointSet.Count > 1)
            {
                pointSet.Remove(currentPoint);

                var segment = pointSet
                    .Select(p => (Point: p, Distance: currentPoint.GetDistanceWith(p)))
                    .OrderBy(t => t.Distance)
                    .Select(t => new Segment(currentPoint, t.Point))
                    .FirstOrDefault(s => IsOutOrBorderPoint(s.Center, allSegments));

                if (segment.Equals(default(Segment)))
                {
                    Console.WriteLine($"Error: point {currentPoint} haven't neighbor!");

                    currentPoint = pointSet.First();

                    continue;
                }

                yield return segment;

                currentPoint = segment.End;
            }

            var lastSegment = new Segment(currentPoint, startPoint);
            if (IsOutOrBorderPoint(lastSegment.Center, allSegments))
            {
                yield return lastSegment;
            }
        }

        private readonly struct IntersectSearchResult
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
