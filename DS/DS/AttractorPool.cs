using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DS
{
    public static class AttractorPool
    {
        public static Dictionary<PointX, HashSet<PointX>> GetX1VsX2(Model model, PointX leftBottom, PointX rightTop,
            double step1, double step2)
        {
            var result = new Dictionary<PointX, HashSet<PointX>>();

            for (var x1 = leftBottom.X1; x1 <= rightTop.X1; x1 += step1)
                for (var x2 = leftBottom.X2; x2 <= rightTop.X2; x2 += step2)
                {
                    var startPoint = new PointX(x1, x2);
                    var point = PhaseTrajectory.Get(model, startPoint, 10000, 1)[0];

                    if (point.IsInfinity())
                        continue;

                    var key = new PointX((int)Math.Round(point.X1), (int)Math.Round(point.X2));
                    var found = false;

                    foreach (var attractor in result.Keys)
                    {
                        found = key.AlmostEquals(attractor, 0);

                        if (found)
                        {
                            result[key].Add(startPoint);
                            break;
                        }
                    }

                    if (!found)
                        result[key] = new HashSet<PointX> { startPoint };
                }

            foreach (var attractor in result.Keys.ToList())
            {
                if (result[attractor].Count == 1)
                    result.Remove(attractor);
            }

            return result;
        }

        public static Dictionary<PointX, HashSet<PointX>> GetX1VsX2Parallel(Model model, PointX leftBottom,
            PointX rightTop, double step1, double step2)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<Dictionary<PointX, HashSet<PointX>>>[processorCount];
            var x1Part = (rightTop.X1 - leftBottom.X1) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var leftBottomPart = new PointX(leftBottom.X1 + x1Part * i, leftBottom.X2);
                var rightTopPart = new PointX(leftBottom.X1 + x1Part * (i + 1), rightTop.X2);

                tasks[i] = Task.Run(() => GetX1VsX2(model, leftBottomPart, rightTopPart, step1, step2));
            }

            return tasks
                .SelectMany(t => t.Result)
                .ToLookup(g => g.Key, g => g.Value)
                .ToDictionary(g => g.Key, g => g.SelectMany(v => v).ToHashSet());
        }

        public static (IList<PointX> First, IList<PointX> Second) GetPoolFor2Attractors(Model model,
            Func<IList<PointX>, bool> isFirstAttractor, PointX leftBottom, PointX rightTop, double step1, double step2)
        {
            var first = new List<PointX>();
            var second = new List<PointX>();

            for (var x1 = leftBottom.X1; x1 <= rightTop.X1; x1 += step1)
            for (var x2 = leftBottom.X2; x2 <= rightTop.X2; x2 += step2)
            {
                var startPoint = new PointX(x1, x2);
                var attractor = PhaseTrajectory.Get(model, startPoint, 19000, 1000);

                if (isFirstAttractor(attractor))
                    first.Add(startPoint);
                else
                    second.Add(startPoint);
            }

            return (first, second);
        }

        public static (IList<PointX> First, IList<PointX> Second) GetPoolFor2AttractorsParallel(Model model,
            Func<IList<PointX>, bool> isFirstAttractor, PointX leftBottom, PointX rightTop, double step1, double step2)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<(IList<PointX> First, IList<PointX> Second)>[processorCount];
            var x1Part = (rightTop.X1 - leftBottom.X1) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var leftBottomPart = new PointX(leftBottom.X1 + x1Part * i, leftBottom.X2);
                var rightTopPart = new PointX(leftBottom.X1 + x1Part * (i + 1), rightTop.X2);

                tasks[i] = Task.Run(() => GetPoolFor2Attractors(model, isFirstAttractor, leftBottomPart, rightTopPart, step1, step2));
            }

            return (tasks.SelectMany(t => t.Result.First).ToList(), tasks.SelectMany(t => t.Result.Second).ToList());
        }
    }
}
