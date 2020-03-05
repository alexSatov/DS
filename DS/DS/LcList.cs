using System;
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

        public IEnumerable<Segment> GetBorderSegments()
        {
            var allPoints = this.SelectMany(lc => lc.Points).ToList();
            var allSegments = this.SelectMany(lc => lc.Segments).ToList();
            var maxX = allPoints.Max(p => p.X) + 10;
            var minX = allPoints.Min(p => p.X) - 10;
            var maxY = allPoints.Max(p => p.Y) + 10;
            var minY = allPoints.Min(p => p.Y) - 10;

            return allSegments
                .AsParallel()
                .Where(s =>
                {
                    var leftStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(minX, s.Start.Y));
                    var leftEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(minX, s.End.Y));

                    if (allSegments.TrueForAll(os => !leftStartSegment.Intersect(os) && !leftEndSegment.Intersect(os)))
                        return true;

                    var upStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(s.Start.X, maxY));
                    var upEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(s.End.X, maxY));

                    if (allSegments.TrueForAll(os => !upStartSegment.Intersect(os) && !upEndSegment.Intersect(os)))
                        return true;

                    var rightStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(maxX, s.Start.Y));
                    var rightEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(maxX, s.End.Y));

                    if (allSegments.TrueForAll(os => !rightStartSegment.Intersect(os) && !rightEndSegment.Intersect(os)))
                        return true;

                    var downStartSegment = new Segment(new PointX(s.Start.X, s.Start.Y), new PointX(s.Start.X, minY));
                    var downEndSegment = new Segment(new PointX(s.End.X, s.End.Y), new PointX(s.End.X, minY));

                    if (allSegments.TrueForAll(os => !downStartSegment.Intersect(os) && !downEndSegment.Intersect(os)))
                        return true;

                    return false;
                });
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
    }
}
