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
            var segments = this.SelectMany(lc => lc.Segments).ToList();
            return segments;
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
