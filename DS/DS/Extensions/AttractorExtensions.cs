using System.Collections.Generic;
using DS.MathStructures.Points;

namespace DS.Extensions
{
    public static class AttractorExtensions
    {
        public static bool Is3Cycle(IList<PointX> points)
        {
            return points.Count > 3 && points[0].AlmostEquals(points[3]) && !points[0].AlmostEquals(points[1]);
        }

        public static bool IsEquilibrium(IList<PointX> points)
        {
            return points.Count > 1 && points[0].AlmostEquals(points[1]);
        }

        public static string Format(this IEnumerable<PointX> points)
        {
            return string.Join(", ", points);
        }
    }
}
