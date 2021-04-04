using System;
using System.Collections.Generic;
using System.Linq;
using DS.Extensions;

namespace DS
{
    public class Attractor<TPoint, TParams>
    {
        public AttractorType Type { get; }
        public IList<TPoint> Points { get; }
        public TParams Params { get; }

        public Attractor(AttractorType type, IList<TPoint> points, TParams @params)
        {
            Type = type;
            Points = points;
            Params = @params;
        }
    }

    public static class Attractor
    {
        public static int MaxCyclePeriod { get; }

        static Attractor()
        {
            MaxCyclePeriod = Enum.GetValues<AttractorType>().Cast<int>().Max();
        }

        public static Attractor<double[], T> From<T>(double[][] points, T @params, double eps = 0.00001)
        {
            if (points.Length == 0)
                throw new ArgumentException("Empty point list", nameof(points));

            if (points[^1].IsInfinity())
                return new Attractor<double[], T>(AttractorType.Infinity, points, @params);

            if (points.Length == 1 || points[^1].AlmostEquals(points[^2], eps))
                return new Attractor<double[], T>(AttractorType.Equilibrium, new[] { points[^1] }, @params);

            var period = 2;

            while (period <= MaxCyclePeriod && points.Length > period)
            {
                if (points[^1].AlmostEquals(points[^(period + 1)], eps))
                    return new Attractor<double[], T>((AttractorType) period, points[^period..], @params);
            }

            return new Attractor<double[], T>(AttractorType.Chaos, points, @params);
        }
    }
}
