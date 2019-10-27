using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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
            dModel.D12 = 0.00186;
            sModel.D12 = 0.00186;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
            sModel.Eps = 0.005;
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;

            var zik = PhaseTrajectory.GetWhile(dModel, new PointX(20, 40), 10000, 0.0001);
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

            PointSaver.SaveToFile("ellipse/chaosZik.txt", chaosZik);
            PointSaver.SaveToFile("ellipse/zik1.txt", zik1);
            PointSaver.SaveToFile("ellipse/zik2.txt", zik2);
            PointSaver.SaveToFile("ellipse/zik3.txt", zik3);
            PointSaver.SaveToFile("ellipse/ellipse1.txt", ellipse1);
            PointSaver.SaveToFile("ellipse/ellipse2.txt", ellipse2);

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

        /// <summary>
        /// Крит. интенсивность. 1 зона: ЗИК - 3цикл, D12 in (0.00145, 0.001682)
        /// </summary>
        public static ChartForm Test8_1(DeterministicModel dModel, StochasticModel sModel)
        {
            SetModels(dModel, sModel);

            IList<(double D12, double Eps)> Search(DeterministicModel dInnerModel, StochasticModel sInnerModel,
                double d12Start, double d12End)
            {
                const double step = 0.000001;
                var result = new List<(double D12, double Eps)>();
                var attractor = new List<PointX> {new PointX(10, 40)};

                for (var d12 = d12Start; d12 < d12End; d12 += step)
                {
                    dInnerModel.D12 = d12;
                    sInnerModel.D12 = d12;
                    attractor = PhaseTrajectory.GetWhile(dInnerModel, attractor[attractor.Count - 1], 5000, 0.0001);

                    for (var eps = 0.1; eps < 2; eps += 0.1)
                    {
                        sInnerModel.Eps = eps;
                        var finded = false;
                        var (ellipse, _) = ScatterEllipse.GetForZik2(dInnerModel, sInnerModel, attractor);

                        foreach (var ellipsePoint in ellipse)
                        {
                            var otherAttractor = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 9996, 4);

                            if (!Is3Cycle(otherAttractor))
                                continue;

                            finded = true;

                            Console.WriteLine($"{string.Join(", ", attractor.Take(4))}; " +
                                $"{string.Join(", ", otherAttractor)}; {d12}");

                            result.Add((d12, eps));

                            break;
                        }

                        if (finded)
                            break;
                    }
                }

                return result;
            }

            var (points, chart) = Test8_Parallel(dModel, sModel, 0.00145, 0.001682, Search);

            PointSaver.SaveToFile("crit_intens\\zone1_1.txt", points);

            return chart;
        }

        /// <summary>
        /// Крит. интенсивность. 2 зона: равновесие - 3цикл, D12 in (0.001909, 0.001974)
        /// </summary>
        public static ChartForm Test8_2(DeterministicModel dModel, StochasticModel sModel)
        {
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;

            (PointX Eq1, PointX Eq2, IEnumerable<PointX> Chaos, IEnumerable<PointX> Ellipse) Search()
            {
                for (var d12 = 0.001909; d12 < 0.001974; d12 += 0.000001)
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
            chart.AddSeries("attractor1", new List<PointX> { eq1 }, Color.Black, 8);
            chart.AddSeries("attractor2", new List<PointX> { eq2 }, Color.Blue, 8);
            chart.AddSeries("ellipse", _ellipse, Color.Red);

            return chart;
        }

        /// <summary>
        /// Крит. интенсивность. 3 зона: равновесие - равновесие, D12 in (0.002166, 0.002294)
        /// </summary>
        public static ChartForm Test8_3(DeterministicModel dModel, StochasticModel sModel)
        {
            SetModels(dModel, sModel);

            IList<(double D12, double Eps)> Search(DeterministicModel dInnerModel, StochasticModel sInnerModel,
                double d12Start, double d12End)
            {
                const double step = 0.000001;
                var result = new List<(double D12, double Eps)>();
                var eq = new PointX(18, 68);

                for (var d12 = d12Start; d12 < d12End; d12 += step)
                {
                    dInnerModel.D12 = d12;
                    sInnerModel.D12 = d12;
                    eq = PhaseTrajectory.Get(dInnerModel, eq, 9999, 1).First();

                    for (var eps = 0.1; eps < 2; eps += 0.1)
                    {
                        sInnerModel.Eps = eps;
                        var finded = false;
                        var ellipse = GetEllipse(sInnerModel, eq);

                        foreach (var ellipsePoint in ellipse)
                        {
                            var otherEq = PhaseTrajectory.Get(dInnerModel, ellipsePoint, 9999, 1).First();

                            if (otherEq.X1 < 28)
                                continue;

                            finded = true;

                            Console.WriteLine($"{eq}; {otherEq}; {d12}");
                            result.Add((d12, eps));

                            break;
                        }

                        if (finded)
                            break;
                    }
                }

                return result;
            }

            var (points, chart) = Test8_Parallel(dModel, sModel, 0.002166, 0.002294, Search);

            PointSaver.SaveToFile("crit_intens\\zone3_1.txt", points);

            return chart;
        }

        /// <summary>
        /// Крит. интенсивность. 4 зона: ЗИК - 3ЗИК, D12 in (0.001682, 0.00173)
        /// </summary>
        public static ChartForm Test8_4(DeterministicModel dModel, StochasticModel sModel)
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
            chart.AddSeries("attractor1", new List<PointX> { eq1 }, Color.Black, 8);
            chart.AddSeries("attractor2", new List<PointX> { eq2 }, Color.Blue, 8);
            chart.AddSeries("ellipse", _ellipse, Color.Red);

            return chart;
        }

        /// <summary>
        /// Крит. интенсивность. 5 зона: равновесие - 3ЗИК, D12 in (0.001855, 0.001909)
        /// </summary>
        public static ChartForm Test8_5(DeterministicModel dModel, StochasticModel sModel)
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
            chart.AddSeries("attractor1", new List<PointX> { eq1 }, Color.Black, 8);
            chart.AddSeries("attractor2", new List<PointX> { eq2 }, Color.Blue, 8);
            chart.AddSeries("ellipse", _ellipse, Color.Red);

            return chart;
        }

        private static (IList<(double D12, double Eps)> Points, ChartForm chart) Test8_Parallel(
            DeterministicModel dModel, StochasticModel sModel, double d12Start, double d12End,
            Func<DeterministicModel, StochasticModel, double, double, IList<(double D12, double Eps)>> searcher)
        {
            var n = Environment.ProcessorCount;
            var tasks = new Task<IList<(double D12, double Eps)>>[n];
            var d12Part = (d12End - d12Start) / n;

            for (var i = 0; i < n; i++)
            {
                var d12PartStart = d12Start + d12Part * i;
                var d12PartEnd = d12Start + d12Part * (i + 1);
                tasks[i] = Task.Run(() => searcher((DeterministicModel) dModel.Copy(),
                    (StochasticModel) sModel.Copy(), d12PartStart, d12PartEnd));
            }

            var points = tasks.SelectMany(t => t.Result).ToList();
            var chart = new ChartForm(points, d12Start, d12End, 0, 2, markerSize: 4);

            return (points, chart);
        }

        private static IList<PointX> GetEllipse(StochasticModel model, PointX point)
        {
            var sensitivityMatrix = SensitivityMatrix.Get(model, point);
            var eigenvalueDecomposition = new EigenvalueDecomposition(sensitivityMatrix);
            var eigenvalues = eigenvalueDecomposition.RealEigenvalues;
            var eigenvectors = eigenvalueDecomposition.Eigenvectors;

            return ScatterEllipse.Get(point, eigenvalues[0], eigenvalues[1],
                eigenvectors.GetColumn(0), eigenvectors.GetColumn(1), model.Eps).ToList();
        }

        private static void SetModels(DeterministicModel dModel, StochasticModel sModel)
        {
            sModel.Sigma1 = 1;
            sModel.Sigma2 = 1;
            sModel.Sigma3 = 0;
            dModel.D21 = 0.0075;
            sModel.D21 = 0.0075;
        }

        private static bool Is3Cycle(IList<PointX> points)
        {
            return points[0].AlmostEquals(points[3]);
        }
    }
}
