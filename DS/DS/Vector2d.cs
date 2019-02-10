using System;

namespace DS
{
	public struct Vector2D
	{
		public double X { get; set; }
		public double Y { get; set; }

		public Vector2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public Vector2D Rotate(double angle)
		{
			return new Vector2D(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));
		}
	}
}
