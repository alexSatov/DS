using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Math.Decompositions;
using DS.MathStructures.Points;
using DS.Models;

namespace DS
{
    public static class ScatterEllipse
    {
        /// <summary>
        /// Построение эллипса рассеивания
        /// </summary>
        /// <param name="attractor">Точка равновесия или элемента цикла</param>
        /// <param name="lambda1">Первое собственное число матрицы чувствительности</param>
        /// <param name="lambda2">Второе собственное число матрицы чувствительности</param>
        /// <param name="v1">Первый собственный вектор матрицы чувствительности</param>
        /// <param name="v2">Второй собственный вектор матрицы чувствительности</param>
        /// <param name="eps">Интенсивность случайного возмущения</param>
        /// <param name="p">Доверительная вероятность</param>
        /// <returns>Набор точек - координат эллипса</returns>
        public static IEnumerable<PointX> Get(PointX attractor, double lambda1, double lambda2, double[] v1, double[] v2,
            double eps, double p = 0.99)
        {
            var q = Math.Sqrt(-Math.Log(1 - p));
            var k1 = eps * q * Math.Sqrt(2 * lambda1);
            var k2 = eps * q * Math.Sqrt(2 * lambda2);

            for (var angle = 0.0; angle < 2 * Math.PI; angle += 0.01)
            {
                var z1 = k1 * Math.Cos(angle);
                var z2 = k2 * Math.Sin(angle);

                var x1 = attractor.X1 + (z1 * v2[1] - z2 * v1[1]) / (v1[0] * v2[1] - v1[1] * v2[0]);
                var x2 = attractor.X2 + (z2 * v1[0] - z1 * v2[0]) / (v1[0] * v2[1] - v1[1] * v2[0]);

                yield return new PointX(x1, x2);
            }
        }

        /// <summary>
        /// Построение эллипса рассеивания для ЗИКа
        /// </summary>
        /// <param name="sModel1">Модель</param>
        /// <param name="zik">Элементы ЗИКа</param>
        /// <returns>2 набора точек - координаты внешнего и внутреннего эллипсов</returns>
        public static (List<PointX> Outer, List<PointX> Inner) GetForZik(DeterministicModel1 dModel1,
            StochasticModel1 sModel1, List<PointX> zik, double qu = 1.821)
        {
            var boundaries = FindBoundaries(zik);
            var x10 = boundaries.X1Min + (boundaries.X1Max - boundaries.X1Min) / 2;
            var x20 = boundaries.X2Min + (boundaries.X2Max - boundaries.X2Min) / 2;
            var orderedZik = GetZikOrderedByAngle(x10, x20, zik);

            //CheckOrderedZik(zik, orderedZik);

            var pv = GetPVectors(zik, orderedZik);

            return BuildEllipses(sModel1, zik, pv, qu);
        }

        /// <summary>
        /// Построение эллипса рассеивания для ЗИКа. Альтернативный алгоритм.
        /// </summary>
        /// <param name="dModel1">Детерм. модель</param>
        /// <param name="sModel1">Стох. модель</param>
        /// <param name="zik">Элементы ЗИКа</param>
        /// <param name="k">Кратность зика</param>
        /// <returns>2 набора точек - координаты внешнего и внутреннего эллипсов</returns>
        public static (List<PointX> Outer, List<PointX> Inner) GetForZik2(DeterministicModel1 dModel1,
            StochasticModel1 sModel1, List<PointX> zik, double qu = 1.821)
        {
            var pv = GetPVectors(dModel1, zik);
            return BuildEllipses(sModel1, zik, pv, qu);
        }

        public static List<double> GetMuForZik(StochasticModel1 model, List<PointX> zik)
        {
            var boundaries = FindBoundaries(zik);
            var x10 = boundaries.X1Min + (boundaries.X1Max - boundaries.X1Min) / 2;
            var x20 = boundaries.X2Min + (boundaries.X2Max - boundaries.X2Min) / 2;
            var orderedZik = GetZikOrderedByAngle(x10, x20, zik);

            //CheckOrderedZik(zik, orderedZik);

            var pv = GetPVectors(zik, orderedZik);
            var p = pv.Select(v => v.Outer(v)).ToList();
            var f = zik.Select(model.GetJacobiMatrix).ToList();
            var phi = GetPhiMatrix(f, p);
            var s = zik.Select(model.GetMatrixQ).ToList();
            var q = GetQMatrix(f, p, s);

            return GetMu(pv, p, f, s, q, phi).ToList();
        }

