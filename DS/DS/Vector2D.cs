using System;

namespace DS
{
	public struct Vector2D
	{
		public double X { get; set; }
		public double Y { get; set; }

		public double Length => Math.Sqrt(X * X + Y * Y);

		public Vector2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public Vector2D(IPoint start, IPoint end)
		{
			X = end.X - start.X;
			Y = end.Y - start.Y;
		}

		public Vector2D Rotate(double angle)
		{
			return new Vector2D(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));
		}

		public static Vector2D operator +(Vector2D left, Vector2D right)
		{
			return new Vector2D(left.X + right.X, left.Y + right.Y);
		}

		public static Vector2D operator -(Vector2D left, Vector2D right)
		{
			return new Vector2D(left.X - right.X, left.Y - right.Y);
		}

		public static double operator *(Vector2D left, Vector2D right)
		{
			return left.X * right.X + left.Y * right.Y;
		}

		public static Vector2D operator *(Vector2D vector2D, double scalar)
		{
			return new Vector2D(vector2D.X * scalar, vector2D.Y * scalar);
		}

		public static Vector2D operator *(double scalar, Vector2D vector2D)
		{
			return vector2D * scalar;
		}

		public static Vector2D operator /(Vector2D vector2D, double scalar)
		{
			return new Vector2D(vector2D.X / scalar, vector2D.Y / scalar);
		}

		public static Vector2D operator /(double scalar, Vector2D vector2D)
		{
			return vector2D / scalar;
		}
	}
}
