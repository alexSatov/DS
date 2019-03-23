using System.Collections.Generic;
using System.Linq;

namespace DS
{
	public static class StochasticTest
	{
		public static ChartForm Test1(StochasticModel model)
		{
			model.Sigma1 = 1;
			model.Sigma2 = 1;
			model.Sigma3 = 0;
			model.Eps = 0.1;
			model.D21 = 0.0075;

			const double step = 0.000002;

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

			PointSaver.SaveToFile("d12_x1_1_.stochastic.txt", points);

			return new ChartForm(points, 0, 0.00245, 0, 45);
		}
	}
}
