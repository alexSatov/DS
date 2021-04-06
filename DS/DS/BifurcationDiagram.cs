using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DS.Extensions;
using DS.MathStructures;
using DS.MathStructures.Points;
using DS.MathStructures.Vectors;
using DS.Models;

namespace DS
{
    public static class BifurcationDiagram
    {
        #region 2-Model

        public class D12VsD21Result
        {
            public List<(PointD D, PointX X)> EquilibriumPoints { get; }
            public List<PointD> InfinityPoints { get; }
            public Dictionary<int, List<(PointD D, PointX X)>> CyclePoints { get; }

            public D12VsD21Result()
            {
                EquilibriumPoints = new List<(PointD, PointX)>();
                InfinityPoints = new List<PointD>();
                CyclePoints = new Dictionary<int, List<(PointD D, PointX X)>>();

                for (var i = 2; i < 16; i++)
                {
                    CyclePoints[i] = new List<(PointD D, PointX X)>();
                }
            }
        }

        public static IEnumerable<(double D12, double X1, double X2)> GetD12VsX(Model1 model, PointX start,
            double d12End, double step)
        {
            for (; model.D12 < d12End; model.D12 += step)
            {
                var points = PhaseTrajectory.Get(model, start, 10000, 2000);

                if (points[0].IsInfinity()) continue;

                foreach (var (x1, x2) in points)
                    yield return (model.D12, x1, x2);
            }
        }

        public static IEnumerable<(double D12, double X1, double X2)> GetD12VsXByPrevious(Model1 model, PointX start,
            double d12End, double step, bool rightToLeft = false)
        {
            Func<bool> condition;

            if (rightToLeft)
            {
                condition = () => model.D12 >= d12End;
                step = -step;
            }
            else
                condition = () => model.D12 <= d12End;

            for (; condition(); model.D12 += step)
            {
                var points = PhaseTrajectory.Get(model, start, 10000, 2000);

                if (points[0].IsInfinity())
                    continue;

                start = points[^1];

                foreach (var (x1, x2) in points)
                    yield return (model.D12, x1, x2);
            }
        }

        public static IEnumerable<(double D12, double X1, double X2)> GetD12VsXByPrevious(DeterministicModel1 dModel1,
            StochasticModel1 sModel1, PointX start, double d12End, double step, bool rightToLeft = false)
        {
            var previous = start;
            var d12 = dModel1.D12;
            Func<bool> condition;

            if (rightToLeft)
            {
                condition = () => d12 >= d12End;
                step = -step;
            }
            else
                condition = () => d12 <= d12End;

            for (; condition(); d12 += step)
            {
                dModel1.D12 = d12;
                sModel1.D12 = d12;
                var attractor = PhaseTrajectory.Get(dModel1, previous, 9999, 1)[0];
                var points = PhaseTrajectory.Get(sModel1, attractor, 0, 2000);

                if (points[0].IsInfinity())
                    continue;

                previous = attractor;

                foreach (var (x1, x2) in points)
                    yield return (d12, x1, x2);
            }
        }

        public static IEnumerable<(double D12, double X1, double X2)> GetD12VsXParallel(Model1 model, PointX start,
            double d12End, double step)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<IEnumerable<(double D12, double X1, double X2)>>[processorCount];
            var d12Part = (d12End - model.D12) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var copy = (Model1) model.Copy();
                var d12PartEnd = model.D12 + d12Part * (i + 1);
                copy.D12 = model.D12 + d12Part * i;

                tasks[i] = Task.Run(() => GetD12VsX(copy, start, d12PartEnd, step));
            }

