using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DS
{
	public class ChartForm : Form
	{
		public Chart Chart { get; }
		public ChartArea ChartArea { get; }

		public readonly Dictionary<string, int> SeriesPointCount = new Dictionary<string, int>();

		public ChartForm(IEnumerable<(double x, double y)> points, double ox1, double ox2, double oy1, double oy2,
			Color? color = null)
		{
			Size = new Size(700, 600);
			Chart = new Chart { Parent = this, Dock = DockStyle.Fill };
			ChartArea = new ChartArea
			{
				AxisX = { Minimum = ox1, Maximum = ox2 },
				AxisY = { Minimum = oy1, Maximum = oy2 }
			};

			Chart.ChartAreas.Add(ChartArea);
			AddSeries("main", points, color ?? Color.DodgerBlue);
		}

		public void AddSeries(string name, IEnumerable<(double x, double y)> points, Color? color = null)
		{
			SeriesPointCount[name] = 0;
			var series = new Series
			{
				Name = name,
				ChartType = SeriesChartType.FastPoint,
				ChartArea = ChartArea.Name,
				MarkerStyle = MarkerStyle.Circle
			};

			if (color.HasValue)
				series.Color = color.Value;

			foreach (var (x, y) in points)
			{
				SeriesPointCount[name]++;
				series.Points.AddXY(x, y);
			}

			Chart.Series.Add(series);
		}
	}
}
