using System.Collections.Generic;

namespace DS
{
    public static class Attractor
    {
        public static bool Is3Cycle(IList<PointX> points)
        {
            return points.Count > 3 && points[0].AlmostEquals(points[3]) && !points[0].AlmostEquals(points[1]);
        }
    }
}
