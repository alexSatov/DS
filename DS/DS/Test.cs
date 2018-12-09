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

			var points = PhaseTrajectory.Get(model, new PointX(20, 40), 2000, 1000).Select(p => (p.X1, p.X2));

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

			var infs = points.InfinityPoints
				.Select(p => (p.D12, p.D21));

			var eqX1EqX2Eq0 = points.EquilibriumPoints
				.Where(t => t.X.AlmostEquals(new PointX(0, 0)))
				.Select(t => (t.D.D12, t.D.D21));

			var eqX2Gt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 > 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var eqX2Lt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 < 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var chart = new ChartForm(infs, 0, 0.004, 0, 0.016, Color.Gray);

			chart.AddSeries("eqX2Lt2X1", eqX2Lt2X1, Color.ForestGreen);
			chart.AddSeries("eqX2Gt2X1", eqX2Gt2X1, Color.DeepSkyBlue);
			chart.AddSeries("eqX1EqX2Eq0", eqX1EqX2Eq0, Color.Goldenrod);

			Console.WriteLine($"equilibrium x2 < 2x1 count: {chart.SeriesPointCount["eqX2Lt2X1"]}");
			Console.WriteLine($"equilibrium x2 > 2x1 count: {chart.SeriesPointCount["eqX2Gt2X1"]}");
			Console.WriteLine($"equilibrium x1 = x2 = 0 count: {chart.SeriesPointCount["eqX1EqX2Eq0"]}");

			return chart;
		}

		public static ChartForm Test5(Model model)
		{
			model.D12 = 0.00005;
			model.D21 = 0.007;

			var points = BifurcationDiagram.GetD12VsD21ParallelByD21(model, new PointX(20, 40), 0.00245, 0.008,
				0.00001225, 0.000005);

			var infs = points.InfinityPoints
				.Select(p => (p.D12, p.D21));

			var eqX1EqX2Eq0 = points.EquilibriumPoints
				.Where(t => t.X.AlmostEquals(new PointX(0, 0)))
				.Select(t => (t.D.D12, t.D.D21));

			var eqX2Gt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 > 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var eqX2Lt2X1 = points.EquilibriumPoints
				.Where(t => t.X.X2 < 2 * t.X.X1)
				.Select(t => (t.D.D12, t.D.D21));

			var chart = new ChartForm(infs, 0, 0.00245, 0.007, 0.008, Color.Gray);

			chart.AddSeries("eqX2Lt2X1", eqX2Lt2X1, Color.ForestGreen);
			chart.AddSeries("eqX2Gt2X1", eqX2Gt2X1, Color.DeepSkyBlue);
			chart.AddSeries("eqX1EqX2Eq0", eqX1EqX2Eq0, Color.Goldenrod);

			Console.WriteLine($"equilibrium x2 < 2x1 count: {chart.SeriesPointCount["eqX2Lt2X1"]}");
			Console.WriteLine($"equilibrium x2 > 2x1 count: {chart.SeriesPointCount["eqX2Gt2X1"]}");
			Console.WriteLine($"equilibrium x1 = x2 = 0 count: {chart.SeriesPointCount["eqX1EqX2Eq0"]}");

			AddCycles(chart, points.CyclePoints);

			return chart;
		}

		// 3 аттрактора
		public static ChartForm Test6(Model model)
		{
			const double step = 0.000005;
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
				model.D12 = 0.00158;
				var lrResult = BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.00245, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
				var rlResult = BifurcationDiagram.GetD12VsXByPrevious(model.Copy(), new PointX(20, 40), 0, step, true)
					.Distinct()
					.Select(v => (v.D12, v.X1));
				return lrResult
					.Concat(rlResult)
					.OrderBy(p => p.Item1)
					.Where(p => p.Item1 >= 0.0014499999 && p.Item1 <= 0.0019700001);
			}

			// [0.00217, 0.00238]
			IEnumerable<(double D12, double X1)> ThirdAttractor()
			{
				model.D12 = 0.00227;
				var lrResult = BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.00245, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
				var rlResult = BifurcationDiagram.GetD12VsXByPrevious(model.Copy(), new PointX(20, 40), 0, step, true)
					.Distinct()
					.Select(v => (v.D12, v.X1));
				return lrResult
					.Concat(rlResult)
					.Where(p => p.Item2 > 31)
					.OrderBy(p => p.Item1);
			}

			var points = FirstAttractor().ToList();
			var lines = points
				.Select(p => $"{p.D12.ToString(CultureInfo.InvariantCulture)} {p.X1.ToString(CultureInfo.InvariantCulture)}");

			File.WriteAllLines("d12_x1_1.txt", lines);

			return new ChartForm(points, 0, 0.00245, 0, 45);
		}

		private static void AddCycles(ChartForm chart, IReadOnlyDictionary<int, List<PointD>> cyclePoints)
		{
			var colors = new[]
			{
				Color.DarkBlue, Color.Red, Color.Blue, Color.Green, Color.DarkGreen, Color.DarkOliveGreen,
				Color.DeepSkyBlue, Color.Orange, Color.Coral, Color.DarkGoldenrod, Color.Violet,
				Color.SaddleBrown, Color.Thistle, Color.SlateBlue
			};

			for (var i = 2; i < 16; i++)
			{
				chart.AddSeries($"cycle{i}", cyclePoints[i].Select(p => (p.D12, p.D21)), colors[i - 2]);
				Console.WriteLine($"cycle{i} count: {chart.SeriesPointCount[$"cycle{i}"]}");
			}
		}
	}
}
