using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DS
{
	public static class DeterministicTest
	{
		public static ChartForm Test1(DeterministicModel model)
		{
			model.D12 = 0.002;
			model.D21 = 0.0063;

			var points = PhaseTrajectory.Get(model, new PointX(20, 40), 2000, 1000)
				.Select(p => (p.X1, p.X2));

			return new ChartForm(points, 0, 40, 0, 80);
		}

		public static ChartForm Test2(DeterministicModel model)
		{
			model.D21 = 0.0063;

			var points = BifurcationDiagram.GetD12VsXParallel(model, new PointX(10, 10), 0.0025, 0.000001)
				.Distinct()
				.Select(v => (v.D12, v.X1));

			return new ChartForm(points, 0.0017, 0.0025, 10, 45);
		}

		public static ChartForm Test3(DeterministicModel model)
		{
			model.D12 = 0.0017;
			model.D21 = 0.005;

			var points = BifurcationDiagram.GetD12VsD21ParallelByD21(model, new PointX(20, 40), 0.00245, 0.00792,
				0.000005, 0.00002);

			var eqX2Gt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 > 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var eqX2Lt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 < 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var chart = new ChartForm(eqX2Gt2X1, 0.0017, 0.00245, 0.005, 0.00792);

			chart.AddSeries("equilibrium x2 < 2x1", eqX2Lt2X1, Color.Aqua);

			AddCycles(chart, points.CyclePoints);

			return chart;
		}

		public static ChartForm Test4(DeterministicModel model)
		{
			var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(20, 40), 0.004, 0.016,
				0.00002, 0.00008);

			return GetCyclesChart(points, 0, 0.004, 0, 0.016);
		}

		public static ChartForm Test5(DeterministicModel model)
		{
			model.D12 = 0.00005;
			model.D21 = 0.007;
			var points = BifurcationDiagram.GetD12VsD21(model, new PointX(20, 40), 0.00245, 0.008,
				0.000004, 0.0000016);

			//(0.0015899999999999996, 0.0072622000000000051)
			//model.D12 = 0.00159;
			//model.D21 = 0.0072622;
			//var points = BifurcationDiagram.GetD12VsD21ParallelByD21(model, new PointX(20, 40), 0.00245, 0.0076,
			//	0.000004, 0.0000017);

			//(0.00200600000000001, 0.0075274500000000024); (19,9106940008894, 62,4944666763375)
			//model.D12 = 0.002006;
			//model.D21 = 0.00752745;
			//var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(19.9106940008894, 62.4944666763375),
			//	0.0018, 0.0078, 0.000004, 0.0000017, true);

			//(0.00200600000000001, 0.0075274500000000024); (19,9106940008894, 62,4944666763375)
			//model.D12 = 0.002006;
			//model.D21 = 0.00752745;
			//var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(19.9106940008894, 62.4944666763375),
			//	0.00245, 0.0078, 0.000004, 0.0000017);

			//var keks = points.CyclePoints[3]
			//	.OrderByDescending(p => p.D.D21)
			//	.Take(5)
			//	.Select(p => (p.D.D12, p.D.D21));

			var chart = GetCyclesChart(points, 0, 0.00245, 0.007, 0.008);

			//chart.AddSeries("kek", keks, Color.Black);

			return chart;
		}

		public static ChartForm Test6(DeterministicModel model)
		{
			const double step = 0.000002;
			model.D21 = 0.0075;

			IEnumerable<(double D12, double X1)> FirstAttractor()
			{
				model.D12 = 0.000045;
				return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.00245, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
			}

			IEnumerable<(double D12, double X1)> SecondAttractor()
			{
				model.D12 = 0.00145;
				return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.0019746, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
			}

			IEnumerable<(double D12, double X1)> ThirdAttractor()
			{
				model.D12 = 0.002166;
				
				return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(34.5964044040563, 44.473609663403728),
						0.0023825, step)
					.Distinct()
					.Select(p => (p.D12, p.X1));
			}

			var points = FirstAttractor().ToList();

			//PointSaver.SaveToFile("d12_x1_1.txt", points);

			return new ChartForm(points, 0, 0.00245, 0, 45);
		}

		// d12 = 0.00145 - ЗИК и 3х цикл: (6, 62); (17, 44); (24, 75)
		// d12 = 0.00197 - равновесие и 3х цикл: (12, 65); (20, 62); (23, 72)
		// d12 = 0.00217 - равновесие и равновесие: (18, 68) и (35, 44)
		// d12 = 0.002382 - хаос и равновесие: (20, 68)
		public static ChartForm Test7(DeterministicModel model)
		{
			model.D12 = 0.002382;
			model.D21 = 0.0075;

			var points = AttractorPool.GetX1VsX2Parallel(model, new PointX(-5, -5), new PointX(45, 85), 0.05, 0.09);

			return GetAttractorPoolChartExact(points, -5, 45, -5, 85);
		}

		public static ChartForm Test8(DeterministicModel model)
		{
			var points = BifurcationDiagram.GetD12VsD21ByPreviousPolarParallel(model, new PointX(20, 40),
				new PointD(0.00159, 0.0072622), 0.00245, 0.008, 0.001, 0.000004, 0.0000017);
			var chart = GetCyclesChart(points, 0, 0.00245, 0.007, 0.008);

			return chart;
		}

		public static ChartForm Test9(DeterministicModel model)
		{
			const double step = 0.0000002;
			model.D21 = 0.0075;

			IEnumerable<(double D12, double L1, double L2)> FirstAttractor()
			{
				model.D12 = 0.000045;
				return Lyapunov.GetD12Indicators(model, new PointX(20, 40), 0.00245, step, true, t: 500000);
			}

			IEnumerable<(double D12, double L1, double L2)> SecondAttractor()
			{
				model.D12 = 0.00145;
				return Lyapunov.GetD12Indicators(model, new PointX(20, 40), 0.0019746, step, true, t: 500000);
			}

			IEnumerable<(double D12, double L1, double L2)> ThirdAttractor()
			{
				model.D12 = 0.002166;
				return Lyapunov.GetD12Indicators(model, new PointX(34.5964044040563, 44.473609663403728),
						0.0023825, step, true, t: 500000);
			}

			var points = FirstAttractor().ToList();
			var l1Points = points.Select(p => (p.D12, p.L1)).ToList();
			var l2Points = points.Select(p => (p.D12, p.L2)).ToList();

			//Console.WriteLine($"Max l1: {l1Points.Max(p => p.Item2)}");
			//Console.WriteLine($"Min l1: {l1Points.Min(p => p.Item2)}");
			//Console.WriteLine($"Max l2: {l2Points.Max(p => p.Item2)}");
			//Console.WriteLine($"Min l2: {l2Points.Min(p => p.Item2)}");

			var chart = new ChartForm(l1Points, 0, 0.00245, -3.5, 1);

			chart.AddSeries("D12vsL2", l2Points, Color.Red);

			PointSaver.SaveToFile("lyapunov\\l1.txt", l1Points);
			PointSaver.SaveToFile("lyapunov\\l2.txt", l2Points);

			return chart;
		}

		// 0.000003, 0.000001363 (по 734)
		public static ChartForm Test10(DeterministicModel model)
		{
			model.D12 = 0.0002;
			model.D21 = 0.007;

			var points = Lyapunov.GetIndicatorsParallelByD21(model, new PointX(20, 40), 0.0024, 0.008, 0.000003, 0.000001363)
				.OrderByDescending(p => p.D21)
				.ThenBy(p => p.D12)
				.ToList();

			var l1Result = new StringBuilder(points[0].L1.Format());
			var l2Result = new StringBuilder(points[0].L2.Format());

			var currentD21 = points[0].D21;

			foreach (var point in points.Skip(1))
			{
				if (point.D21 != currentD21)
				{
					currentD21 = point.D21;

					l1Result.AppendLine();
					l2Result.AppendLine();
					l1Result.Append(point.L1.Format());
					l2Result.Append(point.L2.Format());

					continue;
				}

				l1Result.Append(" ");
				l2Result.Append(" ");
				l1Result.Append(point.L1.Format());
				l2Result.Append(point.L2.Format());
			}

			File.WriteAllText("lyapunovMap\\l1.txt", l1Result.ToString());
			File.WriteAllText("lyapunovMap\\l2.txt", l2Result.ToString());

			return null;
		}

		public static ChartForm Test11(DeterministicModel model)
		{
			model.D12 = 0.002382;
			model.D21 = 0.0075;

			var points = PhaseTrajectory.Get(model, new PointX(32.95, 47.38), 10000, 10000);
			var chart = new ChartForm(points, -5, 45, -5, 85);

			PointSaver.SaveToFile("chaosAttractor.txt", points);

			return chart;
		}

		private static ChartForm GetCyclesChart(BifurcationDiagram.D12VsD21Result points,
			double ox1, double ox2, double oy1, double oy2)
		{
			var zeroPoint = new PointX(0, 0);

			var infinity = points.InfinityPoints.ToList();

			var eqX1EqX2Eq0 = points.EquilibriumPoints
				.Where(t => t.X.AlmostEquals(zeroPoint))
				.Select(t => t.D)
				.ToList();

			var eqX2Gt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 > 2 * t.X.X1)
				.Select(t => t.D)
				.ToList();

			var eqX2Lt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 < 2 * t.X.X1)
				.Select(t => t.D)
				.ToList();

			var chart = new ChartForm(infinity, ox1, ox2, oy1, oy2, Color.Gray, "infinity");

			chart.AddSeries("equilibrium x2 < 2x1", eqX2Lt2X1, Color.ForestGreen);
			chart.AddSeries("equilibrium x2 > 2x1", eqX2Gt2X1, Color.DeepSkyBlue);
			chart.AddSeries("equilibrium x1 = x2 = 0", eqX1EqX2Eq0, Color.Goldenrod);

			PointSaver.SaveToFile("map\\infinity.txt", infinity);
			PointSaver.SaveToFile("map\\eqX2Lt2X1.txt", eqX2Lt2X1);
			PointSaver.SaveToFile("map\\eqX2Gt2X1.txt", eqX2Gt2X1);

			AddCycles(chart, points.CyclePoints);

			return chart;
		}

		private static void AddCycles(ChartForm chart, IReadOnlyDictionary<int, List<(PointD D, PointX X)>> cyclePoints)
		{
			var colors = new[]
			{
				Color.DarkBlue, Color.Red, Color.Blue, Color.Green, Color.DarkGreen, Color.DarkOliveGreen,
				Color.DeepSkyBlue, Color.Orange, Color.Coral, Color.DarkGoldenrod, Color.Violet,
				Color.SaddleBrown, Color.Thistle, Color.SlateBlue
			};

			for (var i = 2; i < 16; i++)
			{
				var points = cyclePoints[i].Select(p => (p.D.D12, p.D.D21)).ToList();
				var name = $"cycle{i}";

				chart.AddSeries(name, points, colors[i - 2]);
				PointSaver.SaveToFile($"map\\{name}.txt", points);
			}
		}

		

		private static ChartForm GetAttractorPoolChart(Dictionary<PointX, HashSet<PointX>> attractorPoints,
			double ox1, double ox2, double oy1, double oy2)
		{
			Console.WriteLine($"attractors count: {attractorPoints.Keys.Count}");

			var chart = new ChartForm(new List<(double x, double y)>(), ox1, ox2, oy1, oy2);

			foreach (var attractor in attractorPoints.Keys.OrderBy(p => p.X1))
			{
				var points = attractorPoints[attractor].Select(p => (p.X1, p.X2)).ToList();
				var name = $"attractor {attractor}";

				chart.AddSeries(name, points);
				PointSaver.SaveToFile($"pool\\{name}.txt", points);
			}

			chart.AddSeries("attractors", attractorPoints.Keys.Select(p => (p.X1, p.X2)), Color.DeepPink);

			return chart;
		}

		// d12 = 0.00145 - ЗИК и 3х цикл: (6, 62); (17, 44); (24, 75)
		// d12 = 0.00197 - равновесие и 3х цикл: (12, 65); (20, 62); (23, 72)
		// d12 = 0.00217 - равновесие и равновесие: (18, 68) и (35, 44)
		// d12 = 0.002382 - хаос и равновесие: (20, 68)
		private static ChartForm GetAttractorPoolChartExact(Dictionary<PointX, HashSet<PointX>> attractorPoints,
			double ox1, double ox2, double oy1, double oy2)
		{
			var eqAttractor = new HashSet<PointX> { new PointX(20, 68) };

			var eq = attractorPoints
				.Where(kv => eqAttractor.Contains(kv.Key))
				.SelectMany(kv => kv.Value)
				.ToList();

			var chaos = attractorPoints
				.Where(kv => !eqAttractor.Contains(kv.Key))
				.SelectMany(kv => kv.Value)
				.ToList();

			var chaosAttractors = attractorPoints.Keys
				.Where(p => !eqAttractor.Contains(p))
				.ToList();

			var chart = new ChartForm(eq, ox1, ox2, oy1, oy2);
			chart.AddSeries(nameof(chaos), chaos, Color.DarkOrange);
			chart.AddSeries(nameof(chaosAttractors), chaosAttractors, Color.DeepPink);
			chart.AddSeries(nameof(eqAttractor), eqAttractor, Color.Yellow);

			PointSaver.SaveToFile($"pool_simple\\{nameof(eqAttractor)}.txt", eqAttractor);
			PointSaver.SaveToFile($"pool_simple\\{nameof(chaosAttractors)}.txt", chaosAttractors);
			PointSaver.SaveToFile($"pool_simple\\{nameof(eq)}.txt", eq);
			PointSaver.SaveToFile($"pool_simple\\{nameof(chaos)}.txt", chaos);

			return chart;
		}
	}
}
