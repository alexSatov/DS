using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Accord.Math;
using Accord.Math.Decompositions;

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

			for (var angle = 0.0; angle < 2* Math.PI; angle += 0.01)
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
		/// <param name="model">Модель</param>
		/// <param name="zik">Элементы ЗИКа</param>
		/// <returns>2 набора точек - координаты внешнего и внутреннего эллипсов</returns>
		public static (List<PointX> Outer, List<PointX> Inner) GetForZik(StochasticModel model, List<PointX> zik,
			double qu = 1.821)
		{
			var boundaries = FindBoundaries(zik);
			var x10 = boundaries.X1Min + (boundaries.X1Max - boundaries.X1Min) / 2;
			var x20 = boundaries.X2Min + (boundaries.X2Max - boundaries.X2Min) / 2;
			var orderedZik = GetZikOrderedByAngle(x10, x20, zik);

			//CheckOrderedZik(orderedZik);

			var pv = GetPVectors(orderedZik).ToList();
			var p = pv.Select(v => v.Outer(v)).ToList();
			var f = orderedZik.Select(model.GetMatrixF).ToList();
			var phi = GetPhiMatrix(f, p);
			var s = orderedZik.Select(model.GetMatrixQ).ToList();
			var q = GetQMatrix(f, p, s);
			var mu = GetMu(pv, p, f, s, q, phi).ToList();
			var ellipse1 = new List<PointX>();
			var ellipse2 = new List<PointX>();

			for (var i = 0; i < zik.Count; i++)
			{
				var pd = pv[i].Multiply(model.Eps * qu * Math.Sqrt(2 * mu[i]));
				var e1P = new PointX(orderedZik[i].X1 + pd[0], orderedZik[i].X2 + pd[1]);
				var e2P = new PointX(orderedZik[i].X1 - pd[0], orderedZik[i].X2 - pd[1]);

				ellipse1.Add(e1P);
				ellipse2.Add(e2P);
			}

			return (ellipse1, ellipse2);
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

		private static List<PointX> GetZikOrderedByAngle(double x10, double x20, List<PointX> zik)
		{
			var zikAdvanced = new List<(int Index, double Angle)>();

			for (var i = 0; i < zik.Count; i++)
			{
				var angle = 0.0;
				var x1d = zik[i].X1 - x10;
				var x2d = zik[i].X2 - x20;

				if (x1d == 0)
					angle = x2d > 0 ? Math.PI / 2 : -Math.PI / 2;
				else if (x1d > 0 && x2d >= 0)
					angle = Math.Atan(x2d / x1d);
				else if (x1d > 0 && x2d < 0)
					angle = 2 * Math.PI + Math.Atan(x2d / x1d);
				else if (x1d < 0)
					angle = Math.PI + Math.Atan(x2d / x1d);

				zikAdvanced.Add((i, angle));
			}

			var result = zikAdvanced.OrderBy(v => v.Angle).ToList();

			return zikAdvanced
				.OrderBy(v => v.Angle)
				.Select(v => zik[v.Index])
				.ToList();
		}

		private static void CheckOrderedZik(List<PointX> zik)
		{
			for (var i = 0; i < zik.Count; i++)
			{
				var p1 = zik[i];
				var p2 = i == zik.Count - 1 ? zik[0] : zik[i + 1];

				if (Math.Sqrt(Math.Pow(p1.X1 - p2.X1, 2) + Math.Pow(p1.X2 - p2.X2, 2)) > 0.00001)
					throw new ArgumentException("Зик не удовлетворяет условию плотности");
			}
		}

		private static IEnumerable<double[]> GetPVectors(List<PointX> orderedZik)
		{
			for (var i = 0; i < orderedZik.Count; i++)
			{
				var (x1, y1) = orderedZik[i];
				var (x2, y2) = i == orderedZik.Count - 1 ? orderedZik[i] : orderedZik[i + 1];
				yield return new[] { x1 - x2, y2 - y1 }.Normalize();
			}
		}

		private static double[,] GetPhiMatrix(List<double[,]> f, List<double[,]> p)
		{
			var phi = f[f.Count - 1];

			for (var i = f.Count - 1; i > 0; i--)
				phi = phi.Dot(p[i]).Dot(f[i - 1]);

			return phi;
		}

		private static double[,] GetQMatrix(List<double[,]> f, List<double[,]> p, List<double[,]> s)
		{
			var k = f.Count - 1;
			var q = Matrix.Zeros(2, 2);

			for (var i = 0; i < k; i++)
				q = p[i + 1].Dot(f[i].Dot(q).Dot(f[i].Transpose()).Add(s[i])).Dot(p[i + 1]);

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
