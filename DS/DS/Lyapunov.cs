using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using DS.Extensions;
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

        #region 2-Model

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
                var (v1, v2) = (new Vector2D(p, p1), new Vector2D(p, p2));

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

        #endregion

        #region N-Model

        public static List<(double D, double[] L)> Get(NModel1 model, DParams dParams, double[] start,
            double eps = 0.00001, int t = 100000)
        {
            var range = dParams.Interval.Range(dParams.Count);

            if (dParams.ByPrevious)
                return range
                    .Select(d =>
                    {
                        model.D[dParams.Di, dParams.Dj] = d;
                        var (o, l) = FindLn(model, start, eps, t);
                        start = o;
                        return (d, l);
                    })
                    .ToList();

            return range
                .AsParallel()
                .Select(d =>
                {
                    var copy = (NModel1) model.Copy();
                    copy.D[dParams.Di, dParams.Dj] = d;
                    return (d, FindLn(copy, start, eps, t).L);
                })
                .ToList();
        }

        private static (double[] O, double[] L) FindLn(BaseNModel model, double[] o, double eps, int t)
        {
            var n = o.Length;
            var z = new double[n];
            var a = new double[n][];

            for (var i = 0; i < n; i++)
            {
                a[i] = o.Copy();
                a[i][i] += eps;
            }

            for (var i = 0; i < t; i++)
            {
                var p = model.GetNextPoint(o);
                var v = new double[n][];
                var b = new double[n][];

                v[0] = model.GetNextPoint(a[0]).Subtract(p);
                b[0] = v[0].Normalize();
                z[0] += Math.Log10(v[0].Euclidean() / eps);

                for (var j = 1; j < n; j++)
                {
                    b[j] = v[j] = model.GetNextPoint(a[j]).Subtract(p);
                    for (var k = 0; k < j; k++)
                        b[j] = b[j].Subtract(b[k].Multiply(v[j].Dot(b[k])));

                    z[j] += Math.Log10(b[j].Euclidean() / eps);
                    b[j] = b[j].Normalize();
                }

                for (var j = 0; j < n; j++)
                {
                    a[j] = b[j].Multiply(eps).Add(p);
                }

                o = p;
            }

            var l = new double[n];
            for (var i = 0; i < n; i++)
                l[i] = z[i] / t;

            return (o, l);
        }

        #endregion
    }
}
