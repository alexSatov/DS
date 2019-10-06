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
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;
            sModel.Eps = 0.01;
            sModel.D21 = 0.0075;
            dModel.D21 = 0.0075;

            const double step = 0.000002;

            IEnumerable<(double D12, double X1)> FirstAttractor()
            {
                dModel.D12 = 0.000045;
                sModel.D12 = 0.000045;
                return BifurcationDiagram.GetD12VsXByPrevious(dModel, sModel, new PointX(20, 40), 0.00245, step * 2)
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

            var points = FirstAttractor().ToList();

            //PointSaver.SaveToFile("d12_x1_3_stochastic.txt", points);

            return new ChartForm(points, 0, 0.00245, 0, 45);
        }

        public static ChartForm Test2(DeterministicModel dModel, StochasticModel sModel)
        {
            dModel.D12 = 0.00194;
            sModel.D12 = 0.00194;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.1;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;

            var attractorPoints = PhaseTrajectory.Get(dModel, new PointX(20, 40), 9999, 1);
            var points = PhaseTrajectory.Get(sModel, attractorPoints[0], 0, 500);
            var chart = new ChartForm(points, 0, 32, 32, 80);

            var sensitivityMatrices = SensitivityMatrix.Get(sModel, attractorPoints).ToList();

            PointSaver.SaveToFile("ellipse/attractor.txt", points);

            for (var i = 0; i < sensitivityMatrices.Count; i++)
            {
                var sensitivityMatrix = sensitivityMatrices[i];
                var eigenvalueDecomposition = new EigenvalueDecomposition(sensitivityMatrix);
                var eigenvalues = eigenvalueDecomposition.RealEigenvalues;
                Console.WriteLine(string.Join("; ", eigenvalues));
                var eigenvectors = eigenvalueDecomposition.Eigenvectors;
                var ellipse = ScatterEllipse.Get(attractorPoints[i], eigenvalues[0], eigenvalues[1],
                    eigenvectors.GetColumn(0), eigenvectors.GetColumn(1), sModel.Eps).ToList();

                chart.AddSeries($"ellipse{i + 1}", ellipse, Color.Red);
                PointSaver.SaveToFile($"ellipse/ellipse{i + 1}.txt", ellipse);
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

        /// <summary>
        /// Построение однокусочного зика
        /// </summary>
        public static ChartForm Test4(DeterministicModel dModel, StochasticModel sModel)
        {
            dModel.D12 = 0.00157;
            sModel.D12 = 0.00157;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.05;
            sModel.Sigma1 = 0;
            sModel.Sigma2 = 0;
            sModel.Sigma3 = 1;

            var zik = PhaseTrajectory.GetWhile(dModel, new PointX(15.3484299431058, 59.4141662230043), 10000, 0.0001);
            var chaosZik = PhaseTrajectory.Get(sModel, zik[0], 0, 2000);
            var (ellipse1, ellipse2) = ScatterEllipse.GetForZik2(dModel, sModel, zik);

            var chart = new ChartForm(chaosZik, 8, 24, 48, 80);
            chart.AddSeries("zik", zik, Color.Black);
            chart.AddSeries("ellipse1", ellipse1, Color.Red);
            chart.AddSeries("ellipse2", ellipse2, Color.Red);

            //PointSaver.SaveToFile("ellipse/attractor.txt", chaosZik);
            //PointSaver.SaveToFile("ellipse/ellipse1.txt", ellipse1);
            //PointSaver.SaveToFile("ellipse/ellipse2.txt", ellipse2);

            return chart;
        }

        /// <summary>
        /// Построение многокусочного зика
        /// </summary>
        public static ChartForm Test5(DeterministicModel dModel, StochasticModel sModel)
        {
            dModel.D12 = 0.0017;
            sModel.D12 = 0.0017;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.05;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;

            var zik = PhaseTrajectory.GetWhile(dModel, new PointX(20, 40), 10000, 0.0001);
            var zik1 = zik.Where((p, i) => i % 3 == 0).ToList();
            var zik2 = zik.Where((p, i) => i % 3 == 1).ToList();
            var zik3 = zik.Where((p, i) => i % 3 == 2).ToList();
            var chaosZik = PhaseTrajectory.Get(sModel, zik[0], 0, 6000);
            var (ellipse1, ellipse2) = ScatterEllipse.GetForZik2(dModel, sModel, zik);

            var chart = new ChartForm(chaosZik, 0, 30, 40, 80);
            chart.AddSeries("zik1", zik1, Color.Black);
            chart.AddSeries("zik2", zik2, Color.BlueViolet);
            chart.AddSeries("zik3", zik3, Color.Orange);
            chart.AddSeries("ellipse1", ellipse1, Color.Red);
            chart.AddSeries("ellipse2", ellipse2, Color.Red);

            //PointSaver.SaveToFile("ellipse/attractor.txt", chaosZik);
            //PointSaver.SaveToFile("ellipse/ellipse1.txt", ellipse1);
            //PointSaver.SaveToFile("ellipse/ellipse2.txt", ellipse2);

            return chart;
        }

        public static ChartForm Test6(DeterministicModel dModel, StochasticModel sModel)
        {
            const double step = 0.000005;
            var d12 = 0.000943;

            dModel.D12 = d12;
            dModel.D21 = 0.0075;
            sModel.D12 = d12;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.1;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;

            var result = new List<(double D12, double MuMax, double MuMin)>();
            var previous = new PointX(20, 40);

            for (; d12 < 0.001855; d12 += step)
            {
                dModel.D12 = d12;
                sModel.D12 = d12;
                var zik = PhaseTrajectory.Get(dModel, previous, 10000, 40000);
                previous = zik[zik.Count - 1];

                var mu = ScatterEllipse.GetMuForZik(sModel, zik);

                result.Add((d12, mu.Max(), mu.Min()));
            }

            var chart = new ChartForm(result.Select(p => (p.D12, p.MuMax)), 0, 0.0019, 0, 2000);
            chart.AddSeries("min", result.Select(p => (p.D12, p.MuMin)), Color.Orange);

            PointSaver.SaveToFile("zik_d12_mu_add.txt", result);

            return chart;
        }

        // (20, 40) -> [0.00145, 0.001682]; (22.060731438419, 58.3431519485857) -> [0.001909, 0.0019746]
        public static ChartForm Test7(DeterministicModel dModel, StochasticModel sModel)
        {
            const double step = 0.0000005;

            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.1;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;

            IEnumerable<(double D12, double Mu11, double Mu12, double Mu21, double Mu22, double Mu31, double Mu32)> I1()
            {
                const double d12End = 0.001682;
                var previous = new PointX(20, 40);

                for (var d12 = 0.00145; d12 < d12End; d12 += step)
                {
                    dModel.D12 = d12;
                    sModel.D12 = d12;

                    var attractorPoints = PhaseTrajectory.Get(dModel, previous, 9999, 3);

                    previous = attractorPoints[2];

                    var sensitivityMatrices = SensitivityMatrix.Get(sModel, attractorPoints).ToList();
                    var mu1 = new EigenvalueDecomposition(sensitivityMatrices[0]).RealEigenvalues;
                    var mu2 = new EigenvalueDecomposition(sensitivityMatrices[1]).RealEigenvalues;
                    var mu3 = new EigenvalueDecomposition(sensitivityMatrices[2]).RealEigenvalues;

                    yield return (d12,
                        Math.Min(mu1[0], mu1[1]), Math.Max(mu1[0], mu1[1]),
                        Math.Min(mu2[0], mu2[1]), Math.Max(mu2[0], mu2[1]),
                        Math.Min(mu3[0], mu3[1]), Math.Max(mu3[0], mu3[1]));
                }
            }

            IEnumerable<(double D12, double Mu11, double Mu12, double Mu21, double Mu22, double Mu31, double Mu32)> I2()
            {
                const double d12End = 0.0019746;
                var previous = new PointX(22.060731438419, 58.3431519485857);

                for (var d12 = 0.001909; d12 < d12End; d12 += step)
                {
                    dModel.D12 = d12;
                    sModel.D12 = d12;

                    var attractorPoints = PhaseTrajectory.Get(dModel, previous, 9999, 3);

                    previous = attractorPoints[2];

                    var sensitivityMatrices = SensitivityMatrix.Get(sModel, attractorPoints).ToList();
                    var mu1 = new EigenvalueDecomposition(sensitivityMatrices[0]).RealEigenvalues;
                    var mu2 = new EigenvalueDecomposition(sensitivityMatrices[1]).RealEigenvalues;
                    var mu3 = new EigenvalueDecomposition(sensitivityMatrices[2]).RealEigenvalues;

                    yield return (d12,
                        Math.Min(mu1[0], mu1[1]), Math.Max(mu1[0], mu1[1]),
                        Math.Min(mu2[0], mu2[1]), Math.Max(mu2[0], mu2[1]),
                        Math.Min(mu3[0], mu3[1]), Math.Max(mu3[0], mu3[1]));
                }
            }

            var points = I2().ToList();

            PointSaver.SaveToFile("I2.txt", points);

            var mu11 = points.Select(p => (p.D12, p.Mu11));
            var mu12 = points.Select(p => (p.D12, p.Mu12));
            var mu21 = points.Select(p => (p.D12, p.Mu21));
            var mu22 = points.Select(p => (p.D12, p.Mu22));
            var mu31 = points.Select(p => (p.D12, p.Mu31));
            var mu32 = points.Select(p => (p.D12, p.Mu32));

            var chart = new ChartForm(mu11, 0, 0.0025, 0, 50, name: "d12 v mu11");
            chart.AddSeries("d12 v mu12", mu12, Color.LightBlue);
            chart.AddSeries("d12 v mu21", mu21, Color.Red);
            chart.AddSeries("d12 v mu22", mu22, Color.DarkRed);
            chart.AddSeries("d12 v mu31", mu31, Color.Orange);
            chart.AddSeries("d12 v mu32", mu32, Color.OrangeRed);

            return chart;
        }
    }
}
