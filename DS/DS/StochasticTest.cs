using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Accord.Math;
using Accord.Math.Decompositions;

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

		public static ChartForm Test2(DeterministicModel dModel, StochasticModel sModel)
		{
			dModel.D12 = 0.0016;
			dModel.D21 = 0.0075;
			sModel.D12 = 0.0016;
			sModel.D21 = 0.0075;
			sModel.Eps = 0.05;
			sModel.Sigma1 = 1;
			sModel.Sigma2 = 1;
			sModel.Sigma3 = 0;

			var attractorPoints = PhaseTrajectory.Get(dModel, new PointX(20, 40), 9999, 3);
			var points = PhaseTrajectory.Get(sModel, attractorPoints[0], 0, 2000);
			var chart = new ChartForm(points, 0, 32, 32, 80);

			var sensitivityMatrices = SensitivityMatrix.Get(sModel, attractorPoints).ToList();

			for (var i = 0; i < sensitivityMatrices.Count; i++)
			{
				var sensitivityMatrix = sensitivityMatrices[i];
				var eigenvalueDecomposition = new EigenvalueDecomposition(sensitivityMatrix);
				var eigenvalues = eigenvalueDecomposition.RealEigenvalues;
				Console.WriteLine(string.Join("; ", eigenvalues));
				var eigenvectors = eigenvalueDecomposition.Eigenvectors;
				var ellipse = ScatterEllipse.Get(attractorPoints[i], eigenvalues[0], eigenvalues[1],
					eigenvectors.GetColumn(0), eigenvectors.GetColumn(1), sModel.Eps);

				chart.AddSeries($"ellipse{i + 1}", ellipse, Color.Red);
			}

			return chart;
		}

		public static ChartForm Test3(DeterministicModel dModel, StochasticModel sModel)
		{
			const double step = 0.0000005;

			dModel.D21 = 0.0075;
			sModel.D21 = 0.0075;
			sModel.Eps = 0.1;
			sModel.Sigma1 = 0;
			sModel.Sigma2 = 0;
			sModel.Sigma3 = 1;

			IEnumerable<(double D12, double Mu1, double Mu2)> I1()
			{
				const double d12End = 0.001;
				var previous = new PointX(20, 40);

				for (var d12 = 0.000045; d12 < d12End; d12 += step)
				{
					dModel.D12 = d12;
					sModel.D12 = d12;
					var attractorPoints = PhaseTrajectory.Get(dModel, previous, 9999, 2);

					if (attractorPoints[0].IsInfinity())
						continue;

					if (!attractorPoints[0].AlmostEquals(attractorPoints[1]))
						yield break;

					previous = attractorPoints[1];
					var sensitivityMatrix = SensitivityMatrix.Get(sModel, previous);
					var mu = new EigenvalueDecomposition(sensitivityMatrix).RealEigenvalues;

					yield return (d12, Math.Min(mu[0], mu[1]), Math.Max(mu[0], mu[1]));
				}
			}

			IEnumerable<(double D12, double Mu1, double Mu2)> I2()
			{
				const double d12End = 0.0017;
				var previous = new PointX(20, 40);

				for (var d12 = 0.0025; d12 > d12End; d12 -= step)
				{
					dModel.D12 = d12;
					sModel.D12 = d12;
					var attractorPoints = PhaseTrajectory.Get(dModel, previous, 9999, 2);

					if (attractorPoints[0].IsInfinity())
						continue;

					if (!attractorPoints[0].AlmostEquals(attractorPoints[1]))
						yield break;

					previous = attractorPoints[1];
					var sensitivityMatrix = SensitivityMatrix.Get(sModel, previous);
					var mu = new EigenvalueDecomposition(sensitivityMatrix).RealEigenvalues;

					yield return (d12, Math.Min(mu[0], mu[1]), Math.Max(mu[0], mu[1]));
				}
			}

			IEnumerable<(double D12, double Mu1, double Mu2)> I3()
			{
				const double d12End = 0.0023;
				var previous = new PointX(34.5964044040563, 44.473609663403728);

				for (var d12 = 0.002166; d12 < d12End; d12 += step)
				{
					dModel.D12 = d12;
					sModel.D12 = d12;
					var attractorPoints = PhaseTrajectory.Get(dModel, previous, 9999, 2);

					if (attractorPoints[0].IsInfinity())
						continue;

					if (!attractorPoints[0].AlmostEquals(attractorPoints[1]))
						yield break;

					previous = attractorPoints[1];
					var sensitivityMatrix = SensitivityMatrix.Get(sModel, previous);
					var mu = new EigenvalueDecomposition(sensitivityMatrix).RealEigenvalues;

					yield return (d12, Math.Min(mu[0], mu[1]), Math.Max(mu[0], mu[1]));
				}
			}

			var points = I3().ToList();

			PointSaver.SaveToFile("I3_param.txt", points);

			var mu1 = points.Where(p => Math.Abs(p.Mu1) < 10000).Select(p => (p.D12, p.Mu1));
			var mu2 = points.Where(p => Math.Abs(p.Mu2) < 10000).Select(p => (p.D12, p.Mu2));

			var chart = new ChartForm(mu1, 0, 0.0025, 0, 50, name: "d12 v mu1");
			chart.AddSeries("d12 v mu2", mu2, Color.Orange);

			return chart;
		}

		public static ChartForm Test4(DeterministicModel dModel, StochasticModel sModel)
		{
			dModel.D12 = 0.001;
			dModel.D21 = 0.0075;
			sModel.D12 = 0.001;
			sModel.D21 = 0.0075;
			sModel.Eps = 0.1;
			sModel.Sigma1 = 1;
			sModel.Sigma2 = 1;
			sModel.Sigma3 = 0;

			var zik = PhaseTrajectory.Get(dModel, new PointX(20, 40), 10000, 2000);
			var chaosZik = PhaseTrajectory.Get(sModel, zik[0], 0, 2000);
			var (ellipse1, ellipse2) = ScatterEllipse.GetForZik(sModel, zik);

			var chart = new ChartForm(chaosZik, 0, 40, 0, 80);
			chart.AddSeries("zik", zik, Color.Black);
			chart.AddSeries("ellipse1", ellipse1.Where(p => Math.Abs(p.X1) < 100 && Math.Abs(p.X2) < 100), Color.Red);
			chart.AddSeries("ellipse2", ellipse2.Where(p => Math.Abs(p.X1) < 100 && Math.Abs(p.X2) < 100), Color.Red);
			//chart.AddSeries("test", new[]
			//{
			//	new PointX(14.5433643962272, 60.587772308652),
			//	new PointX(15.321157404287202, 64.95972934061189),
			//	new PointX(9.62065982040971, 61.463546057530976)
			//}, Color.DeepPink);

			return chart;
		}
	}
}
