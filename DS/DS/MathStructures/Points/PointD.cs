using System.Globalization;

namespace DS.MathStructures.Points
{
    public readonly struct PointD : IPoint
    {
        public double Dx { get; }
        public double Dy { get; }

        public double X => Dx;
        public double Y => Dy;

        public PointD(double dx, double dy)
        {
            Dx = dx;
            Dy = dy;
        }

        public void Deconstruct(out double dx, out double dy)
        {
            dx = Dx;
            dy = Dy;
        }

        public override string ToString()
        {
            return $"({Dx.ToString(CultureInfo.InvariantCulture)}, {Dy.ToString(CultureInfo.InvariantCulture)})";
        }
    }
}
