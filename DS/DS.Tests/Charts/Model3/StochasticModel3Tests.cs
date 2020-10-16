using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using DS.Helpers;
using DS.MathStructures.Points;
using DS.Models;
using NUnit.Framework;

namespace DS.Tests.Charts.Model3
{
    public class StochasticModel3Tests : ChartTests
    {
        private DeterministicModel3 dModel;
        private StochasticModel3 sModel;

        protected override void OnSetUp()
        {
            dModel = new DeterministicModel3(4, 1, 0.2, 3.3);
            sModel = new StochasticModel3(4, 1, 0.2, 3.3, 0.005, 1, 1);
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
            var lcSet = LcSet.FromAttractor(dModel, attractor, 10, eps: 0.01);
            var borderSegments = lcSet.GetBorderSegments2(false, true);
            var ellipse = ScatterEllipse.GetForChaosLc(dModel, lcSet, sModel.Eps).ToList();

            Chart = new ChartForm(attractor2, 0.15, 0.7, 0.5, 2.4);

            foreach (var borderSegment in borderSegments)
                Chart.AddSeries($"border_{i++}", borderSegment.GetBoundaryPoints(), Color.Red, SeriesChartType.FastLine);

            Chart.AddSeries("ellipse", ellipse.Select(t => t.Point), Color.Green, markerSize: 4);

            attractor2.SaveToFile("model3\\chaos_with_noise.txt");
            ellipse.SaveToFile("model3\\ellipse.txt");
            lcSet.SaveToFile(ellipse, "model3\\LC_border.txt");
        }
    }
}