        /// <summary>
        /// Построение эллипса рассеивания для хаоса, описанного критическими линиями.
        /// </summary>
        public static IEnumerable<LcPoint> GetForChaosLc(
            BaseModel model, LcSet lcSet, double eps = 0.01, double kq = 0.001)
        {
            var lcEllipsePoints = GetLcEllipseMap(model, lcSet, eps, kq);
            var borderPoints = lcSet.GetBorderPoints();

            foreach (var borderPoint in borderPoints)
                if (lcEllipsePoints.ContainsKey(borderPoint.LcType))
                    yield return new LcPoint(borderPoint.LcType, borderPoint.LcIndex, borderPoint.Index,
                        lcEllipsePoints[borderPoint.LcType][borderPoint.LcIndex, borderPoint.Index]);
        }

        private static Dictionary<LcType, PointX[,]> GetLcEllipseMap(BaseModel model, LcSet lcSet,
            double eps, double kq)
        {
            var keps = 3 * eps;
            var allSegments = lcSet.GetAllSegments().ToList();
            var lcHList = lcSet[LcType.H];
            var lcVList = lcSet[LcType.V];

            PointX X(PointX point, double mu, double[] nu)
            {
                var v = nu.Multiply(keps * Math.Sqrt(mu));
                return new PointX(point.X1 + v[0], point.X2 + v[1]);
            }

            double[] Nu(PointX point, double[] q)
            {
                var n1 = new[] { q[1], -q[0] };
                var n2 = new[] { -q[1], q[0] };
                var d1 = n1.Multiply(kq);
                var s1 = new PointX(point.X1 + d1[0], point.X2 + d1[1]);

                return LcSet.IsOutOrBorderPoint(s1, allSegments) ? n1.Normalize() : n2.Normalize();
            }

            var s = model.GetLcQMatrix();
            var (n, m) = (lcHList.Count, lcHList[0].Count);
            var mapH = new PointX[n, m];
            var mapV = new PointX[n, m];

            if (lcHList.Count > 0)
            {
                var muH = new double[n, m];
                var nuH = new Dictionary<(int, int), double[]>();
                var qH = new Dictionary<(int, int), double[]>();
                var fH = new Dictionary<(int, int), double[,]>();
                var wH = new Dictionary<(int, int), double[,]>();

                for (var j = 0; j < m; j++)
                {
                    var point = lcHList[0][j];
                    qH[(0, j)] = new double[] { 1, 0 };
                    fH[(0, j)] = model.GetJacobiMatrix(point);
                }

                for (var j = 0; j < m; j++)
                {
                    qH[(1, j)] = fH[(0, j)].Dot(qH[(0, j)]);
                    fH[(1, j)] = model.GetJacobiMatrix(lcHList[1][j]);
                    nuH[(1, j)] = Nu(lcHList[1][j], qH[(1, j)]);
                    muH[1, j] = nuH[(1, j)].Dot(s).Dot(nuH[(1, j)]);
                    wH[(1, j)] = nuH[(1, j)].Multiply(muH[1, j]).Outer(nuH[(1, j)]);
                    mapH[1, j] = X(lcHList[1][j], muH[1, j], nuH[(1, j)]);
                }

                for (var i = 2; i < n; i++)
                for (var j = 0; j < m; j++)
                {
                    qH[(i, j)] = fH[(i - 1, j)].Dot(qH[(i - 1, j)]);
                    fH[(i, j)] = model.GetJacobiMatrix(lcHList[i][j]);
                    nuH[(i, j)] = Nu(lcHList[i][j], qH[(i, j)]);
                    var st = fH[(i - 1, j)].Dot(wH[(i - 1, j)]).Dot(fH[(i - 1, j)].Transpose()).Add(s);
                    muH[i, j] = nuH[(i, j)].Dot(st).Dot(nuH[(i, j)]);
                    wH[(i, j)] = nuH[(i, j)].Multiply(muH[i, j]).Outer(nuH[(i, j)]);
                    mapH[i, j] = X(lcHList[i][j], muH[i, j], nuH[(i, j)]);
                }
            }

            if (lcVList.Count > 0)
            {
                var muV = new double[n, m];
                var nuV = new Dictionary<(int, int), double[]>();
                var qV = new Dictionary<(int, int), double[]>();
                var fV = new Dictionary<(int, int), double[,]>();
                var wV = new Dictionary<(int, int), double[,]>();

                for (var j = 0; j < m; j++)
                {
                    var point = lcVList[0][j];
                    qV[(0, j)] = new double[] { 0, 1 };
                    fV[(0, j)] = model.GetJacobiMatrix(point);
                }

                for (var j = 0; j < m; j++)
                {
                    qV[(1, j)] = fV[(0, j)].Dot(qV[(0, j)]);
                    fV[(1, j)] = model.GetJacobiMatrix(lcVList[1][j]);
                    nuV[(1, j)] = Nu(lcVList[1][j], qV[(1, j)]);
                    muV[1, j] = nuV[(1, j)].Dot(s).Dot(nuV[(1, j)]);
                    wV[(1, j)] = nuV[(1, j)].Multiply(muV[1, j]).Outer(nuV[(1, j)]);
                    mapV[1, j] = X(lcVList[1][j], muV[1, j], nuV[(1, j)]);
                }

                for (var i = 2; i < n; i++)
                for (var j = 0; j < m; j++)
                {
                    qV[(i, j)] = fV[(i - 1, j)].Dot(qV[(i - 1, j)]);
                    fV[(i, j)] = model.GetJacobiMatrix(lcVList[i][j]);
                    nuV[(i, j)] = Nu(lcVList[i][j], qV[(i, j)]);
                    var st = fV[(i - 1, j)].Dot(wV[(i - 1, j)]).Dot(fV[(i - 1, j)].Transpose()).Add(s);
                    muV[i, j] = nuV[(i, j)].Dot(st).Dot(nuV[(i, j)]);
                    wV[(i, j)] = nuV[(i, j)].Multiply(muV[i, j]).Outer(nuV[(i, j)]);
                    mapV[i, j] = X(lcVList[i][j], muV[i, j], nuV[(i, j)]);
                }
            }

            return new Dictionary<LcType, PointX[,]> { { LcType.H, mapH }, { LcType.V, mapV } };
        }

