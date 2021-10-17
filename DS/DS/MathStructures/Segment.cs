using System;
using System.Collections.Generic;
using DS.MathStructures.Points;
using DS.MathStructures.Vectors;

namespace DS.MathStructures
{
    public readonly struct Segment
    {
        public PointX Start { get; }
        public PointX End { get; }
        public Vector2D Vector { get; }

        public double Length => Vector.Length;
        public PointX Center => Start.GetCenterPointWith(End);

        public Segment(PointX start, PointX end)
        {
            Start = start;
            End = end;
            Vector = new Vector2D(start, end);
        }

        public bool Contains(PointX point, double eps = 0.00001)
        {
            return Math.Abs((point.X - Start.X) / (End.X - Start.X) - (point.Y - Start.Y) / (End.Y - Start.Y)) < eps;
        }

        /// <summary>
        /// Gets the intersection of two segments if it exists
        /// </summary>
        /// <remarks>https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282</remarks>/>
        /// <param name="other"></param>
        /// <param name="includeBoundaryPoints">Determines whether to include boundary points case (if same, return start point)</param>
        /// <param name="includeOverlap">Determines whether to include overlap case (point will be null)</param>
        public IntersectPoint GetIntersection(Segment other, bool includeBoundaryPoints = false,
            bool includeOverlap = false)
        {
            var commonBoundaryPoint = GetCommonBoundaryPoint(other);
            if (commonBoundaryPoint.HasValue)
                return new IntersectPoint(includeBoundaryPoints,
                    includeBoundaryPoints ? commonBoundaryPoint.Value : (PointX?) null);

            var (p, r, q, s) = (Start, Vector, other.Start, other.Vector);
            var pq = new Vector2D(p, q);
            var rCrossS = r.Cross(s);
            var pqCrossR = pq.Cross(r);
            var rCrossSIsZero = Math.Abs(rCrossS) < 0.0000001;
            var pqCrossRIsZero = Math.Abs(pqCrossR) < 0.0000001;

            if (rCrossSIsZero && pqCrossRIsZero) // lines are collinear
            {
                if (!includeOverlap)
                    return new IntersectPoint(false, null);

                var sr = s.Dot(r);
                var rSqr = r.Dot(r);
                var t0 = pq.Dot(r) / rSqr;
                var t1 = t0 + sr / rSqr;

                if (sr < 0)
                {
                    (t0, t1) = (t1, t0);
                }

                return new IntersectPoint(t1 >= 0 && t0 <= 1, null);
            }

            if (rCrossSIsZero) // lines are parallel
                return new IntersectPoint(false, null);

            var t = pq.Cross(s) / rCrossS;
            var u = pqCrossR / r.Cross(s);

            return 0 <= t && t <= 1 && 0 <= u && u <= 1
                ? new IntersectPoint(true, p + r * t)
                : new IntersectPoint(false, null);
        }

        public PointX? GetCommonBoundaryPoint(Segment other)
        {
            return Start.Equals(other.Start) || Start.Equals(other.End)
                ? Start
                : End.Equals(other.Start) || End.Equals(other.End)
                    ? End
                    : (PointX?) null;
        }

        public IEnumerable<PointX> GetBoundaryPoints()
        {
            yield return Start;
            yield return End;
        }
    }
}
