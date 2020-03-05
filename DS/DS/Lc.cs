using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;

namespace DS
{
    public class Lc
    {
        public IList<PointX> Points { get; }

        public IEnumerable<Segment> Segments =>
            Enumerable.Range(0, Points.Count - 1)
                .Select(i => new Segment(Points[i], Points[i + 1]));

        public Lc(IList<PointX> points)
        {
            Points = points;
        }

        public Lc GetNextLc(DeterministicModel model)
        {
            return new Lc(Points.Select(model.GetNextPoint).ToList());
        }

        public static double GetX2(DeterministicModel model, double x1)
        {
            return (model.B1 * model.B2 - 2 * model.B2 * model.Px * x1) /
                (2 * model.B1 * model.Px - 4 * model.Px * model.Px * x1);
        }
    }
}
