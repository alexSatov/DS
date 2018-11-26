using System;
using System.Drawing;
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

			var points = BifurcationDiagram.GetD12VsD21Parallel(model, new PointX(20, 40), 0.00245, 0.00792,
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

			var colors = new[]
			{
				Color.DarkBlue, Color.Red, Color.Blue, Color.Green, Color.DarkGreen, Color.DarkOliveGreen,
				Color.DeepSkyBlue, Color.Orange, Color.Coral, Color.DarkGoldenrod, Color.Violet,
				Color.SaddleBrown, Color.Thistle, Color.SlateBlue
			};

			for (var i = 2; i < 16; i++)
			{
				chart.AddSeries($"cycle{i}", points.CyclePoints[i].Select(p => (p.D12, p.D21)), colors[i - 2]);
				Console.WriteLine($"cycle{i} count: {chart.SeriesPointCount[$"cycle{i}"]}");
			}

			return chart;
		}

		public static ChartForm Test4(Model model)
		{
			var points = BifurcationDiagram.GetD12VsD21Parallel(model, new PointX(20, 40), 0.004, 0.016,
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
			model.D21 = 0.007;

			var points = BifurcationDiagram.GetD12VsD21Parallel(model, new PointX(20, 40), 0.00245, 0.008,
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

			return chart;
		}
	}
}
