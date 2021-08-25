using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Accord.Math;
using Accord.Math.Decompositions;
using DS.Extensions;
using DS.Helpers;
using DS.MathStructures.Points;
using DS.Models;
using NUnit.Framework;

namespace DS.Tests.Charts.Model1
{
    /// <summary>
    /// 1, 1, 0 - аддитивный шум
    /// 0, 0, 1 - параметрический шум
    /// </summary>
    public class StochasticModel1Tests : ChartTests
    {
        private DeterministicModel1 dModel;
        private StochasticModel1 sModel;

        protected override void OnSetUp()
        {
            dModel = new DeterministicModel1(0.0002, 0.00052, 10, 20, 0.25, 1);
            sModel = new StochasticModel1(0.0002, 0.00052, 10, 20, 0.25, 1);
        }

        [Test]
        public void Test1()
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

            Chart = new ChartForm(points, 0, 0.00245, 0, 45);
        }

        [Test]
        public void Test2()
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

            // PointSaver.SaveToFile("ellipse/attractor.txt", points);

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
                // PointSaver.SaveToFile($"ellipse/ellipse{i + 1}.txt", ellipse);
            }

            Chart = chart;
        }

        [Test]
        public void Test3()
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

            // PointSaver.SaveToFile("I3_param.txt", points);

            var mu1 = points.Where(p => Math.Abs(p.Mu1) < 10000).Select(p => (p.D12, p.Mu1));
            var mu2 = points.Where(p => Math.Abs(p.Mu2) < 10000).Select(p => (p.D12, p.Mu2));

            var chart = new ChartForm(mu1, 0, 0.0025, 0, 50, name: "d12 v mu1");
            chart.AddSeries("d12 v mu2", mu2, Color.Orange);

            Chart = chart;
        }

        /// <summary>
        /// Построение однокусочного зика [с бассейном]
        /// </summary>
        [Test]
        public void Test4()
        {
            dModel.D12 = 0.00157;
            sModel.D12 = 0.00157;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.1;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;

            var zik = PhaseTrajectory.GetWhileNotConnect(dModel, new PointX(12, 60), 10000, 0.0001);
            var chaosZik = PhaseTrajectory.Get(sModel, zik[0], 0, 2000);
            var (ellipse1, ellipse2) = ScatterEllipse.GetForZik2(dModel, sModel, zik);

            var chart = new ChartForm(chaosZik, 10, 22, 52, 75);
            chart.AddSeries("zik", zik, Color.Black);
            chart.AddSeries("ellipse1", ellipse1, Color.Red);
            //chart.AddSeries("ellipse2", ellipse2, Color.Red);

            var pool = AttractorPool.GetPoolFor2AttractorsParallel(dModel, AttractorExtensions.Is3Cycle, new PointX(10, 52),
                new PointX(22, 75), 0.25, 0.4);

            chart.AddSeries("pool_first", pool.First, Color.HotPink);
            chart.AddSeries("pool_second", pool.Second, Color.Green);

            //PointSaver.SaveToFile("ellipse/attractor.txt", chaosZik);
            //PointSaver.SaveToFile("ellipse/ellipse1.txt", ellipse1);
            //PointSaver.SaveToFile("ellipse/ellipse2.txt", ellipse2);

            Chart = chart;
        }

        /// <summary>
        /// Построение многокусочного зика
        /// </summary>
        [Test]
        public void Test5()
        {
            dModel.D12 = 0.00186;
            sModel.D12 = 0.00186;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.005;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;

            var zik = PhaseTrajectory.GetWhileNotConnect(dModel, new PointX(20, 40), 10000, 0.0001);
            var zik1 = zik.Where((p, i) => i % 3 == 0).ToList();
            var zik2 = zik.Where((p, i) => i % 3 == 1).ToList();
            var zik3 = zik.Where((p, i) => i % 3 == 2).ToList();
            var chaosZik = PhaseTrajectory.Get(sModel, zik[0], 0, 60000);
            var (ellipse1, ellipse2) = ScatterEllipse.GetForZik2(dModel, sModel, zik);

            var chart = new ChartForm(chaosZik, 7, 30, 50, 80);
            chart.AddSeries("zik1", zik1, Color.Black);
            chart.AddSeries("zik2", zik2, Color.BlueViolet);
            chart.AddSeries("zik3", zik3, Color.Orange);
            chart.AddSeries("ellipse1", ellipse1, Color.Red);
            chart.AddSeries("ellipse2", ellipse2, Color.Red);

            // PointSaver.SaveToFile("ellipse/chaosZik.txt", chaosZik);
            // PointSaver.SaveToFile("ellipse/zik1.txt", zik1);
            // PointSaver.SaveToFile("ellipse/zik2.txt", zik2);
            // PointSaver.SaveToFile("ellipse/zik3.txt", zik3);
            // PointSaver.SaveToFile("ellipse/ellipse1.txt", ellipse1);
            // PointSaver.SaveToFile("ellipse/ellipse2.txt", ellipse2);

            Chart = chart;
        }

        [Test]
        public void Test6()
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
                previous = zik[^1];

                var mu = ScatterEllipse.GetMuForZik(sModel, zik);

                result.Add((d12, mu.Max(), mu.Min()));
            }

            var chart = new ChartForm(result.Select(p => (p.D12, p.MuMax)), 0, 0.0019, 0, 2000);
            chart.AddSeries("min", result.Select(p => (p.D12, p.MuMin)), Color.Orange);

            // PointSaver.SaveToFile("zik_d12_mu_add.txt", result);

            Chart = chart;
        }

        /// <summary>
        /// (20, 40) -> [0.00145, 0.001682]; (22.060731438419, 58.3431519485857) -> [0.001909, 0.0019746]
        /// </summary>
        [Test]
        public void Test7()
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

            // PointSaver.SaveToFile("I2.txt", points);

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

            Chart = chart;
        }

        /// <summary>
        /// Крит. интенсивность. 1 зона: ЗИК - 3цикл, D12 in (0.00145, 0.001682)
        /// </summary>
        [Test]
        public void Test8_1()
        {
            const double step = 0.0000023; // 100 итераций

            SetModels();

            (double D12, double Eps) SearchLR(double d12)
            {
                var zik = new List<PointX> { new PointX(12, 60) };
                var dInnerModel = (DeterministicModel1) dModel.Copy();
                var sInnerModel = (StochasticModel1) sModel.Copy();
                dInnerModel.D12 = d12;
                sInnerModel.D12 = d12;

                zik = PhaseTrajectory.GetWhileNotConnect(dInnerModel, zik[^1], 10000, 0.0001);

                if (!ValidateZik(d12, zik))
                {
                    Console.WriteLine($"zik not build on d12: {d12}\r\n");
                    return default;
                }

                for (var eps = 0.01; eps < 2; eps += 0.01)
                {
                    sInnerModel.Eps = eps;
                    var (ellipse, _) = ScatterEllipse.GetForZik2(dInnerModel, sInnerModel, zik);

                    foreach (var ellipsePoint in ellipse)
                    {
                        var cycle3 = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 9996, 4);

                        if (!AttractorExtensions.Is3Cycle(cycle3))
                            continue;

                        Console.WriteLine($"zik: {zik.Take(4).Format()};\r\n" +
                            $"3-cycle: {cycle3.Format()};\r\n" +
                            $"d12: {d12.Format()}, eps: {eps.Format()}\r\n");

                        return (d12, eps);
                    }
                }

                Console.WriteLine($"3-cycle not found with max eps on d12: {d12}\r\n");

                return default;
            }

            (double D12, double Eps) SearchRL(double d12)
            {
                var cycle3 = new List<PointX> { new PointX(20, 40) };
                var dInnerModel = (DeterministicModel1) dModel.Copy();
                var sInnerModel = (StochasticModel1) sModel.Copy();
                dInnerModel.D12 = d12;
                sInnerModel.D12 = d12;

                cycle3 = PhaseTrajectory.Get(dInnerModel, cycle3[^1], 9996, 4);

                if (!AttractorExtensions.Is3Cycle(cycle3))
                {
                    Console.WriteLine($"Error: 3-cycle not build on d12: {d12}\r\n" +
                        $"result: {cycle3.Format()};\r\n");
                    return default;
                }

                cycle3.RemoveAt(3);

                for (var eps = 0.01; eps < 2; eps += 0.01)
                {
                    sInnerModel.Eps = eps;
                    var ellipse = GetEllipses(sInnerModel, cycle3).SelectMany(l => l);

                    foreach (var ellipsePoint in ellipse)
                    {
                        var zik = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 9996, 4);

                        if (AttractorExtensions.Is3Cycle(zik))
                            continue;

                        Console.WriteLine($"3-cycle: {cycle3.Format()};\r\n" +
                            $"zik: {zik.Format()};\r\n" +
                            $"d12: {d12.Format()}, eps: {eps.Format()}\r\n");

                        return (d12, eps);
                    }
                }

                Console.WriteLine($"zik not found with max eps on d12: {d12}\r\n");

                return default;
            }

            var (points, chart) = Test8_Parallel(0.00145, 0.001682, step, SearchLR);

            // PointSaver.SaveToFile("crit_intens\\zone1_1.txt", points);

            Chart = chart;
        }

        /// <summary>
        /// Крит. интенсивность. 2 зона: равновесие - 3цикл, D12 in (0.001909, 0.001974)
        /// </summary>
        [Test]
        public void Test8_2()
        {
            const double step = 0.000001; // 65 итераций

            SetModels();

            (double D12, double Eps) SearchLR(double d12)
            {
                var eq = new List<PointX> { new PointX(18, 65) };
                var dInnerModel = (DeterministicModel1) dModel.Copy();
                var sInnerModel = (StochasticModel1) sModel.Copy();
                dInnerModel.D12 = d12;
                sInnerModel.D12 = d12;

                eq = PhaseTrajectory.Get(dInnerModel, eq[^1], 9996, 4);

                if (!AttractorExtensions.IsEquilibrium(eq))
                {
                    Console.WriteLine($"Error: eq not build on d12: {d12}\r\n" +
                        $"result: {eq.Format()};\r\n");
                    return default;
                }

                for (var eps = 0.01; eps < 2; eps += 0.01)
                {
                    sInnerModel.Eps = eps;
                    var ellipse = GetEllipse(sInnerModel, eq[^1]);

                    foreach (var ellipsePoint in ellipse)
                    {
                        var skipCount = d12 == 0.001909 ? 299996 : 9996;
                        var cycle3 = PhaseTrajectory.Get(dInnerModel, ellipsePoint, skipCount, 4);

                        if (!AttractorExtensions.Is3Cycle(cycle3))
                            continue;

                        Console.WriteLine($"eq: {eq.Format()};\r\n" +
                            $"3-cycle: {cycle3.Format()};\r\n" +
                            $"d12: {d12.Format()}, eps: {eps.Format()}\r\n");

                        return (d12, eps);
                    }
                }

                Console.WriteLine($"3-cycle not found with max eps on d12: {d12.Format()}\r\n");

                return default;
            }

            (double D12, double Eps) SearchRL(double d12)
            {
                var cycle3 = new List<PointX> { new PointX(21, 61) };
                var dInnerModel = (DeterministicModel1) dModel.Copy();
                var sInnerModel = (StochasticModel1) sModel.Copy();
                dInnerModel.D12 = d12;
                sInnerModel.D12 = d12;

                cycle3 = d12 == 0.001909
                    ? PhaseTrajectory.Get(dInnerModel, cycle3[^1], 199996, 4)
                    : PhaseTrajectory.Get(dInnerModel, cycle3[^1], 9996, 4);

                if (!AttractorExtensions.Is3Cycle(cycle3))
                {
                    Console.WriteLine($"Error: 3-cycle not build on d12: {d12}\r\n" +
                        $"result: {cycle3.Format()};\r\n");
                    return default;
                }

                cycle3.RemoveAt(3);

                for (var eps = 0.01; eps < 0.4; eps += 0.01)
                {
                    sInnerModel.Eps = eps;
                    var ellipse = GetEllipses(sInnerModel, cycle3).SelectMany(l => l);

                    foreach (var ellipsePoint in ellipse)
                    {
                        var eq = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 9996, 4);

                        if (!AttractorExtensions.IsEquilibrium(eq))
                            continue;

                        Console.WriteLine($"3-cycle: {cycle3.Format()};\r\n" +
                            $"eq: {eq.Format()};\r\n" +
                            $"d12: {d12.Format()}, eps: {eps.Format()}\r\n");

                        return (d12, eps);
                    }
                }

                Console.WriteLine($"eq not found with max eps on d12: {d12.Format()}\r\n");

                return default;
            }

            var (points, chart) = Test8_Parallel(0.001909, 0.001974, step, SearchRL);

            // PointSaver.SaveToFile("crit_intens\\zone2_2.txt", points);

            Chart = chart;
        }

        /// <summary>
        /// Крит. интенсивность. 3 зона: равновесие - равновесие, D12 in (0.002166, 0.002294)
        /// </summary>
        [Test]
        public void Test8_3()
        {
            const double step = 0.0000012; // 106 итераций

            SetModels();

            (double D12, double Eps) SearchLR(double d12)
            {
                var eq1 = new List<PointX> { new PointX(18, 68) };
                var dInnerModel = (DeterministicModel1) dModel.Copy();
                var sInnerModel = (StochasticModel1) sModel.Copy();
                dInnerModel.D12 = d12;
                sInnerModel.D12 = d12;

                eq1 = PhaseTrajectory.Get(dInnerModel, eq1[0], 9998, 2);

                if (!AttractorExtensions.IsEquilibrium(eq1))
                {
                    Console.WriteLine($"Error: eq1 not build on d12: {d12}\r\n" +
                        $"result: {eq1.Format()};\r\n");
                    return default;
                }

                for (var eps = 0.1; eps < 2; eps += 0.01)
                {
                    sInnerModel.Eps = eps;
                    var ellipse = GetEllipse(sInnerModel, eq1[^1]);

                    foreach (var ellipsePoint in ellipse)
                    {
                        var eq2 = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 9998, 2);

                        if (eq1[^1].AlmostEquals(eq2[^1]))
                            continue;

                        Console.WriteLine($"eq1: {eq1.Format()};\r\n" +
                            $"eq2: {eq2.Format()};\r\n" +
                            $"d12: {d12.Format()}, eps: {eps.Format()}\r\n");

                        return (d12, eps);
                    }
                }

                Console.WriteLine($"eq2 not found with max eps on d12: {d12.Format()}\r\n");

                return default;
            }

            (double D12, double Eps) SearchRL(double d12)
            {
                var eq2 = new List<PointX> { new PointX(35.1513396288763, 42.157142188389) };
                var dInnerModel = (DeterministicModel1) dModel.Copy();
                var sInnerModel = (StochasticModel1) sModel.Copy();
                dInnerModel.D12 = d12;
                sInnerModel.D12 = d12;

                eq2 = PhaseTrajectory.Get(dInnerModel, eq2[0], 9998, 2);

                if (!AttractorExtensions.IsEquilibrium(eq2))
                {
                    Console.WriteLine($"Error: eq2 not build on d12: {d12}\r\n" +
                        $"result: {eq2.Format()};\r\n");
                    return default;
                }

                for (var eps = 0.01; eps < 2; eps += 0.01)
                {
                    sInnerModel.Eps = eps;
                    var ellipse = GetEllipse(sInnerModel, eq2[^1]);

                    foreach (var ellipsePoint in ellipse)
                    {
                        var eq1 = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 9998, 2);

                        if (eq2[^1].AlmostEquals(eq1[^1]))
                            continue;

                        Console.WriteLine($"eq2: {eq2.Format()};\r\n" +
                            $"eq1: {eq1.Format()};\r\n" +
                            $"d12: {d12.Format()}, eps: {eps.Format()}\r\n");

                        return (d12, eps);
                    }
                }

                Console.WriteLine($"eq1 not found with max eps on d12: {d12.Format()}\r\n");

                return default;
            }

            var (points, chart) = Test8_Parallel(0.002166, 0.002294, step, SearchRL);

            // PointSaver.SaveToFile("crit_intens\\zone3_2.txt", points);

            Chart = chart;
        }

        /// <summary>
        /// Крит. интенсивность. 4 зона: ЗИК - 3ЗИК, D12 in (0.001682, 0.00173)
        /// </summary>
        [Test]
        public void Test8_4()
        {
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;

            (PointX Eq1, PointX Eq2, IEnumerable<PointX> Chaos, IEnumerable<PointX> Ellipse) Search()
            {
                for (var d12 = 0.001682; d12 < 0.00173; d12 += 0.000001)
                {
                    for (var eps = 0.1; eps < 1; eps += 0.1)
                    {
                        dModel.D12 = d12;
                        sModel.D12 = d12;
                        sModel.Eps = eps;

                        var eq = PhaseTrajectory.Get(dModel, new PointX(20, 40), 9999, 1).First();
                        //var points = PhaseTrajectory.Get(sModel, eq, 0, 500);
                        var sensitivityMatrix = SensitivityMatrix.Get(sModel, eq);
                        var eigenvalueDecomposition = new EigenvalueDecomposition(sensitivityMatrix);
                        var eigenvalues = eigenvalueDecomposition.RealEigenvalues;
                        var eigenvectors = eigenvalueDecomposition.Eigenvectors;
                        var ellipse = ScatterEllipse.Get(eq, eigenvalues[0], eigenvalues[1],
                            eigenvectors.GetColumn(0), eigenvectors.GetColumn(1), sModel.Eps).ToList();

                        foreach (var ellipsePoint in ellipse)
                        {
                            var otherEq = PhaseTrajectory.Get(dModel, ellipsePoint, 9999, 1).First();
                            if (!eq.AlmostEquals(otherEq))
                                return (eq, otherEq, PhaseTrajectory.Get(sModel, eq, 0, 500), ellipse);
                        }
                    }
                }

                return (new PointX(0, 0), new PointX(0, 0), new List<PointX>(), new List<PointX>());
            }

            var (eq1, eq2, chaos, _ellipse) = Search();

            var chart = new ChartForm(chaos, 0, 40, 0, 80);
            chart.AddSeries("attractor1", new List<PointX> { eq1 }, Color.Black, markerSize: 8);
            chart.AddSeries("attractor2", new List<PointX> { eq2 }, Color.Blue, markerSize: 8);
            chart.AddSeries("ellipse", _ellipse, Color.Red);

            Chart = chart;
        }

        /// <summary>
        /// Крит. интенсивность. 5 зона: равновесие - 3ЗИК, D12 in (0.001855, 0.001909)
        /// </summary>
        [Test]
        public void Test8_5()
        {
            SetModels();

            IList<(double D12, double Eps)> Search(DeterministicModel1 dInnerModel, StochasticModel1 sInnerModel,
                double d12Start, double d12End)
            {
                const double step = 0.000001;
                var result = new List<(double D12, double Eps)>();
                var attractor = new List<PointX> { new PointX(16, 67) };
                //var attractor = new List<PointX> { new PointX(20, 40) };

                for (var d12 = d12Start; d12 < d12End; d12 += step)
                {
                    dInnerModel.D12 = d12;
                    sInnerModel.D12 = d12;
                    attractor = PhaseTrajectory.Get(dInnerModel, attractor[attractor.Count - 1], 19998, 2);
                    //attractor = PhaseTrajectory.GetWhileNotConnect(dInnerModel, attractor[attractor.Count - 1], 5000, 0.0001);

                    //if (!ValidateZik(d12, attractor))
                    //    continue;

                    if (!AttractorExtensions.IsEquilibrium(attractor))
                    {
                        Console.WriteLine($"Error: eq not build on d12 = {d12}");
                        continue;
                    }

                    for (var eps = 0.1; eps < 1.5; eps += 0.1)
                    {
                        sInnerModel.Eps = eps;
                        var found = false;
                        //var (ellipse, _) = ScatterEllipse.GetForZik2(dInnerModel, sInnerModel, attractor);
                        var ellipse = GetEllipse(sInnerModel, attractor[1]);

                        foreach (var ellipsePoint in ellipse)
                        {
                            var otherAttractor = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 19998, 2);

                            if (attractor[1].AlmostEquals(otherAttractor[1]))
                                continue;

                            found = true;

                            Console.WriteLine($"{string.Join(", ", attractor)};\r\n" +
                                $"{string.Join(", ", otherAttractor)}; {d12}");

                            result.Add((d12, eps));

                            break;
                        }

                        if (found)
                            break;
                    }
                }

                return result;
            }

            var (points, chart) = Test8_Parallel_Old(dModel, sModel, 0.001855, 0.001909, Search);

            // PointSaver.SaveToFile("crit_intens\\zone5_1.txt", points);

            Chart = chart;
        }

        /// <summary>
        /// Построение критических линий (для хаоса d12 = 0.00237) и эллипса рассеивания вокруг границы.
        /// </summary>
        [Test]
        public void Test9()
        {
            const double eps = 0.05, d12 = 0.00237, d21 = 0.0075;

            dModel.D12 = d12;
            dModel.D21 = d21;

            sModel.D12 = d12;
            sModel.D21 = d21;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Eps = eps;

            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(31, 64), 50000, 50000);
            var attractor2 = PhaseTrajectory.Get(sModel, attractor[^1], 0, 50000);
            var lcSet = LcSet.FromAttractor(dModel, attractor, 9);
            var borderSegments = lcSet.GetBorderSegments();
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, eps).ToList();

            Chart = new ChartForm(attractor2, 31, 40, 22, 54);

            foreach (var borderSegment in borderSegments)
                Chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red, SeriesChartType.FastLine, 5, 3);

            Chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, markerSize: 5);

            // attractor.SaveToFile("model1_chaos1\\chaos.txt");
            // attractor2.SaveToFile("model1_chaos1\\chaos_with_noise.txt");
            // lcSet.SaveToFile("model1_chaos1\\LC.txt");
            // lcSet.SaveToFile(ellipse, "model1_chaos1\\LC_border.txt");
            // ellipse.SaveToFile("model1_chaos1\\ellipse.txt");
        }

        /// <summary>
        /// Построение критических линий (для хаоса d12 = 0.002538, d21 = 0.0055 - сложный)
        /// и эллипса рассеивания вокруг границы.
        /// </summary>
        [Test]
        public void Test10()
        {
            const double eps = 0.05, d12 = 0.002538, d21 = 0.0055;

            dModel.D12 = d12;
            dModel.D21 = d21;

            sModel.D12 = d12;
            sModel.D21 = d21;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Eps = eps;

            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(36, 40), 50000, 50000);
            var attractor2 = PhaseTrajectory.Get(sModel, attractor[^1], 0, 50000);
            var lcSet = LcSet.FromAttractor(dModel, attractor, 5, 95);
            var borderSegments = lcSet.GetBorderSegments()
                .Where(s => s.Start.X1 > 24 || s.End.X1 > 24)
                .Concat(lcSet.GetBorderSegments2().Where(s => s.Start.X1 <= 24 && s.End.X1 <= 24));
            // var borderSegments = lcSet.GetBorderSegments2();
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, eps).ToList();

            Chart = new ChartForm(attractor2, 10, 44, 0, 64);

            foreach (var borderSegment in borderSegments)
                Chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.FastLine, 5, 3);

            Chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, markerSize: 5);

            // attractor.SaveToFile("model1_chaos3\\chaos.txt");
            // attractor2.SaveToFile("model1_chaos3\\chaos_with_noise.txt");
            // lcSet.SaveToFile("model1_chaos3\\LC.txt");
            // lcSet.SaveToFile(ellipse, "model1_chaos3\\LC_border.txt");
            // ellipse.SaveToFile("model1_chaos3\\ellipse.txt");
        }

        /// <summary>
        /// Построение критических линий (для хаоса d12 = 0.0024, d21 = 0.0055)
        /// и эллипса рассеивания вокруг границы.
        /// </summary>
        [Test]
        public void Test11()
        {
            const double eps = 0.05, d12 = 0.0024, d21 = 0.0055;

            dModel.D12 = d12;
            dModel.D21 = d21;

            sModel.D12 = d12;
            sModel.D21 = d21;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Eps = eps;

            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(36, 40), 50000, 50000);
            //var attractor2 = PhaseTrajectory.Get(sModel, attractor[^1], 0, 50000);
            var lcSet = LcSet.FromAttractor(dModel, attractor, 8);
            var borderSegments = lcSet.GetBorderSegments();
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, eps);

            var chart = new ChartForm(attractor, 27, 40, 14, 54);

            //foreach (var lc in lcSet[LcType.H])
            //    chart.AddSeries($"lc_{i++}", lc, Color.Red, 5, SeriesChartType.Line);

            foreach (var borderSegment in borderSegments)
                chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red, SeriesChartType.FastLine);

            chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, SeriesChartType.Point);

            Chart = chart;
        }

        /// <summary>
        /// Построение критических линий (для хаоса d12 = d21 = 0 - квадрат)
        /// и эллипса рассеивания вокруг границы.
        /// </summary>
        [Test]
        public void Test12()
        {
            const double eps = 0.1, d12 = 0, d21 = 0;

            dModel.D12 = d12;
            dModel.D21 = d21;
            dModel.A1 = 0.00925;
            dModel.A2 = 0.002324;

            sModel.D12 = d12;
            sModel.D21 = d21;
            sModel.A1 = 0.00925;
            sModel.A2 = 0.002324;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Eps = eps;

            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(36, 40), 50000, 50000);
            var attractor2 = PhaseTrajectory.Get(sModel, attractor[^1], 0, 50000);
            var lcSet = LcSet.FromAttractor(dModel, attractor, 3);
            var borderSegments = lcSet.GetBorderSegments();
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, eps, kq:1);

            var chart = new ChartForm(attractor2, 0, 40, 0, 80);
            //chart.AddSeries("0H", lcSet[LcType.H][0], Color.Red, SeriesChartType.Line, 4);
            //chart.AddSeries("0V", lcSet[LcType.V][0], Color.Red, SeriesChartType.Line, 4);
            //chart.AddSeries("1H", lcSet[LcType.H][1], Color.Purple, SeriesChartType.Line, 4);
            //chart.AddSeries("1V", lcSet[LcType.V][1], Color.Purple, SeriesChartType.Line, 4);
            //chart.AddSeries("2H", lcSet[LcType.H][2], Color.Green, SeriesChartType.Line, 4);
            //chart.AddSeries("2V", lcSet[LcType.V][2], Color.Green, SeriesChartType.Line, 4);
            ////chart.AddSeries("3H", lcSet[LcType.H][3], Color.Black, SeriesChartType.Line, 4);
            ////chart.AddSeries("3V", lcSet[LcType.V][3], Color.Black, SeriesChartType.Line, 4);

            foreach (var borderSegment in borderSegments)
                chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.Line, 4);

            chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, SeriesChartType.Point, 4);

            Chart = chart;
        }

        /// <summary>
        /// Построение критических линий (для хаоса d12 = 0.001814, d21 = 0.00785312 - птичка)
        /// и эллипса рассеивания вокруг границы.
        /// </summary>
        [Test]
        public void Test13()
        {
            const double eps = 0.1, d12 = 0.001814, d21 = 0.00785312;

            dModel.D12 = d12;
            dModel.D21 = d21;

            sModel.D12 = d12;
            sModel.D21 = d21;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Eps = eps;

            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(20, 40), 50000, 100000);
            // var attractor2 = PhaseTrajectory.Get(sModel, attractor[^1], 0, 100000);
            var lcSet = LcSet.FromAttractor(dModel, attractor, 7);
            var borderSegments = lcSet.GetBorderSegments();
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, eps).ToList();

            Chart = new ChartForm(attractor, 0, 31, 20, 81);
            // Chart.AddSeries("0H", lcSet[LcType.H][0], Color.Red, SeriesChartType.Line, 4);
            // Chart.AddSeries("0V", lcSet[LcType.V][0], Color.Red, SeriesChartType.Line, 4);
            // Chart.AddSeries("1H", lcSet[LcType.H][1], Color.Purple, SeriesChartType.Line, 4);
            // Chart.AddSeries("1V", lcSet[LcType.V][1], Color.Purple, SeriesChartType.Line, 4);
            // Chart.AddSeries("2H", lcSet[LcType.H][2], Color.Green, SeriesChartType.Line, 4);
            // Chart.AddSeries("2V", lcSet[LcType.V][2], Color.Green, SeriesChartType.Line, 4);
            // Chart.AddSeries("3H", lcSet[LcType.H][3], Color.Black, SeriesChartType.Line, 4);
            // Chart.AddSeries("3V", lcSet[LcType.V][3], Color.Black, SeriesChartType.Line, 4);
            // Chart.AddSeries("4H", lcSet[LcType.H][4], Color.SaddleBrown, SeriesChartType.Line, 4);
            // Chart.AddSeries("4V", lcSet[LcType.V][4], Color.SaddleBrown, SeriesChartType.Line, 4);
            // Chart.AddSeries("5H", lcSet[LcType.H][5], Color.DarkOrange, SeriesChartType.Line, 4);
            // Chart.AddSeries("5V", lcSet[LcType.V][5], Color.DarkOrange, SeriesChartType.Line, 4);
            // Chart.AddSeries("6H", lcSet[LcType.H][6], Color.Olive, SeriesChartType.Line, 4);
            // Chart.AddSeries("6V", lcSet[LcType.V][6], Color.Olive, SeriesChartType.Line, 4);
            // Chart.AddSeries("7H", lcSet[LcType.H][7], Color.Gold, SeriesChartType.Line, 4);
            // Chart.AddSeries("7V", lcSet[LcType.V][7], Color.Gold, SeriesChartType.Line, 4);

            foreach (var borderSegment in borderSegments)
                Chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.FastLine, borderWidth: 3);

            Chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, markerSize: 4);

            // attractor.SaveToFile("model1_chaos2\\chaos.txt");
            // attractor2.SaveToFile("model1_chaos2\\chaos_with_noise.txt");
            // lcSet.SaveToFile("model1_chaos2\\LC.txt");
            // lcSet.SaveToFile(ellipse, "model1_chaos2\\LC_border.txt");
            // ellipse.SaveToFile("model1_chaos2\\ellipse.txt");
        }

        /// <summary>
        /// Построение критических линий
        /// и доверенной полосы вокруг стохастического аттрактора.
        /// </summary>
        [Test]
        public void Test14_1()
        {
            const double eps = 1, d12 = 0.00222, d21 = 0.0073;

            dModel.D12 = d12;
            dModel.D21 = d21;

            sModel.D12 = d12;
            sModel.D21 = d21;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Eps = eps;

            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(35.7819, 38.6575), 5000, 10000);
            var attractor2 = PhaseTrajectory.Get(sModel, attractor[^1], 0, 100000);
            var lc0H = dModel.GetLc0H(11.35, 35.84);
            var lc0V = dModel.GetLc0V(29.22, 67.39);
            var lcSet = LcSet.FromAttractor(dModel, attractor2, 2, lc0H: lc0H, lc0V: lc0V);
            var borderSegments = lcSet.GetBorderSegments();
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, eps).ToList();

            Chart = new ChartForm(attractor2, 0, 40, 0, 80);
            // Chart.AddSeries("0H", lcSet[LcType.H][0], Color.Red, SeriesChartType.Line, 4);
            // Chart.AddSeries("0V", lcSet[LcType.V][0], Color.Red, SeriesChartType.Line, 4);
            // Chart.AddSeries("1H", lcSet[LcType.H][1], Color.Purple, SeriesChartType.Line, 4);
            // Chart.AddSeries("1V", lcSet[LcType.V][1], Color.Purple, SeriesChartType.Line, 4);
            // Chart.AddSeries("2H", lcSet[LcType.H][2], Color.Green, SeriesChartType.Line, 4);
            // Chart.AddSeries("2V", lcSet[LcType.V][2], Color.Green, SeriesChartType.Line, 4);

            foreach (var borderSegment in borderSegments)
                Chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.FastLine, borderWidth: 3);

            Chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, markerSize: 4);

            // attractor2.SaveToFile("eq_with_noise.txt", "model1_lc1");
            // lcSet.SaveToFile("lc.txt", "model1_lc1");
            // ellipse.SaveToFile("band.txt", "model1_lc1");
            // lcSet.SaveToFile(ellipse, "model1_chaos2\\LC_border.txt");
        }

        /// <summary>
        /// Построение критических линий
        /// и доверенной полосы вокруг стохастического аттрактора.
        /// </summary>
        [Test]
        public void Test14_2()
        {
            const double eps = 0.6, d12 = 0.002271, d21 = 0.0073;

            dModel.D12 = d12;
            dModel.D21 = d21;

            sModel.D12 = d12;
            sModel.D21 = d21;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Eps = eps;

            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(36.9259, 66.6076), 5000, 10000);
            var attractor2 = PhaseTrajectory.Get(sModel, attractor[^1], 0, 100000);

            // 0.1
            // var lc0H = dModel.GetLc0H(12.08, 27.42);
            // var lc0V = dModel.GetLc0V(25.20, 52.68);
            // 0.6
            var lc0H = dModel.GetLc0H(12.08, 36.64);
            var lc0V = dModel.GetLc0V(25.20, 67.22);

            var lcSet = LcSet.FromAttractor(dModel, attractor2, 2, lc0H: lc0H, lc0V: lc0V);
            var borderSegments = lcSet.GetBorderSegments();
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, eps).ToList();

            Chart = new ChartForm(attractor2, 0, 40, 0, 80);
            // Chart.AddSeries("0H", lcSet[LcType.H][0], Color.Red, SeriesChartType.Line, 4);
            // Chart.AddSeries("0V", lcSet[LcType.V][0], Color.Red, SeriesChartType.Line, 4);
            // Chart.AddSeries("1H", lcSet[LcType.H][1], Color.Purple, SeriesChartType.Line, 4);
            // Chart.AddSeries("1V", lcSet[LcType.V][1], Color.Purple, SeriesChartType.Line, 4);
            // Chart.AddSeries("2H", lcSet[LcType.H][2], Color.Green, SeriesChartType.Line, 4);
            // Chart.AddSeries("2V", lcSet[LcType.V][2], Color.Green, SeriesChartType.Line, 4);
            // Chart.AddSeries("3H", lcSet[LcType.H][3], Color.Black, SeriesChartType.Line, 4);
            // Chart.AddSeries("3V", lcSet[LcType.V][3], Color.Black, SeriesChartType.Line, 4);
            // Chart.AddSeries("4H", lcSet[LcType.H][4], Color.SaddleBrown, SeriesChartType.Line, 4);
            // Chart.AddSeries("4V", lcSet[LcType.V][4], Color.SaddleBrown, SeriesChartType.Line, 4);

            foreach (var borderSegment in borderSegments)
                Chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.FastLine, borderWidth: 3);

            Chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, markerSize: 4);

            attractor2.SaveToFile("eq_with_noise.txt", "model1_lc2");
            lcSet.SaveToFile("lc.txt", "model1_lc2");
            ellipse.SaveToFile("band.txt", "model1_lc2");
            // lcSet.SaveToFile(ellipse, "model1_chaos2\\LC_border.txt");
        }

        private static (IList<(double D12, double Eps)> Points, ChartForm chart) Test8_Parallel(double d12Start,
            double d12End, double d12Step, Func<double, (double D12, double Eps)> searcher)
        {
            var count = (int) Math.Round((d12End - d12Start) / d12Step);
            var points = Enumerable.Range(0, count)
                .Select(i => d12Start + i * d12Step)
                .Concat(new[] { d12End })
                .AsParallel()
                .AsOrdered()
                .Select(searcher)
                .Where(p => p != default((double, double)))
                .ToList();

            var chart = new ChartForm(points, d12Start, d12End, 0, 2, markerSize: 4);

            return (points, chart);
        }

        private static (IList<(double D12, double Eps)> Points, ChartForm chart) Test8_Parallel_Old(
            DeterministicModel1 dModel1, StochasticModel1 sModel1, double d12Start, double d12End,
            Func<DeterministicModel1, StochasticModel1, double, double, IList<(double D12, double Eps)>> searcher)
        {
            var n = Environment.ProcessorCount;
            var tasks = new Task<IList<(double D12, double Eps)>>[n];
            var d12Part = (d12End - d12Start) / n;

            for (var i = 0; i < n; i++)
            {
                var d12PartStart = d12Start + d12Part * i;
                var d12PartEnd = d12Start + d12Part * (i + 1);
                tasks[i] = Task.Run(() => searcher((DeterministicModel1) dModel1.Copy(),
                    (StochasticModel1) sModel1.Copy(), d12PartStart, d12PartEnd));
            }

            var points = tasks.SelectMany(t => t.Result).ToList();
            var chart = new ChartForm(points, d12Start, d12End, 0, 2, markerSize: 4);

            return (points, chart);
        }

        private static IList<PointX> GetEllipse(StochasticModel1 model, PointX point, double[,] matrix = null)
        {
            var sensitivityMatrix = matrix ?? SensitivityMatrix.Get(model, point);
            var eigenvalueDecomposition = new EigenvalueDecomposition(sensitivityMatrix);
            var eigenvalues = eigenvalueDecomposition.RealEigenvalues;
            var eigenvectors = eigenvalueDecomposition.Eigenvectors;

            return ScatterEllipse.Get(point, eigenvalues[0], eigenvalues[1],
                eigenvectors.GetColumn(0), eigenvectors.GetColumn(1), model.Eps).ToList();
        }

        private static IEnumerable<IList<PointX>> GetEllipses(StochasticModel1 model, IList<PointX> points)
        {
            return SensitivityMatrix.Get(model, points).Select((m, i) => GetEllipse(model, points[i], m));
        }

        private static bool ValidateZik(double d12, List<PointX> attractor)
        {
            if (attractor.Count == 0)
            {
                Console.WriteLine($"Error: zik not build on d12 = {d12}");
                return false;
            }

            if (AttractorExtensions.Is3Cycle(attractor))
            {
                Console.WriteLine($"Error: 3-cycle on d12 = {d12}");
                return false;
            }

            if (AttractorExtensions.IsEquilibrium(attractor))
            {
                Console.WriteLine($"Error: eq on d12 = {d12}");
                return false;
            }

            return true;
        }

        private void SetModels()
        {
            sModel.Sigma1 = 0;
            sModel.Sigma2 = 0;
            sModel.Sigma3 = 1;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
        }
    }
}
