using System.Globalization;

namespace DS.MathStructures.Points
{
    public readonly struct PointD : IPoint
    {
        public double D12 { get; }
        public double D21 { get; }

        public double X => D12;
        public double Y => D21;

        public PointD(double d12, double d21)
        {
            D12 = d12;
            D21 = d21;
        }

        public void Deconstruct(out double d12, out double d21)
        {
            d12 = D12;
            d21 = D21;
        }

        public override string ToString()
        {
            return $"({D12.ToString(CultureInfo.InvariantCulture)}, {D21.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}
