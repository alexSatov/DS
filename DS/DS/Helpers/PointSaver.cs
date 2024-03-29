﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DS.Extensions;
using DS.MathStructures;
using DS.MathStructures.Points;

namespace DS.Helpers
{
    public static class PointSaver
    {
        public static void SaveToFile(this IEnumerable<(double, double)> points, string filename, string dir = null)
        {
            var lines = points.Select(p => $"{p.Item1.Format()} {p.Item2.Format()}");
            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile(this IEnumerable<(int, int, double, double)> points, string filename, string dir = null)
        {
            var lines = points.Select(p => $"{p.Item1} {p.Item2} {p.Item3.Format()} {p.Item4.Format()}");
            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile(this IEnumerable<(double, double, double)> points, string filename, string dir = null)
        {
            var lines = points.Select(p => $"{p.Item1.Format()} {p.Item2.Format()} {p.Item3.Format()}");
            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile(this IEnumerable<(double, double, double, double, double, double, double)> points,
            string filename, string dir = null)
        {
            var lines = points.Select(p =>
                $"{p.Item1.Format()} {p.Item2.Format()} {p.Item3.Format()} {p.Item4.Format()} {p.Item5.Format()} {p.Item6.Format()} {p.Item7.Format()}");

            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile<T>(this IEnumerable<T> points, string filename, string dir = null)
            where T : IPoint
        {
            var lines = points.Select(p => $"{p.X.Format()} {p.Y.Format()}");
            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile(this LcSet lcSet, string filename, string dir = null)
        {
            foreach (var type in lcSet.Keys)
            {
                for(var i = 0; i < lcSet[type].Count; i++)
                {
                    var lines = lcSet[type][i].Select(p => $"{p.X1.Format()} {p.X2.Format()}");
                    WriteToFile(lines, $"{filename}_type{type:D}_lc{i}", dir);
                }
            }
        }

        public static void SaveToFile(this LcSet lcSet, IEnumerable<LcPoint> points, string filename, string dir = null)
        {
            var lines = new List<string>();

            foreach (var point in points)
            {
                var (x1, x2) = lcSet[point.LcType][point.LcIndex][point.Index];
                lines.Add($"{point.LcType:D} {point.LcIndex} {x1.Format()} {x2.Format()}");
            }

            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile(this IEnumerable<LcPoint> points, string filename, string dir = null)
        {
            if (filename.EndsWith(".txt"))
                filename = filename[..^4];

            var groups = points
                .GroupBy(p => p.LcType)
                .ToDictionary(g => g.Key, g => g.GroupBy(p => p.LcIndex));

            foreach (var typeGroup in groups)
            foreach (var lcGroup in typeGroup.Value)
            {
                var lines = lcGroup
                    .OrderBy(p => p.Index)
                    .Select(p => $"{p.Point.X1.Format()} {p.Point.X2.Format()}");

                WriteToFile(lines, $"{filename}_type{typeGroup.Key:D}_lc{lcGroup.Key}", dir);
            }
        }

        public static void SaveToDir(this Dictionary<AttractorType, List<PointD>> attractors, string dir = "diagram2d")
        {
            foreach (var (type, points) in attractors)
                points.SaveToFile(type.ToString(), dir);
        }

        public static void SaveToFile(this IEnumerable<double[]> points, string filename, string dir = null)
        {
            var lines = points.Select(p => string.Join(' ', p.Select(v => v.Format())));
            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile(this IEnumerable<(double, double[])> points, string filename, string dir = null)
        {
            var lines = points
                .Select(p => $"{p.Item1.Format()} {string.Join(' ', p.Item2.Select(v => v.Format()))}");

            WriteToFile(lines, filename, dir);
        }

        public static void SaveToFile(this IEnumerable<Segment> segments, string filename, string dir = null)
        {
            var lines = segments
                .Select(s => s.GetBoundaryPoints().ToList())
                .Select(p => $"{p[0].X1.Format()} {p[0].X2.Format()} {p[1].X1.Format()} {p[1].X2.Format()}");

            WriteToFile(lines, filename, dir);
        }

        private static void WriteToFile(IEnumerable<string> lines, string filename, string subdir = null)
        {
            filename = subdir == null
                ? Path.Combine("..", "data", filename)
                : Path.Combine("..", "data", subdir, filename);

            if (!filename.EndsWith(".txt"))
                filename += ".txt";

            var directory = Path.GetDirectoryName(filename);

            Directory.CreateDirectory(directory);
            File.WriteAllLines(filename, lines);

            Console.WriteLine($"Saved file '{filename}'");
        }
    }
}
