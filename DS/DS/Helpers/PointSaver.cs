﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using DS.MathStructures;

namespace DS.Helpers
{
    public static class PointSaver
    {
        public static void SaveToFile(string filename, IEnumerable<(double, double)> points)
        {
            var lines = points.Select(p => $"{p.Item1.Format()} {p.Item2.Format()}");
            var directory = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(directory);
            File.WriteAllLines(filename, lines);
        }

        public static void SaveToFile(string filename, IEnumerable<(int, int, double, double)> points)
        {
            var lines = points.Select(p => $"{p.Item1} {p.Item2} {p.Item3.Format()} {p.Item4.Format()}");
            var directory = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(directory);
            File.WriteAllLines(filename, lines);
        }

        public static void SaveToFile(string filename, IEnumerable<(double, double, double)> points)
        {
            var lines = points.Select(p => $"{p.Item1.Format()} {p.Item2.Format()} {p.Item3.Format()}");
            File.WriteAllLines(filename, lines);
        }

        public static void SaveToFile(string filename,
            IEnumerable<(double, double, double, double, double, double, double)> points)
        {
            var lines = points.Select(p =>
                $"{p.Item1.Format()} {p.Item2.Format()} {p.Item3.Format()} {p.Item4.Format()} {p.Item5.Format()} {p.Item6.Format()} {p.Item7.Format()}");
            File.WriteAllLines(filename, lines);
        }

        public static void SaveToFile(string filename, IEnumerable<PointX> points)
        {
            SaveToFile(filename, points.Select(p => (p.X1, p.X2)));
        }

        public static void SaveToFile(string filename, IEnumerable<PointD> points)
        {
            SaveToFile(filename, points.Select(p => (p.D12, p.D21)));
        }
    }
}
