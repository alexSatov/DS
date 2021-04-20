using System.Drawing;
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
        public void TestCycle()
        {
            var model = GetModel_2(0.0014, 0.0075);

            var points = PhaseTrajectory.Get(model, new[] { 0.5, 0.5 }, 2000, 1000)
                .Select(p => (p[0], p[1]));

            Chart = new ChartForm(points, 0, 1, 0, 1);
        }

        [Test]
        public void TestDiagram()
        {
            var model = GetModel_2(0.0017, 0.0063);
            var interval = new Interval<double>(0.0017, 0.0025);
            var dParams = new DParams(interval);

            var points = BifurcationDiagram.Get(model, dParams, new[] { 0.25, 0.125 }, 1000)
                .Select(p => (p.D, p.Point[0]))
                .Distinct();

            Chart = new ChartForm(points, 0.0017, 0.0025, 0.25, 1.125);
        }

        [Test]
        public void TestDiagram2()
        {
            var model = GetModel_2();
            var intervalX = new Interval<double>(0, 0.004);
            var intervalY = new Interval<double>(0, 0.016);
            var dParams = new D2Params(intervalX, intervalY);

            var attractors = BifurcationDiagram.Get(model, dParams, new[] { 0.5, 0.5 }, 400, 400)
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Params).ToList());

            Chart = new ChartForm(attractors[AttractorType.Infinity], 0, 0.004, 0, 0.016,
                AttractorType.Infinity.ToColor());

            foreach (var (type, points) in attractors.Where(a => a.Key != AttractorType.Infinity))
            {
                Chart.AddSeries(type.ToString(), points, type.ToColor());
            }
        }

        [Test]
        public void TestDiagramPolar()
        {
            var model = GetModel_2();
            var intervalX = new Interval<double>(0, 0.00245);
            var intervalY = new Interval<double>(0.007, 0.008);
            var dParams = new D2Params(intervalX, intervalY, ByPreviousType: ByPreviousType.Polar);

            var attractors = BifurcationDiagram.GetPolar(model, dParams, new PointD(0.00159, 0.0072622), new[] { 0.5, 0.5 }, 360)
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
        public void TestDiagramPolar2()
        {
            var model = GetModel_2();
            var intervalX = new Interval<double>(0.00205, 0.00245);
            var intervalY = new Interval<double>(0.0068, 0.008);
            var dParams = new D2Params(intervalX, intervalY, ByPreviousType: ByPreviousType.Polar);

            var attractors = BifurcationDiagram.GetPolar(model, dParams, new PointD(0.002272, 0.007308), new[] { 0.925, 0.83 }, 360)
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
        public void TestLyapunov()
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