        private static (List<PointX> Outer, List<PointX> Inner) BuildEllipses(StochasticModel1 model,
            List<PointX> zik, List<double[]> pv, double qu)
        {
            var p = pv.Select(v => v.Outer(v)).ToList();
            var f = zik.Select(model.GetJacobiMatrix).ToList();
            var phi = GetPhiMatrix(f, p);
            var s = zik.Select(model.GetMatrixQ).ToList();
            var q = GetQMatrix(f, p, s);
            var mu = GetMu(pv, p, f, s, q, phi).ToList();
            var ellipse1 = new List<PointX>();
            var ellipse2 = new List<PointX>();

            for (var i = 0; i < zik.Count; i++)
            {
                var pd = pv[i].Multiply(model.Eps * qu * Math.Sqrt(2 * mu[i]));
                var e1P = new PointX(zik[i].X1 + pd[0], zik[i].X2 + pd[1]);
                var e2P = new PointX(zik[i].X1 - pd[0], zik[i].X2 - pd[1]);

                ellipse1.Add(e1P);
                ellipse2.Add(e2P);
            }

            var ellipse1MinX = ellipse1.Min(point => point.X);
            var ellipse1MaxX = ellipse1.Max(point => point.X);
            var ellipse2MinX = ellipse2.Min(point => point.X);
            var ellipse2MaxX = ellipse2.Max(point => point.X);
            var d1 = ellipse1MaxX - ellipse1MinX;
            var d2 = ellipse2MaxX - ellipse2MinX;

            return d1 >= d2 ? (ellipse1, ellipse2) : (ellipse2, ellipse1);
        }

        private static (double X1Min, double X1Max, double X2Min, double X2Max) FindBoundaries(List<PointX> zik)
        {
            var x1Min = double.MaxValue;
            var x1Max = double.MinValue;
            var x2Min = double.MaxValue;
            var x2Max = double.MinValue;

            foreach (var (x1, x2) in zik)
            {
                if (x1Min > x1)
                    x1Min = x1;
                if (x1Max < x1)
                    x1Max = x1;
                if (x2Min > x2)
                    x2Min = x2;
                if (x2Max < x2)
                    x2Max = x2;
            }

            return (x1Min, x1Max, x2Min, x2Max);
        }

