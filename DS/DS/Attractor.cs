using System.Collections.Generic;

namespace DS
{
    public static class Attractor
    {
        public static bool Is3Cycle(IList<PointX> points)
        {
            return points.Count > 3 && points[0].AlmostEquals(points[3]) && !points[0].AlmostEquals(points[1]);
        }

        public static bool IsEquilibrium(IList<PointX> points)
        {
            return points.Count > 1 && points[points.Count - 1].AlmostEquals(points[points.Count - 2]);
        }
    }
}
