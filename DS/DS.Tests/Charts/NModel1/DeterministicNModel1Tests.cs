using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using DS.Helpers;
using DS.MathStructures;
using DS.MathStructures.Points;
using DS.Models;
using DS.Tests.Extensions;
using NUnit.Framework;

namespace DS.Tests.Charts.NModel1
{
    public class DeterministicNModel1Tests : ChartTests
    {
        [Test]
        public void TestZik()
        {
            var model = GetModel_2(0.0014, 0.0075);

            var points = PhaseTrajectory.Get(model, new[] { 0.5, 0.5 }, 8000, 2000)
                .Select(p => (p[0], p[1]));

            Chart = new ChartForm(points, 0, 1, 0, 1);
        }

        [Test]
        public void TestZik_FromFile()
        {
            var points = File
                .ReadAllLines("D:\\src\\DS\\DS\\DS.ConsoleApp\\bin\\Debug\\data\\PhaseTrajectory_2021-05-09T17-40-30\\PhaseTrajectory.txt")
                .Select(s =>
                {
                    var result = s.Split(' ');
                    return (double.Parse(result[0], CultureInfo.InvariantCulture),
                        double.Parse(result[1], CultureInfo.InvariantCulture));
                });

            Chart = new ChartForm(points, 0, 1, 0, 1);
        }

        [Test]
        public void TestBifurcationDiagram()
        {
            var model = GetModel_2(0.0017, 0.0063);
            var interval = new Interval<double>(0.0017, 0.0025);
            var dParams = new DParams(interval, ByPrevious: true);

            var points = BifurcationDiagram.Get(model, dParams, new[] { 0.25, 0.125 })
                .Select(p => (p.D, p.Point[0]))
                .Distinct();

            Chart = new ChartForm(points, 0.0017, 0.0025, 0.25, 1.125);
        }

        [Test]
        public void TestBifurcationDiagram_FromFile()
        {
            var points = File
                .ReadAllLines("D:\\src\\DS\\DS\\DS.ConsoleApp\\bin\\Debug\\data\\BifurcationDiagram_2021-05-09T18-04-09\\BifurcationDiagram.txt")
                .Select(s =>
                {
                    var result = s.Split(' ');
                    return (double.Parse(result[0], CultureInfo.InvariantCulture),
                        double.Parse(result[1], CultureInfo.InvariantCulture));
                });

            Chart = new ChartForm(points, 0.0017, 0.0025, 0.25, 1.125);
        }

        [Test]
        public void TestModeMap()
        {
            var model = GetModel_2();
            var intervalX = new Interval<double>(0, 0.00245);
            var intervalY = new Interval<double>(0.007, 0.008);
            var dParams = new D2Params(intervalX, intervalY, ByPreviousType: ByPreviousType.None);

            var attractors = BifurcationDiagram.Get(model, dParams, new[] { 0.5, 0.5 })
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Params).ToList());

            Chart = new ChartForm(attractors[AttractorType.Equilibrium], 0, 0.00245, 0.007, 0.008,
                AttractorType.Equilibrium.ToColor());

