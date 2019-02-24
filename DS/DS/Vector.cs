using System;

namespace DS
{
	public struct Vector
	{
		public double X { get; set; }
		public double Y { get; set; }

		public double Length => Math.Sqrt(X * X + Y * Y);

		public Vector(double x, double y)
		{
			X = x;
			Y = y;
		}

		public Vector(IPoint start, IPoint end)
		{
			X = end.X - start.X;
			Y = end.Y - start.Y;
		}

		public Vector Rotate(double angle)
		{
			return new Vector(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));
		}

		public static Vector operator +(Vector left, Vector right)
		{
			return new Vector(left.X + right.X, left.Y + right.Y);
		}

		public static Vector operator -(Vector left, Vector right)
		{
			return new Vector(left.X - right.X, left.Y - right.Y);
		}

		public static double operator *(Vector left, Vector right)
		{
			return left.X * right.X + left.Y * right.Y;
		}

		public static Vector operator *(Vector vector, double scalar)
		{
			return new Vector(vector.X * scalar, vector.Y * scalar);
		}

		public static Vector operator *(double scalar, Vector vector)
		{
			return vector * scalar;
		}

		public static Vector operator /(Vector vector, double scalar)
		{
			return new Vector(vector.X / scalar, vector.Y / scalar);
		}

		public static Vector operator /(double scalar, Vector vector)
		{
			return vector / scalar;
		}
	}
}
