using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DS.Helpers;
using DS.MathStructures;

namespace DS
{
    public static class AttractorPoolChart
    {
        // d12 = 0.00145 - ЗИК и 3х цикл: (6, 62); (17, 44); (24, 75)
        // d12 = 0.00197 - равновесие и 3х цикл: (12, 65); (20, 62); (23, 72)
        // d12 = 0.00217 - равновесие и равновесие: (18, 68) и (35, 44)
        // d12 = 0.002382 - хаос и равновесие: (20, 68)
        public static ChartForm GetAttractorPoolChart0(Dictionary<PointX, HashSet<PointX>> attractorPoints,
            double ox1, double ox2, double oy1, double oy2)
        {
            var eqAttractor = new HashSet<PointX> { new PointX(20, 68) };

            var eq = attractorPoints
                .Where(kv => eqAttractor.Contains(kv.Key))
                .SelectMany(kv => kv.Value)
                .ToList();

            var chaos = attractorPoints
                .Where(kv => !eqAttractor.Contains(kv.Key))
                .SelectMany(kv => kv.Value)
                .ToList();

            var chaosAttractors = attractorPoints.Keys
                .Where(p => !eqAttractor.Contains(p))
                .ToList();

            var chart = new ChartForm(eq, ox1, ox2, oy1, oy2, markerSize: 4);
            chart.AddSeries(nameof(chaos), chaos, Color.DarkSeaGreen, 4);
            chart.AddSeries(nameof(chaosAttractors), chaosAttractors, Color.GreenYellow, 5);
            chart.AddSeries(nameof(eqAttractor), eqAttractor, Color.Blue, 5);

            PointSaver.SaveToFile($"pool_simple\\{nameof(eqAttractor)}.txt", eqAttractor);
            PointSaver.SaveToFile($"pool_simple\\{nameof(chaosAttractors)}.txt", chaosAttractors);
            PointSaver.SaveToFile($"pool_simple\\{nameof(eq)}.txt", eq);
            PointSaver.SaveToFile($"pool_simple\\{nameof(chaos)}.txt", chaos);

            return chart;
        }

        /// <summary>
        /// Бассейн. d12 = 0.001909 - равновесие (17, 67) и 3х цикл { (21, 59), (24, 73), (11, 63) }
        /// </summary>
        public static ChartForm GetAttractorPoolChart1(Dictionary<PointX, HashSet<PointX>> attractorPoints,
            double ox1, double ox2, double oy1, double oy2)
        {
            var eqAttractor = new HashSet<PointX> { new PointX(17, 67) };
            var cycle3Attractor = new HashSet<PointX> { new PointX(21, 59), new PointX(24, 73), new PointX(11, 63) };

            var eq = attractorPoints
                .Where(kv => eqAttractor.Contains(kv.Key))
                .SelectMany(kv => kv.Value)
                .ToList();

            var cycle3 = attractorPoints
                .Where(kv => !eqAttractor.Contains(kv.Key))
                .SelectMany(kv => kv.Value)
                .ToList();

            var chart = new ChartForm(eq, ox1, ox2, oy1, oy2, markerSize: 4);
            chart.AddSeries(nameof(cycle3), cycle3, Color.OrangeRed, 4);
            chart.AddSeries(nameof(eq), eq, Color.DodgerBlue, 4);
            chart.AddSeries(nameof(eqAttractor), eqAttractor, Color.Blue, 8);
            chart.AddSeries(nameof(cycle3Attractor), cycle3Attractor, Color.Red, 8);

            return chart;
        }

        /// <summary>
        /// Бассейн. d12 = 0.001974 - равновесие (17, 68) и 3х цикл { (20, 62), (23, 72), (13, 65) }
        /// </summary>
        public static ChartForm GetAttractorPoolChart2(Dictionary<PointX, HashSet<PointX>> attractorPoints,
            double ox1, double ox2, double oy1, double oy2)
        {
            var eqAttractor = new HashSet<PointX> { new PointX(17, 68) };
            var cycle3Attractor = new HashSet<PointX> { new PointX(20, 62), new PointX(23, 72), new PointX(13, 65) };

            var eq = attractorPoints
                .Where(kv => eqAttractor.Contains(kv.Key))
                .SelectMany(kv => kv.Value)
                .ToList();

            var cycle3 = attractorPoints
                .Where(kv => !eqAttractor.Contains(kv.Key))
                .SelectMany(kv => kv.Value)
                .ToList();

            var chart = new ChartForm(eq, ox1, ox2, oy1, oy2, markerSize: 4);
            chart.AddSeries(nameof(cycle3), cycle3, Color.OrangeRed, 4);
            chart.AddSeries(nameof(eq), eq, Color.DodgerBlue, 4);
            chart.AddSeries(nameof(eqAttractor), eqAttractor, Color.Blue, 8);
            chart.AddSeries(nameof(cycle3Attractor), cycle3Attractor, Color.Red, 8);

            return chart;
        }
    }
}