            foreach (var task in tasks)
            foreach (var values in task.Result)
                yield return values;
        }

        public static D12VsD21Result GetD12VsD21(Model1 model, PointX start, double d12End, double d21End,
            double step1, double step2)
        {
            var d21Start = model.D21;
            var result = new D12VsD21Result();

            for (; model.D12 <= d12End; model.D12 += step1)
            {
                model.D21 = d21Start;
                for (; model.D21 <= d21End; model.D21 += step2)
                    TryAddToResult(model, PhaseTrajectory.Get(model, start, 10000, 1)[0], result);
            }

            return result;
        }

        public static D12VsD21Result GetD12VsD21ByPreviousD21(Model1 model, PointX start, double d12End, double d21End,
            double step1, double step2, bool rightToLeft = false, bool upToDown = false)
        {
            var d21Start = model.D21;
            var result = new D12VsD21Result();
            Func<bool> conditionD12 = () => model.D12 <= d12End;
            Func<bool> conditionD21 = () => model.D21 <= d21End;

            if (rightToLeft)
            {
                conditionD12 = () => model.D12 >= d12End;
                step1 = -step1;
            }

            if (upToDown)
            {
                conditionD21 = () => model.D21 >= d21End;
                step2 = -step2;
            }

            for (; conditionD12(); model.D12 += step1)
            {
                model.D21 = d21Start;
                var previous = start;
                for (; conditionD21(); model.D21 += step2)
                {
                    previous = PhaseTrajectory.Get(model, previous, 10000, 1)[0];
                    TryAddToResult(model, previous, result);
                }
            }

            return result;
        }

        public static D12VsD21Result GetD12VsD21ByPreviousD12(Model1 model, PointX start, double d12End, double d21End,
            double step1, double step2)
        {
            var d12Start = model.D12;
            var result = new D12VsD21Result();

            for (; model.D21 <= d21End; model.D21 += step2)
            {
                model.D12 = d12Start;
                var previous = start;
                for (; model.D12 <= d12End; model.D12 += step1)
                {
                    previous = PhaseTrajectory.Get(model, previous, 10000, 1)[0];
                    TryAddToResult(model, previous, result);
                }
            }

            return result;
        }

        public static D12VsD21Result GetD12VsD21ParallelByD12(Model1 model, PointX start, double d12End, double d21End,
            double step1, double step2, bool rightToLeft = false, bool upToDown = false)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<D12VsD21Result>[processorCount];
            var d12Part = (d12End - model.D12) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var copy = (Model1) model.Copy();
                var d12PartEnd = model.D12 + d12Part * (i + 1);
                copy.D12 = model.D12 + d12Part * i;

                tasks[i] = Task.Run(() => GetD12VsD21ByPreviousD21(copy, start, d12PartEnd, d21End, step1, step2, rightToLeft, upToDown));
            }

            return UniteResults(tasks);
        }

        public static D12VsD21Result GetD12VsD21ParallelByD21(Model1 model, PointX start, double d12End, double d21End,
            double step1, double step2)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<D12VsD21Result>[processorCount];
            var d21Part = (d21End - model.D21) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var copy = (Model1) model.Copy();
                var d21PartEnd = model.D21 + d21Part * (i + 1);
                copy.D21 = model.D21 + d21Part * i;

                tasks[i] = Task.Run(() => GetD12VsD21ByPreviousD12(copy, start, d12End, d21PartEnd, step1, step2));
            }

            return UniteResults(tasks);
        }

        public static D12VsD21Result GetD12VsD21ByPreviousPolar(Model1 model, PointX startX, PointD startD,
            Rect dArea, double angleStep, double step1, double step2,
            double startAngle = 0, double endAngle = 2 * Math.PI)
        {
            var result = new D12VsD21Result();
            var startVector = new Vector2D(1, 0);

            for (var angle = startAngle; angle < endAngle; angle += angleStep)
            {
                var previous = startX;
                var vector = startVector.Rotate(angle);
                var shiftVector = new Vector2D(vector.X * step1, vector.Y * step2);

                model.D12 = startD.Dx;
                model.D21 = startD.Dy;

                while (dArea.Contains(model.D12, model.D21))
                {
                    model.D12 += shiftVector.X;
                    model.D21 += shiftVector.Y;
                    previous = PhaseTrajectory.Get(model, previous, 10000, 1)[0];
                    TryAddToResult(model, previous, result);
                }
            }

            return result;
        }

        public static D12VsD21Result GetD12VsD21ByPreviousPolarParallel(Model1 model, PointX startX, PointD startD,
            Rect dArea, double angleStep, double step1, double step2,
            double startAngle = 0, double endAngle = 2 * Math.PI)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<D12VsD21Result>[processorCount];
            var anglePart = (endAngle - startAngle) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var copy = (Model1) model.Copy();
                var startAnglePart = startAngle + anglePart * i;
                var endAnglePart = startAngle + anglePart * (i + 1);

                tasks[i] = Task.Run(() =>
                    GetD12VsD21ByPreviousPolar(copy, startX, startD, dArea, angleStep, step1, step2,
                        startAnglePart, endAnglePart));
            }

            return UniteResults(tasks);
        }

        private static void TryAddToResult(Model1 model, PointX point, D12VsD21Result result)
        {
            if (point.IsInfinity())
                result.InfinityPoints.Add(new PointD(model.D12, model.D21));

            var next = model.GetNextPoint(point);

            if (point.AlmostEquals(next))
            {
                result.EquilibriumPoints.Add((new PointD(model.D12, model.D21), point));
                return;
            }

            var cycle = FindCycle(model, point, next);

            if (!cycle.Found) return;

            result.CyclePoints[cycle.Period].Add((new PointD(model.D12, model.D21), point));
        }

        private static (bool Found, int Period) FindCycle(BaseModel baseModel, PointX first, PointX second)
        {
            var points = new PointX[16];
            points[0] = first;
            points[1] = second;

            for (var i = 1; i < points.Length - 1; i++)
            {
                points[i + 1] = baseModel.GetNextPoint(points[i]);

                if (points[0].AlmostEquals(points[i + 1]))
                    return (true, i + 1);
            }

            return (false, 0);
        }

        private static D12VsD21Result UniteResults(IEnumerable<Task<D12VsD21Result>> tasks)
        {
            var result = new D12VsD21Result();

            foreach (var task in tasks)
            {
                var taskResult = task.Result;

                result.EquilibriumPoints.AddRange(taskResult.EquilibriumPoints);
                result.InfinityPoints.AddRange(taskResult.InfinityPoints);

                foreach (var period in taskResult.CyclePoints.Keys)
                    result.CyclePoints[period].AddRange(taskResult.CyclePoints[period]);
            }

            return result;
        }

        #endregion

        #region N-Model

        public static List<(double D, double[] Point)> Get(NModel1 model, DParams dParams,
            double[] start, int count, int skip = 8000, int get = 2000)
        {
            return dParams.ByPrevious
                ? dParams.Interval.Range(count).SelectMany(Selector).Where(Predicate).ToList()
                : dParams.Interval.Range(count).AsParallel().SelectMany(Selector).Where(Predicate).ToList();

            IEnumerable<(double D, double[] Point)> Selector(double d)
            {
                var copy = (NModel1) model.Copy();
                copy.D[dParams.Di, dParams.Dj] = d;

                var points = PhaseTrajectory.Get(copy, start, skip, get);

                if (dParams.ByPrevious)
                    start = points[^1];

                return points.Select(p => (D: d, Point: p));
            }

            bool Predicate((double D12, double[] Point) t) => !t.Point.IsInfinity();
        }

        public static List<Attractor<double[], PointD>> Get(NModel1 model, D2Params dParams,
            double[] start, int countX, int countY, int skip = 8000, int get = 2000, double eps = 0.00001)
        {
            return dParams.ByPreviousType switch
            {
                ByPreviousType.None => X(false),
                ByPreviousType.X => X(true),
                ByPreviousType.Y => Y(),
                var byPreviousType => throw new ArgumentOutOfRangeException(nameof(byPreviousType), byPreviousType,
                    "Unknown type")
            };

            List<Attractor<double[], PointD>> X(bool byPrevious)
            {
                return dParams.IntervalX.Range(countX)
                    .AsParallel()
                    .SelectMany(dx =>
                    {
                        var current = start;
                        var copy = (NModel1) model.Copy();
                        copy.D[dParams.Dxi, dParams.Dxj] = dx;

                        return dParams.IntervalY.Range(countY).Select(dy =>
                        {
                            copy.D[dParams.Dyi, dParams.Dyj] = dy;

                            var points = PhaseTrajectory.Get(copy, current, skip, get);

                            if (byPrevious)
                                current = points[^1];

                            return Attractor.From(points, new PointD(dx, dy), eps);
                        });
                    })
                    .ToList();
            }

            List<Attractor<double[], PointD>> Y()
            {
                return dParams.IntervalY.Range(countY)
                    .AsParallel()
                    .SelectMany(dy =>
                    {
                        var current = start;
                        var copy = (NModel1) model.Copy();
                        copy.D[dParams.Dyi, dParams.Dyj] = dy;

                        return dParams.IntervalX.Range(countX).Select(dx =>
                        {
                            copy.D[dParams.Dxi, dParams.Dxj] = dx;

                            var points = PhaseTrajectory.Get(copy, current, skip, get);
                            current = points[^1];

                            return Attractor.From(points, new PointD(dx, dy), eps);
                        });
                    })
                    .ToList();
            }
        }

        #endregion
    }
}
