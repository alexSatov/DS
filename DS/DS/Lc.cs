using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;

namespace DS
{
    public class Lc
	{
		private readonly IList<PointX> points;

		public Lc(IList<PointX> points)
		{
			this.points = points;
		}

		public static double GetX2(DeterministicModel model, double x1)
        {
            return (model.B1 * model.B2 - 2 * model.B2 * model.Px * x1) /
                   (2 * model.B1 * model.Px - 4 * model.Px * model.Px * x1);
        }

        public static IEnumerable<PointX> GetNextLc(DeterministicModel model, IEnumerable<PointX> lc)
        {
            return lc.Select(model.GetNextPoint);
        }
    }
}
