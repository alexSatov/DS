using System;
using System.Linq;
using DS.Extensions;

namespace DS
{
    public record Attractor<TPoint, TParams>(AttractorType Type, TPoint[] Points, TParams Params);

    public static class Attractor
    {
        public static int MaxCyclePeriod { get; }

        static Attractor()
        {
            MaxCyclePeriod = Enum.GetValues<AttractorType>().Cast<int>().Max();
        }

        public static Attractor<double[], T> From<T>(double[][] points, T @params, double eps)
        {
            if (points.Length == 0)
                throw new ArgumentException("Empty point list", nameof(points));

            if (points[^1].IsInfinity())
                return new Attractor<double[], T>(AttractorType.Infinity, points[^1..], @params);

            if (points.Length == 1 || points[^1].AlmostEquals(points[^2], eps))
                return new Attractor<double[], T>(AttractorType.Equilibrium, points[^1..], @params);

            var period = 1;

            while (++period <= MaxCyclePeriod && points.Length > period)
            {
                if (points[^1].AlmostEquals(points[^(period + 1)], eps))
                    return new Attractor<double[], T>((AttractorType) period, points[^period..], @params);
            }

            return new Attractor<double[], T>(AttractorType.Chaos, points, @params);
        }
    }
}
