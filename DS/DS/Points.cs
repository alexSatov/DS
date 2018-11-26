using System;

namespace DS
{
	public struct PointX
	{
		public double X1 { get; set; }
		public double X2 { get; set; }

		public static PointX Infinity => new PointX(double.MaxValue, double.MaxValue);

		public PointX(double x1, double x2)
		{
			X1 = x1;
			X2 = x2;
		}

		public bool AlmostEquals(PointX other, double eps = 0.00001)
		{
			return Math.Abs(X1 - other.X1) < eps && Math.Abs(X2 - other.X2) < eps;
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
			return $"({X1}, {X2})";
		}

		public void Deconstruct(out double x1, out double x2)
		{
			x1 = X1;
			x2 = X2;
		}
	}

	public struct PointD
	{
		public double D12 { get; set; }
		public double D21 { get; set; }

		public PointD(double d12, double d21)
		{
			D12 = d12;
			D21 = d21;
		}

		public override string ToString()
		{
			return $"({D12}, {D21})";
		}

		public void Deconstruct(out double d12, out double d21)
		{
			d12 = D12;
			d21 = D21;
		}
	}
}
