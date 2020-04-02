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
                        if (IsBorderPoint(point, allSegments))
                            yield return (lcType, lcIndex, index, point);
                    }
                }
            }
        }

        public List<Segment> GetBorderSegments(bool handleHalfBorders = true, bool closeBorder = true)
        {
            var allSegments = GetAllSegments().ToList();
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
                CloseBorder(borderSegments, allSegments);

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

        private static void CloseBorder(ICollection<Segment> borderSegments, ICollection<Segment> allSegments)
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
                .Where(p => borderPointsUsingCount[p].Count == 1)
                .ToHashSet();

            while (aloneBorderPoints.Count > 1)
            {
                var aloneBorderPoint = aloneBorderPoints.First();
                aloneBorderPoints.Remove(aloneBorderPoint);

                var minLength = double.MaxValue;
                var minLengthPoint = default(PointX);

                foreach (var other in aloneBorderPoints.Except(new []{ borderPointsUsingCount[aloneBorderPoint].Neighbor }))
                {
                    var length = aloneBorderPoint.GetDistanceWith(other);
                    if (length < minLength)
                    {
                        minLength = length;
                        minLengthPoint = other;
                    }
                }

                var segment = new Segment(aloneBorderPoint, minLengthPoint);

                if (!allSegments.Any(s => s.GetIntersection(segment).IsIntersect))
                    borderSegments.Add(segment);

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

        //public List<Segment> GetBorderSegments2(bool closeBorder = true)
        //{
        //    var borderSegments = new List<Segment>();
        //    var borderPoints = GetBorderPoints()
        //        .GroupBy(t => t.Type)
        //        .ToDictionary(g => g.Key, g => g
        //            .GroupBy(t => t.LcIndex)
        //            .ToDictionary(g => g.Key, g => g.Select(t => (t.Index, t.Point)).ToList()));

        //    foreach (var lcType in Enum.GetValues(typeof(LcType)).Cast<LcType>())
        //    {
        //        if (!borderPoints.ContainsKey(lcType))
        //            continue;

        //        for (var lcIndex = 0; lcIndex < borderPoints[lcType].Count; lcIndex++)
        //        {
        //            for (var i = 0; i < borderPoints[lcType][lcIndex].Count - 1; i++)
        //            {
        //                var start = borderPoints[lcType][lcIndex][i];
        //                var end = borderPoints[lcType][lcIndex][i + 1];

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
