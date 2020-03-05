using System;

namespace DS.MathStructures
{
	public struct Segment
	{
		public IPoint Start { get; }
		public IPoint End { get; }
		public Vector2D Vector { get; }

		public double Length => Vector.Length;

		public Segment(IPoint start, IPoint end)
		{
			Start = start;
			End = end;
			Vector = new Vector2D(start, end);
		}

		/// <summary>
		/// Checks if two segments have the intersection
		/// </summary>
		/// <seealso cref="https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282"/>
		/// <param name="other"></param>
		/// <param name="includeBoundaryPoints">Determines whether to include boundary points case</param>
		/// <param name="includeOverlap">Determines whether to include overlap case</param>
		/// <returns></returns>
		public bool Intersect(Segment other, bool includeBoundaryPoints = false, bool includeOverlap = false)
		{
			if (HasCommonBoundaryPoint(other))
				return includeBoundaryPoints;

			var (p, r, q, s) = (Start, Vector, other.Start, other.Vector);
			var pq = new Vector2D(p, q);
			var rCrossS = r.Cross(s);
			var pqCrossR = pq.Cross(r);
			var rCrossSIsZero = Math.Abs(rCrossS) < 0.0000001;
			var pqCrossRIsZero = Math.Abs(pqCrossR) < 0.0000001;

			if (rCrossSIsZero && pqCrossRIsZero) // lines are collinear
			{
				if (!includeOverlap)
					return false;

				var sr = s.Dot(r);
				var rSqr = r.Dot(r);
				var t0 = pq.Dot(r) / rSqr;
				var t1 = t0 + sr / rSqr;

				if (sr < 0)
				{
					var _ = t0;
					t0 = t1;
					t1 = _;
				}

				return t1 >= 0 && t0 <= 1;
			}

			if (rCrossSIsZero) // lines are parallel
				return false;

			var t = pq.Cross(s) / rCrossS;
			var u = pqCrossR / r.Cross(s);

			return 0 <= t && t <= 1 && 0 <= u && u <= 1;
		}

		public bool HasCommonBoundaryPoint(Segment other)
		{
			return Start.Equals(other.Start)
				|| Start.Equals(other.End)
				|| End.Equals(other.Start)
				|| End.Equals(other.End);
		}
	}
}
