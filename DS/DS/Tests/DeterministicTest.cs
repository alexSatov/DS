using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using DS.Helpers;
using DS.MathStructures;

namespace DS
{
    public static class DeterministicTest
    {
        public static ChartForm Test1(DeterministicModel model)
        {
            model.D12 = 0.0014;
            model.D21 = 0.0075;

            var points = PhaseTrajectory.Get(model, new PointX(20, 40), 2000, 1000)
                .Select(p => (p.X1, p.X2));

            return new ChartForm(points, 0, 40, 0, 80);
        }

        public static ChartForm Test2(DeterministicModel model)
        {
            model.D21 = 0.0063;

            var points = BifurcationDiagram.GetD12VsXParallel(model, new PointX(10, 10), 0.0025, 0.000001)
                .Distinct()
                .Select(v => (v.D12, v.X1));

            return new ChartForm(points, 0.0017, 0.0025, 10, 45);
        }

        public static ChartForm Test3(DeterministicModel model)
        {
            model.D12 = 0.0017;
            model.D21 = 0.005;

            var points = BifurcationDiagram.GetD12VsD21ParallelByD21(model, new PointX(20, 40), 0.00245, 0.00792,
                0.000005, 0.00002);

            var eqX2Gt2X1 = points.EquilibriumPoints
                .Where(t => t.X.X2 > 2 * t.X.X1)
                .Select(t => (t.D.D12, t.D.D21));

            var eqX2Lt2X1 = points.EquilibriumPoints
                .Where(t => t.X.X2 < 2 * t.X.X1)
                .Select(t => (t.D.D12, t.D.D21));

            var chart = new ChartForm(eqX2Gt2X1, 0.0017, 0.00245, 0.005, 0.00792);

            chart.AddSeries("equilibrium x2 < 2x1", eqX2Lt2X1, Color.Aqua);

            AddCycles(chart, points.CyclePoints);

            return chart;
        }

        public static ChartForm Test4(DeterministicModel model)
        {
            var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(20, 40), 0.004, 0.016,
                0.00002, 0.00008);

            return GetCyclesChart(points, 0, 0.004, 0, 0.016);
        }

        public static ChartForm Test5(DeterministicModel model)
        {
            model.D12 = 0.00005;
            model.D21 = 0.007;
            var points = BifurcationDiagram.GetD12VsD21(model, new PointX(20, 40), 0.00245, 0.008,
                0.000004, 0.0000016);

            var chart = GetCyclesChart(points, 0, 0.00245, 0.007, 0.008);

            return chart;
        }

        public static ChartForm Test6(DeterministicModel model)
        {
            const double step = 0.000002;
            model.D21 = 0.0075;

            // [0.000045, 0.00245]
            IEnumerable<(double D12, double X1)> FirstAttractor()
            {
                model.D12 = 0.000045;
                return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.00245, step)
                    .Distinct()
                    .Select(v => (v.D12, v.X1));
            }

