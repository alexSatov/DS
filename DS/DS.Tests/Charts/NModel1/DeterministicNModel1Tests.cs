using System.Linq;
using DS.MathStructures;
using DS.Models;
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
            var model = GetModel_2();
            var interval = new Interval<double>(0.0017, 0.0025);

            var points = BifurcationDiagram.GetD12VsX(model, interval, new[] { 0.25, 0.125 }, 1000)
                .Select(p => (p.D12, p.X[0]));

            Chart = new ChartForm(points, 0.0017, 0.0025, 0, 1);
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
