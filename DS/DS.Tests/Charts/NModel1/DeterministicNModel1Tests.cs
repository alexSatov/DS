using System.Linq;
using DS.MathStructures;
using DS.Models;
using DS.Tests.Extensions;
using NUnit.Framework;

namespace DS.Tests.Charts.NModel1
{
    public class DeterministicNModel1Tests : ChartTests
    {
        [Test]
        public void Test1()
        {
            var model = GetModel_2();

            var points = PhaseTrajectory.Get(model, new[] { 0.5, 0.5 }, 2000, 1000)
                .Select(p => (p[0], p[1]));

            Chart = new ChartForm(points, 0, 1, 0, 1);
        }

        [Test]
        public void Test2()
        {
            var model = GetModel_2(0.0017, 0.0063);
            var interval = new Interval<double>(0.0017, 0.0025);
            var dParams = new DParams(interval, 0, 1);

            var points = BifurcationDiagram.Get(model, dParams, new[] { 0.25, 0.125 }, 1000)
                .Select(p => (p.D, p.Point[0]))
                .Distinct();

            Chart = new ChartForm(points, 0.0017, 0.0025, 0.25, 1.125);
        }

        [Test]
        public void Test3()
        {
            var model = GetModel_2(0, 0);
            var intervalX = new Interval<double>(0, 0.004);
            var intervalY = new Interval<double>(0, 0.016);
            var dParams = new D2Params(intervalX, intervalY, 0, 1, 1, 0, ByPreviousType.Y);

            var attractors = BifurcationDiagram.Get(model, dParams, new[] { 0.25, 0.125 }, 400, 400)
                .GroupBy(a => a.Type)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Params).ToList());

            Chart = new ChartForm(attractors[AttractorType.Infinity], 0, 0.004, 0, 0.016,
                AttractorType.Infinity.ToColor());

            foreach (var (type, points) in attractors.Where(a => a.Key != AttractorType.Infinity))
            {
                Chart.AddSeries(type.ToString(), points, type.ToColor());
            }
        }

        private static DeterministicNModel1 GetModel_2(double d12 = 0.0014, double d21 = 0.0075)
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
