using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using DS.MathStructures.Points;
using DS.Models;
using NUnit.Framework;

namespace DS.Tests.Charts.Model2
{
    public class DeterministicModel2Tests : ChartTests
    {
        private DeterministicModel2 model;

        protected override void OnSetUp()
        {
            model = new DeterministicModel2(0, 0.3);
        }

        /// <summary>
        /// Базовые аттракторы модели
        /// </summary>
        [Test]
        public void Test1()
        {
            var model1 = model;
            model1.A = 0.5;
            var model2 = (DeterministicModel2) model.Copy();
            model2.A = 0.61;
            var model3 = (DeterministicModel2) model.Copy();
            model3.A = 0.7;
            var model4 = (DeterministicModel2) model.Copy();
            model4.A = 1.01;

            var points1 = PhaseTrajectory.Get(model1, new PointX(0.5, 0.5), 2000, 1000);
            var points2 = PhaseTrajectory.Get(model2, new PointX(0.5, 0.5), 2000, 1000);
            var points3 = PhaseTrajectory.Get(model3, new PointX(0.5, 0.5), 2000, 1000);
            var points4 = PhaseTrajectory.Get(model4, new PointX(0.5, 0.5), 2000, 1000);

            var chart = new ChartForm(points1, -0.5, 1.5, -0.5, 1.5, Color.Black, null, 5);
            chart.AddSeries(nameof(points2), points2, Color.DodgerBlue);
            chart.AddSeries(nameof(points3), points3, Color.Red);
            chart.AddSeries(nameof(points4), points4, Color.Green, SeriesChartType.Line, 5, 1);

            Chart = chart;
        }

        /// <summary>
        /// Хаос
        /// </summary>
        [Test]
        public void Test2()
        {
            model.A = 1.1;

            var points = PhaseTrajectory.Get(model, new PointX(0.5, 0.5), 5000, 100000);

            Chart = new ChartForm(points, -2, 2, -2, 2);
        }

        /// <summary>
        /// Построение критических линий (для хаоса выше)
        /// </summary>
        [Test]
        public void Test3()
        {
            const int markerSize = 6;
            model.A = 1.1;

            var attractor = PhaseTrajectory.Get(model, new PointX(0.5, 0.5), 5000, 100000);
            var lcList = LcSet.FromAttractor(model, attractor, 5, eps: 0.001)[LcType.H];
            var chart = new ChartForm(attractor, -1, 1.5, -1, 1.5);

            chart.AddSeries("lc0", lcList[0], Color.Red, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc1", lcList[1], Color.Black, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc2", lcList[2], Color.Green, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc3", lcList[3], Color.DarkViolet, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc4", lcList[4], Color.DeepPink, SeriesChartType.Line, markerSize);
            chart.AddSeries("lc5", lcList[5], Color.SaddleBrown, SeriesChartType.Line, markerSize);

            Chart = chart;
        }

        /// <summary>
        /// Построение границы (для хаоса выше)
        /// </summary>
        [Test]
        public void Test4()
        {
            model.A = 1.1;

            var i = 0;
            var attractor = PhaseTrajectory.Get(model, new PointX(0.5, 0.5), 5000, 100000);
            var lcSet = LcSet.FromAttractor(model, attractor, 5, eps: 0.001);
            var borderSegments = lcSet.GetBorderSegments(true, true);

            var chart = new ChartForm(attractor, -1, 1.5, -1, 1.5);

            foreach (var borderSegment in borderSegments)
                chart.AddSeries($"border{i++}", borderSegment.GetBoundaryPoints(), Color.Red,
                    SeriesChartType.FastLine);

            Chart = chart;
        }
    }
}
