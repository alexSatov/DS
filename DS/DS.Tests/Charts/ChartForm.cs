﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DS.MathStructures.Points;

namespace DS.Tests.Charts
{
    public class ChartForm : Form
    {
        public Chart Chart { get; }
        public ChartArea ChartArea { get; }

        public ChartForm(IEnumerable<(double x, double y)> points, double ox1, double ox2, double oy1, double oy2,
            Color? color = null, string name = null, int markerSize = 2, bool gridEnabled = false,
            int borderWidth = 2, SeriesChartType seriesChartType = SeriesChartType.FastPoint)
        {
            Size = new Size(700, 600);
            Chart = new Chart { Parent = this, Dock = DockStyle.Fill };
            ChartArea = new ChartArea
            {
                AxisX = { Minimum = ox1, Maximum = ox2 },
                AxisY = { Minimum = oy1, Maximum = oy2 }
            };

            if (!gridEnabled)
            {
                ChartArea.AxisX.MajorGrid.Enabled = false;
                ChartArea.AxisY.MajorGrid.Enabled = false;
            }

            ChartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(50, Color.Black);
            ChartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(50, Color.Black);

            Chart.ChartAreas.Add(ChartArea);
            AddSeries(name ?? "main", points, color ?? Color.DodgerBlue, markerSize: markerSize,
                seriesChartType: seriesChartType, borderWidth: borderWidth);
        }

        public ChartForm(IEnumerable<PointX> points, double ox1, double ox2, double oy1, double oy2,
            Color? color = null, string name = null, int markerSize = 2, bool gridEnabled = false)
            : this(points.Select(p => (p.X1, p.X2)), ox1, ox2, oy1, oy2, color, name, markerSize, gridEnabled)
        {
        }

        public ChartForm(IEnumerable<PointD> points, double ox1, double ox2, double oy1, double oy2,
            Color? color = null, string name = null, int markerSize = 2, bool gridEnabled = false)
            : this(points.Select(p => (D12: p.Dx, D21: p.Dy)), ox1, ox2, oy1, oy2, color, name, markerSize, gridEnabled)
        {
        }

        public void AddSeries(string name, IEnumerable<(double x, double y)> points, Color? color = null,
            SeriesChartType seriesChartType = SeriesChartType.FastPoint, int markerSize = 2, int borderWidth = 2)
        {
            var seriesPointCount = 0;
            var series = new Series
            {
                Name = name,
                ChartType = seriesChartType,
                ChartArea = ChartArea.Name,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = markerSize,
                BorderWidth = borderWidth
            };

            if (color.HasValue)
                series.Color = color.Value;

            foreach (var (x, y) in points)
            {
                seriesPointCount++;
                series.Points.AddXY(x, y);
            }

            Console.WriteLine($"'{name}' points count: {seriesPointCount}");
            Chart.Series.Add(series);
        }

        public void AddSeries<T>(string name, IEnumerable<T> points, Color? color = null,
            SeriesChartType seriesChartType = SeriesChartType.FastPoint, int markerSize = 2, int borderWidth = 2)
            where T : IPoint
        {
            AddSeries(name, points.Select(p => (p.X, p.Y)), color, seriesChartType, markerSize, borderWidth);
        }
    }
}
