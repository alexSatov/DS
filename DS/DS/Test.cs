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

			var points = BifurcationDiagram.GetD12VsXParallel(model, new PointX(20, 40), 0.0025, 0.000005)
				.Distinct()
				.Select(v => (v.D12, v.X1));

			return new ChartForm(points, 0.0017, 0.0025, 10, 45);
		}

		public static ChartForm Test3(Model model)
		{
			model.D12 = 0.0017;
			model.D21 = 0.005;

			var points = BifurcationDiagram.GetD12VsD21Parallel(model, new PointX(20, 40), 0.00245, 0.00792, 0.000005, 0.00002)
				.CyclePoints[2]
				.Select(p => (p.D12, p.D21));

			return new ChartForm(points, 0.0017, 0.00245, 0.005, 0.00792);
		}
	}
}
