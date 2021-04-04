using System;
using System.Globalization;
using System.Linq;

namespace DS.Extensions
{
    public static class DoubleExtensions
    {
        public static string Format(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static bool TendsToValue(this double[] vector, double value)
        {
            return vector.Any(x => Math.Abs(x) >= value);
        }

        public static bool IsInfinity(this double[] vector)
        {
            return vector.All(x => x == double.MaxValue);
        }

        public static bool AlmostEquals(this double[] vector, double[] other, double eps)
        {
            if (vector.Length != other.Length)
                return false;

            return !vector.Where((t, i) => Math.Abs(t - other[i]) > eps).Any();
        }
    }
}
