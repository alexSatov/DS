using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;

namespace DS
{
    public class Lc : List<PointX>
    {
        public bool IsX1 { get; }

        public IEnumerable<Segment> Segments =>
            Enumerable.Range(0, Count - 1)
                .Select(i => new Segment(this[i], this[i + 1]));

        public Lc(IEnumerable<PointX> points, bool isX1 = true) : base(points)
        {
            IsX1 = isX1;
        }

        public Lc GetNextLc(DeterministicModel model)
        {
            return new Lc(this.Select(model.GetNextPoint), IsX1);
        }

        public static double GetX2(DeterministicModel model, double x1)
        {
            return (model.B1 * model.B2 - 2 * model.B2 * model.Px * x1) /
                (2 * model.B1 * model.Px - 4 * model.Px * model.Px * x1);
        }
    }
}
