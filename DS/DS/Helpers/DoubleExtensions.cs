using System.Globalization;

namespace DS.Helpers
{
    public static class DoubleExtensions
    {
        public static string Format(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
