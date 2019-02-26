using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DS
{
	public static class Lyapunov
	{
		public static IEnumerable<(double D12, double L1, double L2)> GetIndicators(Model model, PointX start,
			double d12End, double step, bool byPrevious = false, double eps = 0.00001, double t = 1000000)
		{
			var result = new List<(double D12, double L1, double L2)>();
			var previous = start;

			for (; model.D12 < d12End; model.D12 += step)
			{
				var o = byPrevious ? previous : start;
				var a = new PointX(o.X1 + eps, o.X2);
				var b = new PointX(o.X1, o.X2 + eps);

				double z1 = 0, z2 = 0;

				for (var i = 0; i < t; i++)
				{
					var (p, p1, p2) = (model.GetNextPoint(o), model.GetNextPoint(a), model.GetNextPoint(b));
					var (v1, v2) = (new Vector(p1, p), new Vector(p2, p));

					var b1 = v1 / v1.Length;
					var b2 = v2 - v2 * b1 * b1;

					z1 += Math.Log10(v1.Length / eps);
					z2 += Math.Log10(b2.Length / eps);

					b1 *= eps;
					b2 = b2 / b2.Length * eps;

					o = p;
					a = new PointX(b1.X + p.X, b1.Y + p.Y);
					b = new PointX(b2.X + p.X, b2.Y + p.Y);
				}

				previous = o;

				var (l1, l2) = (z1 / t, z2 / t);

				result.Add((model.D12, l1, l2));
			}

			return result;
		}

		public static IEnumerable<(double D12, double L1, double L2)> GetIndicatorsParallel(Model model, PointX start,
			double d12End, double step, bool byPrevious = false, double eps = 0.00001, double t = 1000000)
		{
			var processorCount = Environment.ProcessorCount;
			var tasks = new Task<IEnumerable<(double D12, double L1, double L2)>>[processorCount];
			var d12Part = (d12End - model.D12) / processorCount;

			for (var i = 0; i < processorCount; i++)
			{
				var copy = model.Copy();
				var d12PartEnd = model.D12 + d12Part * (i + 1);
				copy.D12 = model.D12 + d12Part * i;

				tasks[i] = Task.Run(() => GetIndicators(copy, start, d12PartEnd, step, byPrevious, eps, t));
			}

			foreach (var task in tasks)
			foreach (var values in task.Result)
				yield return values;
		}
	}
}
