using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;

namespace DS
{
    public class Lc : List<PointX>
    {
        public IEnumerable<Segment> Segments =>
            Enumerable.Range(0, Count - 1)
                .Select(i => new Segment(this[i], this[i + 1]));

        public Lc(IEnumerable<PointX> points) : base(points)
        {
        }

        public Lc GetNextLc(DeterministicModel model)
        {
            return new Lc(this.Select(model.GetNextPoint));
        }

        public static double GetX2(DeterministicModel model, double x1)
        {
            return (model.B1 * model.B2 - 2 * model.B2 * model.Px * x1) /
                (2 * model.B1 * model.Px - 4 * model.Px * model.Px * x1);
        }
    }
}
