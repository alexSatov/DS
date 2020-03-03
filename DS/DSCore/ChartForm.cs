﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DS
{
    public class ChartForm : Form
    {
        public Chart Chart { get; }
        public ChartArea ChartArea { get; }

        public ChartForm(IEnumerable<(double x, double y)> points, double ox1, double ox2, double oy1, double oy2,
            Color? color = null, string name = null, int markerSize = 2)
        {
            Size = new Size(700, 600);
            Chart = new Chart { Parent = this, Dock = DockStyle.Fill };
            ChartArea = new ChartArea
            {
                AxisX = { Minimum = ox1, Maximum = ox2 },
                AxisY = { Minimum = oy1, Maximum = oy2 }
            };

            Chart.ChartAreas.Add(ChartArea);
            AddSeries(name ?? "main", points, color ?? Color.DodgerBlue, markerSize);
        }

        public ChartForm(IEnumerable<PointX> points, double ox1, double ox2, double oy1, double oy2,
            Color? color = null, string name = null, int markerSize = 2)
            : this(points.Select(p => (p.X1, p.X2)), ox1, ox2, oy1, oy2, color, name, markerSize)
        {
        }

        public ChartForm(IEnumerable<PointD> points, double ox1, double ox2, double oy1, double oy2,
            Color? color = null, string name = null, int markerSize = 2)
            : this(points.Select(p => (p.D12, p.D21)), ox1, ox2, oy1, oy2, color, name, markerSize)
        {
        }

        public void AddSeries(string name, IEnumerable<(double x, double y)> points, Color? color = null,
            int markerSize = 2, SeriesChartType seriesChartType = SeriesChartType.FastPoint)
        {
            var seriesPointCount = 0;
            var series = new Series
            {
                Name = name,
                ChartType = seriesChartType,
                ChartArea = ChartArea.Name,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = markerSize
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

        public void AddSeries(string name, IEnumerable<PointX> points, Color? color = null, int markerSize = 2,
            SeriesChartType seriesChartType = SeriesChartType.FastPoint)
        {
            AddSeries(name, points.Select(p => (p.X1, p.X2)), color, markerSize, seriesChartType);
        }

        public void AddSeries(string name, IEnumerable<PointD> points, Color? color = null, int markerSize = 2,
            SeriesChartType seriesChartType = SeriesChartType.FastPoint)
        {
            AddSeries(name, points.Select(p => (p.D12, p.D21)), color, markerSize, seriesChartType);
        }
    }
}
