using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DS
{
	public static class BifurcationDiagram
	{
		public class D12VsD21Result
		{
			public List<PointD> EquilibriumPoints { get; }
			public List<PointD> InfinityPoints { get; }
			public Dictionary<int, List<PointD>> CyclePoints { get; }

			public D12VsD21Result()
			{
				EquilibriumPoints = new List<PointD>();
				InfinityPoints = new List<PointD>();
				CyclePoints = new Dictionary<int, List<PointD>>();

				for (var i = 2; i < 16; i++)
				{
					CyclePoints[i] = new List<PointD>();
				}
			}
		}

		public static IEnumerable<(double D12, double X1, double X2)> GetD12VsX(Model model, PointX start,
			double d12End, double step)
		{
			for (; model.D12 < d12End; model.D12 += step)
			{
				var points = PhaseTrajectory.Get(model, start, 10000, 2000);

				if (points[0].IsInfinity()) continue;

				foreach (var (x1, x2) in points)
					yield return (model.D12, x1, x2);
			}
		}

		public static IEnumerable<(double D12, double X1, double X2)> GetD12VsXParallel(Model model, PointX start, double d12End, double step)
		{
			var processorCount = Environment.ProcessorCount;
			var tasks = new Task<IEnumerable<(double D12, double X1, double X2)>>[processorCount];
			var d12Part = (d12End - model.D12) / processorCount;

			for (var i = 0; i < processorCount; i++)
			{
				var copy = model.Copy();
				var d12PartEnd = model.D12 + d12Part * (i + 1);
				copy.D12 = model.D12 + d12Part * i;

				tasks[i] = Task.Run(() => GetD12VsX(copy, start, d12PartEnd, step));
			}

			foreach (var task in tasks)
			foreach (var values in task.Result)
				yield return values;
		}

		public static D12VsD21Result GetD12VsD21(Model model, PointX start, double d12End, double d21End,
			double step1, double step2)
		{
			var d21Start = model.D21;
			var result = new D12VsD21Result();

			for (; model.D12 < d12End; model.D12 += step1)
			{
				model.D21 = d21Start;
				for (; model.D21 < d21End; model.D21 += step2)
				{
					var point = PhaseTrajectory.Get(model, start, 10000, 1)[0];

					if (point.IsInfinity())
						result.InfinityPoints.Add(new PointD(model.D12, model.D21));

					var next = model.GetNextPoint(point);

					if (point.AlmostEquals(next))
					{
						result.EquilibriumPoints.Add(new PointD(model.D12, model.D21));
						continue;
					}

					var cycle = FindCycle(model, point, next);

					if (!cycle.Found) continue;

					result.CyclePoints[cycle.Period].Add(new PointD(model.D12, model.D21));
				}
			}

			return result;
		}

		public static D12VsD21Result GetD12VsD21Parallel(Model model, PointX start, double d12End, double d21End,
			double step1, double step2)
		{
			var result = new D12VsD21Result();
			var processorCount = Environment.ProcessorCount;
			var tasks = new Task<D12VsD21Result>[processorCount];
			var d12Part = (d12End - model.D12) / processorCount;

			for (var i = 0; i < processorCount; i++)
			{
				var copy = model.Copy();
				var d12PartEnd = model.D12 + d12Part * (i + 1);
				copy.D12 = model.D12 + d12Part * i;

				tasks[i] = Task.Run(() => GetD12VsD21(copy, start, d12PartEnd, d21End, step1, step2));
			}

			foreach (var task in tasks)
			{
				var taskResult = task.Result;

				result.EquilibriumPoints.AddRange(taskResult.EquilibriumPoints);
				result.InfinityPoints.AddRange(taskResult.InfinityPoints);

				foreach (var period in taskResult.CyclePoints.Keys)
					result.CyclePoints[period].AddRange(taskResult.CyclePoints[period]);
			}

			return result;
		}

		private static (bool Found, int Period) FindCycle(Model model, PointX first, PointX second)
		{
			var points = new PointX[16];
			points[0] = first;
			points[1] = second;

			for (var i = 1; i < points.Length - 1; i++)
			{
				points[i + 1] = model.GetNextPoint(points[i]);

				if (points[0].AlmostEquals(points[i + 1]))
					return (true, i + 1);
			}

			return (false, 0);
		}
	}
}
