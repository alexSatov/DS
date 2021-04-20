using System.Collections.Generic;
using System.IO;
using System.Linq;
using DS.Extensions;
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
            var lines = new List<string>();

            foreach (var lcType in lcSet.Keys)
                for (var i = 0; i < lcSet[lcType].Count; i++)
                    foreach (var point in lcSet[lcType][i])
                        lines.Add($"{lcType:D} {i} {point.X1.Format()} {point.X2.Format()}");

            WriteToFile(lines, filename, dir);
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
            var lines = points
                .Select(p => $"{p.LcType:D} {p.LcIndex} {p.Index} {p.Point.X1.Format()} {p.Point.X2.Format()}");

            WriteToFile(lines, filename, dir);
        }

        public static void SaveToDir(this Dictionary<AttractorType, List<PointD>> attractors, string dir = "diagram2d")
        {
            foreach (var (type, points) in attractors)
                points.SaveToFile(type.ToString().ToLowerInvariant(), dir);
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
        }
    }
}
