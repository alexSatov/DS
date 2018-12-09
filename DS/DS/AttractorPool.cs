using System.Collections.Generic;

namespace DS
{
	public static class AttractorPool
	{
		public static Dictionary<PointX, HashSet<PointX>> GetX1VsX2(Model model, PointX leftBottom, PointX rightTop,
			double step1, double step2)
		{
			var result = new Dictionary<PointX, HashSet<PointX>>();

			for (var x1 = leftBottom.X1; x1 <= rightTop.X1; x1 += step1)
			{
				for (var x2 = leftBottom.X2; x2 <= rightTop.X2; x2 += step2)
				{
					var point = PhaseTrajectory.Get(model, new PointX(x1, x2), 10000, 1)[0];
					var found = false;

					foreach (var attractor in result.Keys)
					{
						found = point.AlmostEquals(attractor);

						if (!found) continue;

						result[attractor].Add(point);
						break;
					}

					if (!found)
						result[point] = new HashSet<PointX>();
				}
			}

			return result;
		}
	}
}
