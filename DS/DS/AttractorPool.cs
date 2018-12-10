using System;
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
			for (var x2 = leftBottom.X2; x2 <= rightTop.X2; x2 += step2)
			{
				var startPoint = new PointX(x1, x2);
				var point = PhaseTrajectory.Get(model, startPoint, 10000, 1)[0];

				if (point.IsInfinity()) continue;

				var key = new PointX((int) Math.Round(point.X1), (int) Math.Round(point.X2));
				var found = false;

				foreach (var attractor in result.Keys)
				{
					found = key.AlmostEquals(attractor, 0);

					if (!found) continue;

					result[key].Add(startPoint);

					break;
				}

				if (!found)
					result[key] = new HashSet<PointX>();
			}

			return result;
		}
	}
}
