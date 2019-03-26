using System.Collections.Generic;
using System.Linq;

namespace DS
{
	public static class StochasticTest
	{
		public static ChartForm Test1(DeterministicModel dModel, StochasticModel sModel)
		{
			sModel.Sigma1 = 0;
			sModel.Sigma2 = 0;
			sModel.Sigma3 = 1;
			sModel.Eps = 0.1;
			sModel.D21 = 0.0075;
			dModel.D21 = 0.0075;

			const double step = 0.000002;

			IEnumerable<(double D12, double X1)> FirstAttractor()
			{
				dModel.D12 = 0.000045;
				sModel.D12 = 0.000045;
				return BifurcationDiagram.GetD12VsXByPrevious(dModel, sModel, new PointX(20, 40), 0.00245, step*2)
					.Distinct()
					.Select(v => (v.D12, v.X1));
			}

			IEnumerable<(double D12, double X1)> SecondAttractor()
			{
				dModel.D12 = 0.00145;
				sModel.D12 = 0.00145;
				return BifurcationDiagram.GetD12VsXByPrevious(dModel, sModel, new PointX(20, 40), 0.0019746, step)
					.Distinct()
					.Select(v => (v.D12, v.X1));
			}

			IEnumerable<(double D12, double X1)> ThirdAttractor()
			{
				dModel.D12 = 0.002166;
				sModel.D12 = 0.002166;

				return BifurcationDiagram.GetD12VsXByPrevious(dModel, sModel, new PointX(34.5964044040563, 44.473609663403728),
						0.0023825, step)
					.Distinct()
					.Select(p => (p.D12, p.X1));
			}

			var points = ThirdAttractor().ToList();

			PointSaver.SaveToFile("d12_x1_3_stochastic.txt", points);

			return new ChartForm(points, 0, 0.00245, 0, 45);
		}
	}
}
