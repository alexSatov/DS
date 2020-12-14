using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using DS.Helpers;
using DS.MathStructures.Points;
using DS.Models;
using NUnit.Framework;

namespace DS.Tests.Charts.Model2
{
    public class StochasticModel2Tests : ChartTests
    {
        private DeterministicModel2 dModel;
        private StochasticModel2 sModel;

        protected override void OnSetUp()
        {
            dModel = new DeterministicModel2(1.1, 0.3);
            sModel = new StochasticModel2(1.1, 0.3, 0.02);
        }

        /// <summary>
        /// Хаос с внешней границей и "эллипсом"
        /// </summary>
        [Test]
        public void Test1()
        {
            var i = 0;
            var attractor = PhaseTrajectory.Get(dModel, new PointX(0.5, 0.5), 5000, 100000);
            var attractor2 = PhaseTrajectory.Get(sModel, new PointX(0.5, 0.5), 5000, 100000);
            var lcSet = LcSet.FromAttractor(dModel, attractor, 5, eps: 0.001);
            var borderSegments = lcSet.GetBorderSegments(true, true);
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, sModel.Eps).ToList();

            Chart = new ChartForm(attractor2, -1.25, 1.5, -1.4, 1.5);

            foreach (var borderSegment in borderSegments)
                Chart.AddSeries($"border{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.FastLine, borderWidth: 4);

            Chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, markerSize: 6);

            // attractor.SaveToFile("model2\\chaos.txt");
            // attractor2.SaveToFile("model2\\chaos_with_noise.txt");
            // ellipse.SaveToFile("model2\\ellipse.txt");
            // lcSet.SaveToFile("model2\\LC.txt");
            // lcSet.SaveToFile(ellipse, "model2\\LC_border.txt");
        }
    }
}
