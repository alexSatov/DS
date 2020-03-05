using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DS.Helpers;
using DS.MathStructures;

namespace DS
{
    public class LcList : List<Lc>
    {
        public LcList(IEnumerable<Lc> lcs) : base(lcs)
        {
        }

        public List<Segment> GetBorderSegments()
        {
            var allPoints = this.SelectMany(lc => lc.Points).ToList();
            var allSegments = this.SelectMany(lc => lc.Segments).ToList();
            var maxX = allPoints.Max(p => p.X) + 10;
            var minX = allPoints.Min(p => p.X) - 10;
            var maxY = allPoints.Max(p => p.Y) + 10;
            var minY = allPoints.Min(p => p.Y) - 10;
            var halfBorderSegmentResults = new ConcurrentQueue<IntersectSearchResult>();
            var borderSegments = allSegments
                .AsParallel()
                .Where(s =>
                {
                    var leftStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(minX, s.Start.Y));
                    var leftEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(minX, s.End.Y));
                    var leftIntersect = IntersectSearch(s, leftStartSegment, leftEndSegment);

                    if (CheckEmptyIntersect(leftIntersect))
                        return true;

                    var upStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(s.Start.X, maxY));
                    var upEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(s.End.X, maxY));
                    var upIntersect = IntersectSearch(s, upStartSegment, upEndSegment);

                    if (CheckEmptyIntersect(upIntersect))
                        return true;

                    var rightStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(maxX, s.Start.Y));
                    var rightEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(maxX, s.End.Y));
                    var rightIntersect = IntersectSearch(s, rightStartSegment, rightEndSegment);

                    if (CheckEmptyIntersect(rightIntersect))
                        return true;

                    var downStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(s.Start.X, minY));
                    var downEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(s.End.X, minY));
                    var downIntersect = IntersectSearch(s, downStartSegment, downEndSegment);

                    return CheckEmptyIntersect(downIntersect);
                })
                .ToList();

            bool CheckEmptyIntersect(IntersectSearchResult intersectSearchResult)
            {
                if (intersectSearchResult.NotFound)
                    return true;

                if (intersectSearchResult.HalfIntersect)
                    halfBorderSegmentResults.Enqueue(intersectSearchResult);

                return false;
            }

            IntersectSearchResult IntersectSearch(Segment segment, Segment startSegment, Segment endSegment)
            {
                var foundOnStart = allSegments.Any(s => s.GetIntersection(startSegment).IsIntersect);
                var foundOnEnd = allSegments.Any(s => s.GetIntersection(endSegment).IsIntersect);

                if (foundOnStart || foundOnEnd)
                    return new IntersectSearchResult(foundOnStart, foundOnEnd, segment);

                return new IntersectSearchResult(false, false, segment);
            }

            var halfBorderSegment = new HashSet<IntersectSearchResult>(halfBorderSegmentResults);

            foreach (var halfBorderSegmentResult in halfBorderSegment)
            {
                var borderIntersection = borderSegments
                    .Select(bs => bs.GetIntersection(halfBorderSegmentResult.Segment))
                    .FirstOrDefault(ip => ip.IsIntersect);

                if (!borderIntersection.Point.HasValue)
                {
                    Console.WriteLine($"Border intersection hasn't point: \r\n{halfBorderSegmentResult.ToJson()}");
                    continue;
                }

                var start = halfBorderSegmentResult.FoundOnStart
                    ? halfBorderSegmentResult.Segment.End
                    : halfBorderSegmentResult.Segment.Start;

                borderSegments.Add(new Segment(start, borderIntersection.Point.Value));
            }

            return borderSegments;
        }

        public static LcList FromAttractor(DeterministicModel model, IEnumerable<PointX> attractor, int x2,
            int count, int lcPointsCount = 100)
        {
            var lc0RawPoints = attractor.Where(p => (int) Math.Round(p.X2) == x2).ToList();
            var (minX1, maxX1) = (lc0RawPoints.Min(p => p.X1), lc0RawPoints.Max(p => p.X1));
            var step = (maxX1 - minX1) / lcPointsCount;
            var lc0Points = Enumerable.Range(0, lcPointsCount + 1)
                .Select(i => minX1 + i * step)
                .Select(x1 => new PointX(x1, Lc.GetX2(model, x1)))
                .ToList();

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

        private struct IntersectSearchResult
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
