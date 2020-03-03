using System.Globalization;

namespace DS
{
    public static class DoubleExtensions
    {
        public static string Format(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
