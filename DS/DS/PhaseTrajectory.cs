using System.Collections.Generic;

namespace DS
{
    public static class PhaseTrajectory
    {
        public static List<PointX> Get(Model model, PointX start, int skipCount, int getCount)
        {
            var current = start;
            var points = new List<PointX>();

            for (var i = 0; i < skipCount + getCount; i++)
            {
                var next = model.GetNextPoint(current);

                if (next.TendsToInfinity(model))
                    return new List<PointX> { PointX.Infinity };

                if (i >= skipCount)
                    points.Add(next);

                current = next;
            }

            return points;
        }

        public static List<PointX> GetWhile(Model model, PointX start, int skipCount, double eps, int k = 1)
        {
            var points = new List<PointX>();
            var current = start;

            for (var i = 0; i < skipCount; i++)
                current = model.GetNextPoint(current);

            start = model.GetNextPoint(current);
            points.Add(start);
            current = start;

            do
            {
                for (var i = 0; i < k; i++)
                    current = model.GetNextPoint(current);

                if (current.TendsToInfinity(model))
                    return new List<PointX> { PointX.Infinity };

                points.Add(current);
            } while (!current.AlmostEquals(start, eps));

            return points;
        }
    }
}
