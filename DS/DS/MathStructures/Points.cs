using System;
using System.Globalization;

namespace DS.MathStructures
{
    public struct PointX : IPoint
    {
        public double X1 { get; }
        public double X2 { get; }

        public double X => X1;
        public double Y => X2;

        public static PointX Infinity => new PointX(double.MaxValue, double.MaxValue);

        public PointX(double x1, double x2)
        {
            X1 = x1;
            X2 = x2;
        }

        public bool AlmostEquals(PointX other, double eps = 0.00001)
        {
            return Math.Abs(X1 - other.X1) <= eps && Math.Abs(X2 - other.X2) <= eps;
        }

        public bool IsInfinity()
        {
            return X1 == double.MaxValue && X2 == double.MaxValue;
        }

        public bool TendsToInfinity(Model model)
        {
            return Math.Abs(X1) > model.MaxX || Math.Abs(X2) > model.MaxX;
        }

        public override string ToString()
        {
            return $"({X1.ToString(CultureInfo.InvariantCulture)}, {X2.ToString(CultureInfo.InvariantCulture)})";
        }

        public void Deconstruct(out double x1, out double x2)
        {
            x1 = X1;
            x2 = X2;
        }

        public double GetDistanceWith(PointX other)
        {
            var x = other.X - X;
            var y = other.Y - Y;

            return Math.Sqrt(x * x + y * y);
        }

        public PointX GetCenterPointWith(PointX other)
        {
            var vector = new Vector2D(this, other) / 2;
            return this + vector;
        }

        public static PointX operator +(PointX left, Vector2D right)
        {
            return new PointX(left.X + right.X, left.Y + right.Y);
        }

        public static PointX operator +(Vector2D left, PointX right)
        {
            return new PointX(left.X + right.X, left.Y + right.Y);
        }
    }

    public struct PointD : IPoint
    {
        public double D12 { get; set; }
        public double D21 { get; set; }

        public double X => D12;
        public double Y => D21;

        public PointD(double d12, double d21)
        {
            D12 = d12;
            D21 = d21;
        }

        public override string ToString()
        {
            return $"({D12.ToString(CultureInfo.InvariantCulture)}, {D21.ToString(CultureInfo.InvariantCulture)})";
        }

        public void Deconstruct(out double d12, out double d21)
        {
            d12 = D12;
            d21 = D21;
        }
    }

    public interface IPoint
    {
        double X { get; }
        double Y { get; }
    }
}
