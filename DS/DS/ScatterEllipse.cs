using System;
using System.Collections.Generic;

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
	}
}
