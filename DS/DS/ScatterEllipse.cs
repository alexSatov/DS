﻿using System;
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

			//CheckOrderedZik(zik, orderedZik);

			var pv = GetPVectors(zik, orderedZik);

			return BuildEllipses(model, zik, pv, qu);
		}

		/// <summary>
		/// Построение эллипса рассеивания для ЗИКа. Альтернативный алгоритм.
		/// </summary>
		/// <param name="dModel">Детерм. модель</param>
		/// <param name="sModel">Стох. модель</param>
		/// <param name="zik">Элементы ЗИКа</param>
		/// <param name="k">Кратность зика</param>
		/// <returns>2 набора точек - координаты внешнего и внутреннего эллипсов</returns>
		public static (List<PointX> Outer, List<PointX> Inner) GetForZik2(DeterministicModel dModel, StochasticModel sModel,
			List<PointX> zik, int k = 1, double qu = 1.821)
		{
			var pv = GetPVectors(dModel, zik, k);
			return BuildEllipses(sModel, zik, pv, qu);
		}

		public static List<double> GetMuForZik(StochasticModel model, List<PointX> zik, double qu = 1.821)
		{
			var boundaries = FindBoundaries(zik);
			var x10 = boundaries.X1Min + (boundaries.X1Max - boundaries.X1Min) / 2;
			var x20 = boundaries.X2Min + (boundaries.X2Max - boundaries.X2Min) / 2;
			var orderedZik = GetZikOrderedByAngle(x10, x20, zik);

			//CheckOrderedZik(zik, orderedZik);

			var pv = GetPVectors(zik, orderedZik);
			var p = pv.Select(v => v.Outer(v)).ToList();
			var f = zik.Select(model.GetMatrixF).ToList();
			var phi = GetPhiMatrix(f, p);
			var s = zik.Select(model.GetMatrixQ).ToList();
			var q = GetQMatrix(f, p, s);

			return GetMu(pv, p, f, s, q, phi).ToList();
		}

		private static (List<PointX> Outer, List<PointX> Inner) BuildEllipses(StochasticModel model, List<PointX> zik,
			List<double[]> pv, double qu)
		{
			var p = pv.Select(v => v.Outer(v)).ToList();
			var f = zik.Select(model.GetMatrixF).ToList();
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

		private static List<double[]> GetPVectors(DeterministicModel model, List<PointX> zik, int k)
		{
			var pv = new List<double[]>();
			var (p1, p2) = (zik[0], zik[zik.Count - 1]);

			for (var i = 0; i < zik.Count; i++)
			{
				var (x1, y1) = p1;
				var (x2, y2) = p2;

				pv.Add(new[] { y1 - y2, x2 - x1 }.Normalize());

				for (var j = 0; j < k; j++)
				{
					p1 = model.GetNextPoint(p1);
					p2 = model.GetNextPoint(p2);
				}
			}

			return pv;
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
			{
				q = p[i + 1].Dot(f[i].Dot(q).Dot(f[i].Transpose()).Add(s[i])).Dot(p[i + 1]);

				//if (double.IsNaN(q[0, 0]) || i == 4343)
				//	continue;
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
