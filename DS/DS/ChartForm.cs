using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DS
{
	public class ChartForm : Form
	{
		public int PointsCount { get; }

		public ChartForm(IEnumerable<(double x, double y)> points, double ox1, double ox2, double oy1, double oy2)
		{
			Size = new Size(900, 600);

			var chart = new Chart { Parent = this, Dock = DockStyle.Fill };
			var chartArea = new ChartArea
			{
				AxisX = { Minimum = ox1, Maximum = ox2 },
				AxisY = { Minimum = oy1, Maximum = oy2 }
			};

			chart.ChartAreas.Add(chartArea);

			var series = new Series
			{
				Name = "model",
				ChartType = SeriesChartType.FastPoint,
				ChartArea = chartArea.Name,
				MarkerStyle = MarkerStyle.Circle
			};

			foreach (var (x, y) in points)
			{
				PointsCount++;
				series.Points.AddXY(x, y);
			}

			chart.Series.Add(series);
		}
	}
}
