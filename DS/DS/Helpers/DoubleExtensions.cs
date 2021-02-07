using System;
using System.Globalization;
using System.Linq;

namespace DS.Helpers
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
    }
}
