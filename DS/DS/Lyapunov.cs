using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accord.Math;
using DS.MathStructures.Points;
using DS.MathStructures.Vectors;
using DS.Models;
using Matrix = Accord.Math.Matrix;

namespace DS
{
    public static class Lyapunov
    {
        /// <summary>
        /// Решение матричного уравнения X = A X AT + B
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="n">Количество итераций</param>
        /// <returns></returns>
        public static double[,] SolveDiscreteEquation(double[,] a, double[,] b, int n = 400)
        {
            var x = Matrix.Identity(2);
            var at = a.Transpose();

            for (var i = 0; i < n; i++)
                x = a.Dot(x).Dot(at).Add(b);

            return x;
        }

        public static IEnumerable<(double D12, double L1, double L2)> GetD12Indicators(Model1 model, PointX start,
            double d12End, double step, bool byPrevious = false, double eps = 0.00001, double t = 100000)
        {
            var result = new List<(double D12, double L1, double L2)>();
            var previous = start;

            for (; model.D12 < d12End; model.D12 += step)
            {
                var (o, l1, l2) = FindL1L2(model, start, previous, byPrevious, eps, t);
                previous = o;

                result.Add((model.D12, l1, l2));
            }

            return result;
        }

        public static IEnumerable<(double D12, double L1, double L2)> GetD12IndicatorsParallel(Model1 model, PointX start,
            double d12End, double step, double eps = 0.00001, double t = 100000)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<IEnumerable<(double D12, double L1, double L2)>>[processorCount];
            var d12Part = (d12End - model.D12) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var copy = (Model1) model.Copy();
                var d12PartEnd = model.D12 + d12Part * (i + 1);
                copy.D12 = model.D12 + d12Part * i;

                tasks[i] = Task.Run(() => GetD12Indicators(copy, start, d12PartEnd, step, false, eps, t));
            }

            foreach (var task in tasks)
                foreach (var values in task.Result)
                    yield return values;
        }

        public static IEnumerable<(double D12, double D21, double L1, double L2)> GetIndicatorsByD12(Model1 model, PointX start,
            double d12End, double d21End, double step1, double step2, bool byPrevious = false,
            double eps = 0.00001, double t = 100000)
        {
            var result = new List<(double D12, double D21, double L1, double L2)>();
            var d12Start = model.D12;

            for (; model.D21 < d21End; model.D21 += step2)
            {
                var previous = start;
                model.D12 = d12Start;

                for (; model.D12 < d12End; model.D12 += step1)
                {
                    var (o, l1, l2) = FindL1L2(model, start, previous, byPrevious, eps, t);
                    previous = o;

                    result.Add((model.D12, model.D21, l1, l2));
                }
            }

            return result;
        }

        public static IEnumerable<(double D12, double D21, double L1, double L2)> GetIndicatorsByD21(Model1 model, PointX start,
            double d12End, double d21End, double step1, double step2, bool byPrevious = false,
            double eps = 0.00001, double t = 100000)
        {
            var result = new List<(double D12, double D21, double L1, double L2)>();
            var d21Start = model.D21;

            for (; model.D12 < d12End; model.D12 += step1)
            {
                var previous = start;
                model.D21 = d21Start;

                for (; model.D21 < d21End; model.D21 += step2)
                {
                    var (o, l1, l2) = FindL1L2(model, start, previous, byPrevious, eps, t);
                    previous = o;

                    result.Add((model.D12, model.D21, l1, l2));
                }
            }

            return result;
        }

        public static IEnumerable<(double D12, double D21, double L1, double L2)> GetIndicatorsParallelByD12(Model1 model, PointX start,
            double d12End, double d21End, double step1, double step2, double eps = 0.00001, double t = 100000)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<IEnumerable<(double D12, double D21, double L1, double L2)>>[processorCount];
            var d12Part = (d12End - model.D12) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var copy = (Model1) model.Copy();
                var d12PartEnd = model.D12 + d12Part * (i + 1);
                copy.D12 = model.D12 + d12Part * i;

                tasks[i] = Task.Run(() => GetIndicatorsByD21(copy, start, d12PartEnd, d21End, step1, step2, true, eps, t));
            }

            foreach (var task in tasks)
                foreach (var values in task.Result)
                    yield return values;
        }

        public static IEnumerable<(double D12, double D21, double L1, double L2)> GetIndicatorsParallelByD21(Model1 model, PointX start,
            double d12End, double d21End, double step1, double step2, double eps = 0.00001, double t = 100000)
        {
            var processorCount = Environment.ProcessorCount;
            var tasks = new Task<IEnumerable<(double D12, double D21, double L1, double L2)>>[processorCount];
            var d21Part = (d21End - model.D21) / processorCount;

            for (var i = 0; i < processorCount; i++)
            {
                var copy = (Model1) model.Copy();
                var d21PartEnd = model.D21 + d21Part * (i + 1);
                copy.D21 = model.D21 + d21Part * i;

                tasks[i] = Task.Run(() => GetIndicatorsByD12(copy, start, d12End, d21PartEnd, step1, step2, true, eps, t));
            }

            foreach (var task in tasks)
                foreach (var values in task.Result)
                    yield return values;
        }

        private static (PointX O, double L1, double L2) FindL1L2(BaseModel baseModel, PointX start, PointX previous,
            bool byPrevious, double eps, double t)
        {
            var o = byPrevious ? previous : start;
            var a = new PointX(o.X1 + eps, o.X2);
            var b = new PointX(o.X1, o.X2 + eps);

            double z1 = 0, z2 = 0;

            for (var i = 0; i < t; i++)
            {
                var (p, p1, p2) = (baseModel.GetNextPoint(o), baseModel.GetNextPoint(a), baseModel.GetNextPoint(b));
                var (v1, v2) = (new Vector2D(p1, p), new Vector2D(p2, p));

                var b1 = v1 / v1.Length;
                var b2 = v2 - v2 * b1 * b1;

                z1 += Math.Log10(v1.Length / eps);
                z2 += Math.Log10(b2.Length / eps);

                b1 *= eps;
                b2 = b2 / b2.Length * eps;

                o = p;
                a = new PointX(b1.X + p.X, b1.Y + p.Y);
                b = new PointX(b2.X + p.X, b2.Y + p.Y);
            }

            var (l1, l2) = (z1 / t, z2 / t);

            l1 = Math.Abs(l1) < 0.001 ? 0 : l1;
            l2 = Math.Abs(l2) < 0.001 ? 0 : l2;

            return (o, l1, l2);
        }
    }
}
