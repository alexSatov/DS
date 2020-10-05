using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using DS.Helpers;
using DS.MathStructures.Points;
using DS.Models;
using NUnit.Framework;

namespace DS.Tests.Charts.Model3
{
    public class DeterministicModel3Tests : ChartTests
    {
        private DeterministicModel3 model;

        protected override void OnSetUp()
        {
            model = new DeterministicModel3(4, 1, 0.2, 3.3);
        }

        /// <summary>
        /// Хаос
        /// </summary>
        [Test]
        public void Test1()
        {
            var points = PhaseTrajectory.Get(model, new PointX(0.5, 1), 5000, 100000);

            Chart = new ChartForm(points, 0.15, 0.7, 0.5, 2.4);
        }

        /// <summary>
        /// Построение критических линий (для хаоса выше)
        /// </summary>
        [Test]
        public void Test2()
        {
            const int markerSize = 6;

            var attractor = PhaseTrajectory.Get(model, new PointX(0.5, 0.5), 5000, 100000);
            var lcSet = LcSet.FromAttractor(model, attractor, 10, eps: 0.01);
            var lcList = lcSet[LcType.H];
            var chart = new ChartForm(attractor, 0.15, 0.7, 0.5, 2.4);

            chart.AddSeries("lc0", lcList[0], Color.Red, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc1", lcList[1], Color.Black, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc2", lcList[2], Color.Green, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc3", lcList[3], Color.DarkViolet, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc4", lcList[4], Color.DeepPink, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc5", lcList[5], Color.SaddleBrown, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc6", lcList[6], Color.Gold, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc7", lcList[7], Color.Coral, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc8", lcList[8], Color.Aqua, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc9", lcList[9], Color.Purple, SeriesChartType.Line, markerSize);

            attractor.SaveToFile("model3\\chaos.txt");
            lcSet.SaveToFile("model3\\LC.txt");

            Chart = chart;
        }

        /// <summary>
        /// Построение границы (для хаоса выше)
        /// </summary>
        [Test]
        public void Test3()
        {
            var i = 0;
            var attractor = PhaseTrajectory.Get(model, new PointX(0.5, 0.5), 5000, 100000);
            var lcSet = LcSet.FromAttractor(model, attractor, 10, eps: 0.01);
            var borderSegments = lcSet.GetBorderSegments2(false, true);

            var chart = new ChartForm(attractor, 0.15, 0.7, 0.5, 2.4);

            foreach (var borderSegment in borderSegments)
                chart.AddSeries($"border{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.FastLine);

            Chart = chart;
        }
    }
}