            foreach (var (type, points) in attractors.Where(a => a.Key != AttractorType.Equilibrium))
            {
                Chart.AddSeries(type.ToString(), points, type.ToColor());
            }
        }

        [Test]
        public void TestModeMap_FromFile()
        {
            var attractors = new Dictionary<AttractorType, List<PointD>>();

            foreach (var type in Enum.GetValues<AttractorType>())
            {
                var path = $"D:\\src\\DS\\DS\\DS.ConsoleApp\\bin\\Debug\\data\\ModeMap_2021-05-09T18-05-20\\{type}.txt";
                var points = File
                    .ReadAllLines(path)
                    .Select(s =>
                    {
                        var result = s.Split(' ');
                        return new PointD(double.Parse(result[0], CultureInfo.InvariantCulture),
                            double.Parse(result[1], CultureInfo.InvariantCulture));
                    })
                    .ToList();

                attractors[type] = points;
            }

            Chart = new ChartForm(attractors[AttractorType.Infinity], 0, 0.004, 0, 0.016,
                AttractorType.Infinity.ToColor());

            foreach (var (type, points) in attractors.Where(a => a.Key != AttractorType.Infinity))
            {
                Chart.AddSeries(type.ToString(), points, type.ToColor());
            }
        }

        [Test]
        public void TestPolarModeMap()
        {
            var model = GetModel_2();
            var intervalX = new Interval<double>(0, 0.00245);
            var intervalY = new Interval<double>(0.007, 0.008);
            var dParams = new D2Params(intervalX, intervalY, ByPreviousType: ByPreviousType.Polar);

            var attractors = BifurcationDiagram.GetPolar(model, dParams, new PointD(0.00159, 0.0072622), 360, new[] { 0.5, 0.5 })
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Params).ToList());

            Chart = new ChartForm(attractors[AttractorType.Equilibrium], 0, 0.00245, 0.007, 0.008,
                AttractorType.Equilibrium.ToColor());

            foreach (var (type, points) in attractors.Where(a => a.Key != AttractorType.Equilibrium))
            {
                Chart.AddSeries(type.ToString(), points, type.ToColor());
            }
        }

        [Test]
        public void TestPolarModeMap_FromFile()
        {
            var attractors = new Dictionary<AttractorType, List<PointD>>();

            foreach (var type in Enum.GetValues<AttractorType>())
            {
                var path = $"D:\\src\\DS\\DS\\DS.ConsoleApp\\bin\\Debug\\data\\ModeMap_2021-05-09T18-13-24\\{type}.txt";
                if (!File.Exists(path))
                    continue;

                var points = File
                    .ReadAllLines(path)
                    .Select(s =>
                    {
                        var result = s.Split(' ');
                        return new PointD(double.Parse(result[0], CultureInfo.InvariantCulture),
                            double.Parse(result[1], CultureInfo.InvariantCulture));
                    })
                    .ToList();

                attractors[type] = points;
            }

            Chart = new ChartForm(attractors[AttractorType.Infinity], 0, 0.00245, 0.007, 0.008,
                AttractorType.Infinity.ToColor());

            foreach (var (type, points) in attractors.Where(a => a.Key != AttractorType.Equilibrium))
            {
                Chart.AddSeries(type.ToString(), points, type.ToColor());
            }
        }

        [Test]
        public void TestPolarModeMap2()
        {
            var model = GetModel_2();
            var intervalX = new Interval<double>(0.00205, 0.00245);
            var intervalY = new Interval<double>(0.0068, 0.008);
            var dParams = new D2Params(intervalX, intervalY, ByPreviousType: ByPreviousType.Polar);

            var attractors = BifurcationDiagram.GetPolar(model, dParams, new PointD(0.002272, 0.007308), 360, new[] { 0.925, 0.83 })
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Params).ToList());

            Chart = new ChartForm(attractors[AttractorType.Equilibrium], 0.00205, 0.00245, 0.0068, 0.008,
                AttractorType.Equilibrium.ToColor());

            foreach (var (type, points) in attractors.Where(a => a.Key != AttractorType.Equilibrium))
            {
                Chart.AddSeries(type.ToString(), points, type.ToColor());
            }

            attractors.SaveToDir();
        }

        [Test]
        public void TestLyapunovComponents()
        {
            var model = GetModel_2(0.000045, 0.0075);
            var interval = new Interval<double>(0.000045, 0.00245);
            var dParams = new DParams(interval, ByPrevious: true);

            var result = Lyapunov.Get(model, dParams, new[] { 0.5, 0.5 });
            var l1Points = result.Select(p => (p.D, p.L[0])).ToList();
            var l2Points = result.Select(p => (p.D, p.L[1])).ToList();

            Chart = new ChartForm(l1Points, 0, 0.00245, -3.5, 1);
            Chart.AddSeries("D12vsL2", l2Points, Color.Red);
        }

        [Test]
        public void TestLyapunovComponents_FromFile()
        {
            var points = File
                .ReadAllLines("D:\\src\\DS\\DS\\DS.ConsoleApp\\bin\\Debug\\data\\LyapunovComponents_2021-05-09T18-14-11\\LyapunovComponents.txt")
                .Select(s =>
                {
                    var result = s.Split(' ');
                    return (double.Parse(result[0], CultureInfo.InvariantCulture),
                        double.Parse(result[1], CultureInfo.InvariantCulture),
                        double.Parse(result[2], CultureInfo.InvariantCulture));
                })
                .ToList();

            var l1Points = points.Select(p => (p.Item1, p.Item2)).ToList();
            var l2Points = points.Select(p => (p.Item1, p.Item3)).ToList();

            Chart = new ChartForm(l1Points, 0, 0.00245, -3.5, 1);
            Chart.AddSeries("D12vsL2", l2Points, Color.Red);
        }

        private static DeterministicNModel1 GetModel_2(double d12 = 0, double d21 = 0)
        {
            var d = new[,]
            {
                { 0.0002, d12 },
                { d21, 0.00052 }
            };

            return new DeterministicNModel1(2, new double[] { 10, 20 }, d, 0.25, 1);
        }

        protected override void OnSetUp()
        {
        }
    }
}