            // [0.00145, 0.0019746]
            IEnumerable<(double D12, double X1)> SecondAttractor()
            {
                model.D12 = 0.00145;
                return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(20, 40), 0.0019746, step)
                    .Distinct()
                    .Select(v => (v.D12, v.X1));
            }

            // [0.002166, 0.0023825]
            IEnumerable<(double D12, double X1)> ThirdAttractor()
            {
                model.D12 = 0.002166;
                return BifurcationDiagram.GetD12VsXByPrevious(model, new PointX(34.5964044040563, 44.473609663403728),
                        0.0023825, step)
                    .Distinct()
                    .Select(p => (p.D12, p.X1));
            }

            var points = FirstAttractor().ToList();

            //PointSaver.SaveToFile("d12_x1_1.txt", points);

            return new ChartForm(points, 0, 0.00245, 0, 45);
        }

        // d12 = 0.00145 - ЗИК и 3х цикл: (6, 62); (17, 44); (24, 75)
        // d12 = 0.00197 - равновесие и 3х цикл: (12, 65); (20, 62); (23, 72)
        // d12 = 0.00217 - равновесие и равновесие: (18, 68) и (35, 44)
        // d12 = 0.002382 - хаос и равновесие: (20, 68)
        public static ChartForm Test7(DeterministicModel model)
        {
            model.D12 = 0.001909;
            model.D21 = 0.0075;

            var points = AttractorPool.GetX1VsX2Parallel(model, new PointX(-5, -5), new PointX(45, 85), 0.5, 0.9);

            return AttractorPoolChart.GetAttractorPoolChart1(points, -5, 45, -5, 85);
        }

        public static ChartForm Test8(DeterministicModel model)
        {
            var points = BifurcationDiagram.GetD12VsD21ByPreviousPolarParallel(model, new PointX(20, 40),
                new PointD(0.00159, 0.0072622), new Rect(0, 0.00245, 0.007, 0.008), 0.001, 0.000004, 0.0000017);
            var chart = GetCyclesChart(points, 0, 0.00245, 0.007, 0.008);

            return chart;
        }

        public static ChartForm Test9(DeterministicModel model)
        {
            const double step = 0.0000002;
            model.D21 = 0.0075;

            IEnumerable<(double D12, double L1, double L2)> FirstAttractor()
            {
                model.D12 = 0.000045;
                return Lyapunov.GetD12Indicators(model, new PointX(20, 40), 0.00245, step, true, t: 500000);
            }

            IEnumerable<(double D12, double L1, double L2)> SecondAttractor()
            {
                model.D12 = 0.00145;
                return Lyapunov.GetD12Indicators(model, new PointX(20, 40), 0.0019746, step, true, t: 500000);
            }

            IEnumerable<(double D12, double L1, double L2)> ThirdAttractor()
            {
                model.D12 = 0.002166;
                return Lyapunov.GetD12Indicators(model, new PointX(34.5964044040563, 44.473609663403728),
                        0.0023825, step, true, t: 500000);
            }

            var points = FirstAttractor().ToList();
            var l1Points = points.Select(p => (p.D12, p.L1)).ToList();
            var l2Points = points.Select(p => (p.D12, p.L2)).ToList();

            //Console.WriteLine($"Max l1: {l1Points.Max(p => p.Item2)}");
            //Console.WriteLine($"Min l1: {l1Points.Min(p => p.Item2)}");
            //Console.WriteLine($"Max l2: {l2Points.Max(p => p.Item2)}");
            //Console.WriteLine($"Min l2: {l2Points.Min(p => p.Item2)}");

            var chart = new ChartForm(l1Points, 0, 0.00245, -3.5, 1);

            chart.AddSeries("D12vsL2", l2Points, Color.Red);

            PointSaver.SaveToFile("lyapunov\\l1.txt", l1Points);
            PointSaver.SaveToFile("lyapunov\\l2.txt", l2Points);

            return chart;
        }

        // 0.000003, 0.000001363 (по 734)
        public static ChartForm Test10(DeterministicModel model)
        {
            model.D12 = 0.0002;
            model.D21 = 0.007;

            var points = Lyapunov.GetIndicatorsByD21(model, new PointX(20, 40), 0.0024, 0.008, 0.000003, 0.000001363)
                .OrderByDescending(p => p.D21)
                .ThenBy(p => p.D12)
                .ToList();

            var l1Result = new StringBuilder(points[0].L1.Format());
            var l2Result = new StringBuilder(points[0].L2.Format());

            var currentD21 = points[0].D21;

            foreach (var point in points.Skip(1))
            {
                if (point.D21 != currentD21)
                {
                    currentD21 = point.D21;

                    l1Result.AppendLine();
                    l2Result.AppendLine();
                    l1Result.Append(point.L1.Format());
                    l2Result.Append(point.L2.Format());

                    continue;
                }

                l1Result.Append(" ");
                l2Result.Append(" ");
                l1Result.Append(point.L1.Format());
                l2Result.Append(point.L2.Format());
            }

            File.WriteAllText("lyapunovMap\\l1.txt", l1Result.ToString());
            File.WriteAllText("lyapunovMap\\l2.txt", l2Result.ToString());

            return null;
        }

        public static ChartForm Test11(DeterministicModel model)
        {
            model.D12 = 0.002382;
            model.D21 = 0.0075;

            var points = PhaseTrajectory.Get(model, new PointX(32.95, 47.38), 10000, 10000);
            var chart = new ChartForm(points, -5, 45, -5, 85);

            PointSaver.SaveToFile("chaosAttractor.txt", points);

            return chart;
        }

        /// <summary>
        /// Построение критических линий (для хаоса d12 = 0.002382)
        /// </summary>
        public static ChartForm Test12(DeterministicModel model)
        {
            model.D12 = 0.00237;
            model.D21 = 0.0075;

            var attractor = PhaseTrajectory.Get(model, new PointX(34, 61), 50000, 50000);
            var lcList = LcSet.FromAttractor(model, attractor, 9)[LcType.H];
            var chart = new ChartForm(attractor, 32.8, 38.4, 25, 50.5);

            chart.AddSeries("lc0", lcList[0], Color.Red, SeriesChartType.Line, 5);
            chart.AddSeries("lc1", lcList[1], Color.Black, SeriesChartType.Line, 5);
            chart.AddSeries("lc2", lcList[2], Color.DarkBlue, SeriesChartType.Line, 5);
            chart.AddSeries("lc3", lcList[3], Color.Blue, SeriesChartType.Line, 5);
            chart.AddSeries("lc4", lcList[4], Color.Green, SeriesChartType.Line, 5);
            chart.AddSeries("lc5", lcList[5], Color.Gold, SeriesChartType.Line, 5);
            chart.AddSeries("lc6", lcList[6], Color.Orange, SeriesChartType.Line, 5);
            chart.AddSeries("lc7", lcList[7], Color.Violet, SeriesChartType.Line, 5);
            chart.AddSeries("lc8", lcList[8], Color.DarkViolet, SeriesChartType.Line, 5);

            return chart;
        }

        /// <summary>
        /// Построение критических линий (для хаоса d12 = 0.00237). Выделяем границу.
        /// </summary>
        public static ChartForm Test13(DeterministicModel model)
        {
            model.D12 = 0.00237;
            model.D21 = 0.0075;

            var i = 0;
            var attractor = PhaseTrajectory.Get(model, new PointX(34, 61), 50000, 50000);
            var lcList = LcSet.FromAttractor(model, attractor, 9);
            var borderSegments = lcList.GetBorderSegments();

            var chart = new ChartForm(attractor, 32.8, 38.4, 25, 50.5);

            foreach (var borderSegment in borderSegments)
                chart.AddSeries($"border{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    seriesChartType: SeriesChartType.FastLine);

            return chart;
        }

        /// <summary>
        /// Построение бассейнов
        /// </summary>
        public static ChartForm Test15(DeterministicModel model)
        {
            model.D12 = 0.00236;
            model.D21 = 0.0075;

            var pool = AttractorPool.GetPoolFor2AttractorsParallel(model, Attractor.Is3Cycle, new PointX(0, 0),
                new PointX(40, 80), 0.5, 0.8);

            var chart = new ChartForm(pool.First, 0, 40, 0, 80, markerSize: 4);
            chart.AddSeries("pool_second", pool.Second, Color.Orange, markerSize: 4);

            //PointSaver.SaveToFile("ellipse/attractor.txt", chaosZik);
            //PointSaver.SaveToFile("ellipse/ellipse1.txt", ellipse1);
            //PointSaver.SaveToFile("ellipse/ellipse2.txt", ellipse2);

            return chart;
        }

        /// <summary>
        /// Карта режимов для правой области
        /// </summary>
        public static ChartForm Test16_1(DeterministicModel model)
        {
            model.D12 = 0.0022;
            model.D21 = 0.001;
            var points = BifurcationDiagram.GetD12VsD21ParallelByD12(model, new PointX(20, 40), 0.0032, 0.01,
                0.0000025, 0.000025);

            var chart = GetCyclesChart(points, 0.0022, 0.0032, 0.001, 0.01);

            return chart;
        }

        /// <summary>
        /// Карта режимов для правой области (полярный алгоритм)
        /// синий - 4 цикл: D12=0.0025, D21=0.004; красный - 3 цикл: D12=0.0025, D21=0.0055
        /// </summary>
        public static ChartForm Test16_2(DeterministicModel model)
        {
            var points = BifurcationDiagram.GetD12VsD21ByPreviousPolarParallel(model, new PointX(20, 40),
                new PointD(0.0025, 0.0055), new Rect(0.0022, 0.0032, 0.001, 0.01), 0.001, 0.000001, 0.00001);

            var chart = GetCyclesChart(points, 0.0022, 0.0032, 0.001, 0.01);

            return chart;
        }

        private static ChartForm GetCyclesChart(BifurcationDiagram.D12VsD21Result points,
            double ox1, double ox2, double oy1, double oy2)
        {
            var zeroPoint = new PointX(0, 0);

            var infinity = points.InfinityPoints.ToList();

            var eqX1EqX2Eq0 = points.EquilibriumPoints
                .Where(t => t.X.AlmostEquals(zeroPoint))
                .Select(t => t.D)
                .ToList();

            var eqX2Gt2X1 = points.EquilibriumPoints
                .Where(t => t.X.X2 > 2 * t.X.X1)
                .Select(t => t.D)
                .ToList();

            var eqX2Lt2X1 = points.EquilibriumPoints
                .Where(t => t.X.X2 < 2 * t.X.X1)
                .Select(t => t.D)
                .ToList();

            var chart = new ChartForm(infinity, ox1, ox2, oy1, oy2, Color.Gray, "infinity");

            chart.AddSeries("equilibrium x2 < 2x1", eqX2Lt2X1, Color.ForestGreen);
            chart.AddSeries("equilibrium x2 > 2x1", eqX2Gt2X1, Color.DeepSkyBlue);
            chart.AddSeries("equilibrium x1 = x2 = 0", eqX1EqX2Eq0, Color.Goldenrod);

            PointSaver.SaveToFile("map\\infinity.txt", infinity);
            PointSaver.SaveToFile("map\\eqX2Lt2X1.txt", eqX2Lt2X1);
            PointSaver.SaveToFile("map\\eqX2Gt2X1.txt", eqX2Gt2X1);

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
                PointSaver.SaveToFile($"map\\{name}.txt", points);
            }
        }
    }
}
