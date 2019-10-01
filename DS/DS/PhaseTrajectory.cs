using System.Collections.Generic;

namespace DS
{
	public static class PhaseTrajectory
	{
		public static List<PointX> Get(Model model, PointX start, int skipCount, int getCount)
		{
			var current = start;
			var points = new List<PointX>();

			for (var i = 0; i < skipCount + getCount; i++)
			{
				var next = model.GetNextPoint(current);

				if (next.TendsToInfinity(model))
					return new List<PointX> { PointX.Infinity };

				if (i >= skipCount)
					points.Add(next);

				current = next;
			}

			return points;
		}

		public static List<PointX> GetWhile(Model model, PointX start, int skipCount, double eps)
		{
			var points = new List<PointX>();
			var current = start;
			var next = new PointX(100, 100);

			while (skipCount >= 0 || !next.AlmostEquals(start, eps))
			{
				next = model.GetNextPoint(current);

				if (next.TendsToInfinity(model))
					return new List<PointX> { PointX.Infinity };

				if (--skipCount < 0)
					points.Add(next);
				else
					start = next;

				current = next;
			}

			return points;
		}
	}
}
