using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;
using DS.MathStructures.Points;
using DS.Models;

namespace DS
{
    public class Lc : List<PointX>
    {
        public LcType Type { get; }

        public IEnumerable<Segment> Segments =>
            Enumerable.Range(0, Count - 1)
                .Select(i => new Segment(this[i], this[i + 1]));

        public Lc(IEnumerable<PointX> points, LcType type = LcType.H) : base(points)
        {
            Type = type;
        }

        public Lc GetNextLc(BaseModel baseModel)
        {
            return new Lc(this.Select(baseModel.GetNextPoint), Type);
        }
    }

    public enum LcType
    {
        H, // X2 = const
        V // X1 = const
    }
}
