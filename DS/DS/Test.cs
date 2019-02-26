﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DS
{
	public static class Test
	{
		public static ChartForm Test1(Model model)
		{
			model.D12 = 0.002;
			model.D21 = 0.0063;

			var points = PhaseTrajectory.Get(model, new PointX(20, 40), 2000, 1000)
				.Select(p => (p.X1, p.X2));

			return new ChartForm(points, 0, 40, 0, 80);
		}

		public static ChartForm Test2(Model model)
		{
			model.D21 = 0.0063;

			var points = BifurcationDiagram.GetD12VsX(model, new PointX(10, 10), 0.0025, 0.000005)
				.Distinct()
				.Select(v => (v.D12, v.X1));

			return new ChartForm(points, 0.0017, 0.0025, 10, 45);
		}

		public static ChartForm Test3(Model model)
		{
			model.D12 = 0.0017;
			model.D21 = 0.005;

			var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(20, 40), 0.00245, 0.00792,
				0.000005, 0.00002);

			var eqX2Gt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 > 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var eqX2Lt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 < 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var chart = new ChartForm(eqX2Gt2X1, 0.0017, 0.00245, 0.005, 0.00792);

			chart.AddSeries("eqX2Lt2X1", eqX2Lt2X1, Color.Aqua);
			Console.WriteLine($"equilibrium x2 < 2x1 count: {chart.SeriesPointCount["eqX2Lt2X1"]}");

			AddCycles(chart, points.CyclePoints);

			return chart;
		}

		public static ChartForm Test4(Model model)
		{
			var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(20, 40), 0.004, 0.016,
				0.00002, 0.00008);

			return GetCyclesChart(points, 0, 0.004, 0, 0.016);
		}

		public static ChartForm Test5(Model model)
		{
			//model.D12 = 0.00005;
			//model.D21 = 0.008;
			//var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(20, 40), 0.00245, 0.007,
			//	0.000004, 0.0000017, upToDown: true);

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
			model.D12 = 0.002006;
			model.D21 = 0.00752745;
			var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(19.9106940008894, 62.4944666763375),
				0.00245, 0.0078, 0.000004, 0.0000017);

			//var keks = points.CyclePoints[3]
			//	.OrderByDescending(p => p.D.D21)
			//	.Take(5)
			//	.Select(p => (p.D.D12, p.D.D21));

			var chart = GetCyclesChart(points, 0, 0.00245, 0.007, 0.008);

			//chart.AddSeries("kek", keks, Color.Black);

			return chart;
		}

		// 3 аттрактора
		public static ChartForm Test6(Model model)
		{
			const double step = 0.000001;
			model.D21 = 0.0075;

			IEnumerable<(double D12, double X1)> FirstAttractor()
			{
				model.D12 = 0.000045;
				return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.00245, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
			}

			// [0.00145, 0.00197]
			IEnumerable<(double D12, double X1)> SecondAttractor()
			{
				model.D12 = 0.00145;
				return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.00197, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
			}

			// [0.00218, 0.00238]
			IEnumerable<(double D12, double X1)> ThirdAttractor()
			{
				model.D12 = 0.00218;
				return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.00238, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
			}

			var points = SecondAttractor().ToList();

			SaveToFile("d12_x1_3.txt", points);

			return new ChartForm(points, 0, 0.00245, 0, 45);
		}

		public static ChartForm Test7(Model model)
		{
			model.D12 = 0.00158;
			model.D21 = 0.0075;

			var points = AttractorPool.GetX1VsX2Parallel(model, new PointX(-5, -5), new PointX(45, 85), 0.25, 0.45);

			return GetAttractorPoolChartExact(points, -5, 45, -5, 85);
		}

		public static ChartForm Test8(Model model)
		{
			var points = BifurcationDiagram.GetD12VsD21ByPreviousPolarParallel(model, new PointX(20, 40),
				new PointD(0.00159, 0.0072622), 0.00245, 0.008, 0.01, 0.000004, 0.0000017);
			var chart = GetCyclesChart(points, 0, 0.00245, 0.007, 0.008);

			return chart;
		}

		public static ChartForm Test9(Model model)
		{
			model.D12 = 0.000045;
			model.D21 = 0.0075;

			var points = Lyapunov.GetIndicatorsParallel(model, new PointX(20, 40), 0.00245, 0.000001, true)
				.ToList();

			var l1Points = points.Select(p => (p.D12, p.L1)).ToList();
			var l2Points = points.Select(p => (p.D12, p.L2)).ToList();

			Console.WriteLine($"Max l1: {l1Points.Max(p => p.Item2)}");
			Console.WriteLine($"Min l1: {l1Points.Min(p => p.Item2)}");
			Console.WriteLine($"Max l2: {l2Points.Max(p => p.Item2)}");
			Console.WriteLine($"Min l2: {l2Points.Min(p => p.Item2)}");

			var chart = new ChartForm(l1Points, 0, 0.00245, -6.5, 1);

			chart.AddSeries("D12vsL2", l2Points, Color.Red);

			SaveToFile("lyapunov\\l1.txt", l1Points);
			SaveToFile("lyapunov\\l2.txt", l2Points);

			return chart;
		}

		private static ChartForm GetCyclesChart(BifurcationDiagram.D12VsD21Result points,
			double ox1, double ox2, double oy1, double oy2)
		{
			var zeroPoint = new PointX(0, 0);

			var infinity = points.InfinityPoints
				.Select(p => (p.D12, p.D21))
				.ToList();

			var eqX1EqX2Eq0 = points.EquilibriumPoints
				.Where(t => t.X.AlmostEquals(zeroPoint))
				.Select(t => (t.D.D12, t.D.D21))
				.ToList();

			var eqX2Gt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 > 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21))
				.ToList();

			var eqX2Lt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 < 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21))
				.ToList();

			var chart = new ChartForm(infinity, ox1, ox2, oy1, oy2, Color.Gray);

			chart.AddSeries("eqX2Lt2X1", eqX2Lt2X1, Color.ForestGreen);
			chart.AddSeries("eqX2Gt2X1", eqX2Gt2X1, Color.DeepSkyBlue);
			chart.AddSeries("eqX1EqX2Eq0", eqX1EqX2Eq0, Color.Goldenrod);

			Console.WriteLine($"infinities count: {chart.SeriesPointCount["main"]}");
			Console.WriteLine($"equilibrium x2 < 2x1 count: {chart.SeriesPointCount["eqX2Lt2X1"]}");
			Console.WriteLine($"equilibrium x2 > 2x1 count: {chart.SeriesPointCount["eqX2Gt2X1"]}");
			Console.WriteLine($"equilibrium x1 = x2 = 0 count: {chart.SeriesPointCount["eqX1EqX2Eq0"]}");

			SaveToFile("map\\infinity.txt", infinity);
			SaveToFile("map\\eqX2Lt2X1.txt", eqX2Lt2X1);
			SaveToFile("map\\eqX2Gt2X1.txt", eqX2Gt2X1);

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
				Console.WriteLine($"{name} count: {chart.SeriesPointCount[name]}");
				SaveToFile($"map\\{name}.txt", points);
			}
		}

		private static void SaveToFile(string filename, IEnumerable<(double, double)> points)
		{
			var lines = points
				.Select(p => $"{Format(p.Item1)} {Format(p.Item2)}");

			File.WriteAllLines(filename, lines);
		}

		private static string Format(double value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
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
				Console.WriteLine($"{name} count: {chart.SeriesPointCount[name]}");
				SaveToFile($"pool\\{name}.txt", points);
			}

			chart.AddSeries("attractors", attractorPoints.Keys.Select(p => (p.X1, p.X2)), Color.Blue);

			return chart;
		}

		private static ChartForm GetAttractorPoolChartExact(Dictionary<PointX, HashSet<PointX>> attractorPoints,
			double ox1, double ox2, double oy1, double oy2)
		{
			var cyclePoints = new HashSet<PointX> {new PointX(5, 57), new PointX(21, 40), new PointX(26, 77)};

			var cycle = attractorPoints
				.Where(kv => cyclePoints.Contains(kv.Key))
				.SelectMany(kv => kv.Value)
				.Select(p => (p.X1, p.X2))
				.ToList();

			var other = attractorPoints
				.Where(kv => !cyclePoints.Contains(kv.Key))
				.SelectMany(kv => kv.Value)
				.Select(p => (p.X1, p.X2))
				.ToList();

			var chart = new ChartForm(cycle, ox1, ox2, oy1, oy2);

			chart.AddSeries("other", other, Color.DarkOrange);
			SaveToFile($"pool\\cycle3.txt", cycle);
			SaveToFile($"pool\\other.txt", other);

			return chart;
		}
	}
}