        private static List<int> GetZikOrderedByAngle(double x10, double x20, List<PointX> zik)
        {
            var orderedZik = new List<(int Index, double Angle)>();

            for (var i = 0; i < zik.Count; i++)
            {
                var x1d = zik[i].X1 - x10;
                var x2d = zik[i].X2 - x20;
                var angle = Math.Atan2(x2d, x1d);

                if (angle < 0)
                    angle += 2 * Math.PI;

                orderedZik.Add((i, angle));
            }

            return orderedZik
                .OrderBy(v => v.Angle)
                .Select(v => v.Index)
                .ToList();
        }

        private static void CheckOrderedZik(List<PointX> zik, List<int> orderedZik)
        {
            for (var i = 0; i < zik.Count; i++)
            {
                var p1 = zik[orderedZik[i]];
                var p2 = i == zik.Count - 1 ? zik[orderedZik[0]] : zik[orderedZik[i + 1]];

                if (Math.Sqrt(Math.Pow(p1.X1 - p2.X1, 2) + Math.Pow(p1.X2 - p2.X2, 2)) > 0.00001)
                    throw new ArgumentException("Зик не удовлетворяет условию плотности");
            }
        }

        private static List<double[]> GetPVectors(List<PointX> zik, List<int> orderedZik)
        {
            var pv = new List<(int Index, double[] Vector)>();

            for (var i = 0; i < orderedZik.Count; i++)
            {
                var (x1, y1) = zik[orderedZik[i]];
                var (x2, y2) = i == zik.Count - 1 ? zik[orderedZik[0]] : zik[orderedZik[i + 1]];

                pv.Add((orderedZik[i], new[] { y1 - y2, x2 - x1 }.Normalize()));
            }

            return pv
                .OrderBy(v => v.Index)
                .Select(v => v.Vector)
                .ToList();
        }

        private static List<double[]> GetPVectors(BaseModel baseModel, List<PointX> zik)
        {
            var pv = new List<double[]>();
            var (p1, p2) = (zik[0], zik[^1]);

            for (var i = 0; i < zik.Count; i++)
            {
                var (x1, y1) = p1;
                var (x2, y2) = p2;

                pv.Add(new[] { y1 - y2, x2 - x1 }.Normalize());

                p1 = baseModel.GetNextPoint(p1);
                p2 = baseModel.GetNextPoint(p2);
            }

            return pv;
        }

        private static double[,] GetPhiMatrix(List<double[,]> f, List<double[,]> p)
        {
            var phi = f[0];

            for (var i = 1; i < f.Count; i++)
                phi = f[i].Dot(p[i].Dot(phi));

            return phi;
        }

        private static double[,] GetQMatrix(List<double[,]> f, List<double[,]> p, List<double[,]> s)
        {
            var k = f.Count - 1;
            var q = Matrix.Zeros(2, 2);

            for (var i = 0; i < k; i++)
            {
                q = p[i + 1].Dot(f[i].Dot(q).Dot(f[i].Transpose()).Add(s[i])).Dot(p[i + 1]);
            }

            return f[k].Dot(q).Dot(f[k].Transpose()).Add(s[k]);
        }

        private static IEnumerable<double> GetMu(List<double[]> pv, List<double[,]> p, List<double[,]> f,
            List<double[,]> s, double[,] q, double[,] phi)
        {
            var mu0 = pv[0].Dot(q).Dot(pv[0]) / (1 - Math.Pow(pv[0].Dot(phi).Dot(pv[0]), 2));

            yield return mu0;

            var w = p[0].Multiply(mu0);

            for (var i = 0; i < f.Count - 1; i++)
            {
                w = p[i + 1].Dot(f[i].Dot(w).Dot(f[i].Transpose()).Add(s[i])).Dot(p[i + 1]);
                var eigenvalueDecomposition = new EigenvalueDecomposition(w);

                yield return eigenvalueDecomposition.RealEigenvalues.Max();
            }
        }
    }
}
