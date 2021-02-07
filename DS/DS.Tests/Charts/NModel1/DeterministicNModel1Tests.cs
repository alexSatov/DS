using System;
using System.Linq;
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

            var points = PhaseTrajectory.Get(model, new double[] { 20, 40 }, 2000, 1000)
                .Select(p => (p[0], p[1]));

            Chart = new ChartForm(points, 0, 40, 0, 80);
        }

        private static DeterministicNModel1 GetModel_2()
        {
            throw new NotImplementedException();
        }

        protected override void OnSetUp()
        {
        }
    }
}
