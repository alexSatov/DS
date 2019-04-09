using System;
using System.Collections.Generic;
using Accord.Math;

namespace DS
{
	public static class SensitivityMatrix
	{
		/// <summary>
		/// Получение матрицы чувствительности.
		/// Решение уравнения W = B W BT + Q
		/// </summary>
		/// <param name="model">Модель</param>
		/// <param name="attractorPoints">Элементы аттрактора (равновесия или цикла)</param>
		/// <returns>Матрицы чувствительности для каждой точки аттрактора</returns>
		public static IEnumerable<double[,]> Get(StochasticModel model, List<PointX> attractorPoints)
		{
			var fn = new List<double[,]>();
			var ftn = new List<double[,]>();
			var qn = new List<double[,]>();

			foreach (var point in attractorPoints)
			{
				var f = model.GetMatrixF(point);

				fn.Add(f);
				ftn.Add(f.Transpose());
				qn.Add(model.GetMatrixQ(point));
			}

			var b = fn[fn.Count - 1];
			var q = qn[qn.Count - 1];

			for (var i = attractorPoints.Count - 2; i >= 0; i--)
			{
				b = b.Dot(fn[i]);

				var qi = fn[fn.Count - 1];

				for (var k = i + 2; k < attractorPoints.Count; k++)
					qi = qi.Dot(fn[attractorPoints.Count - k]);

				qi.Dot(qn[i]).Dot(ftn[i + 1]);

				for (var k = i + 2; k < attractorPoints.Count; k++)
					qi = qi.Dot(ftn[k]);

				q = q.Add(qi);
			}

			var w = Lyapunov.SolveDiscreteEquation(b, q);

			yield return w;

			for (var i = 1; i < attractorPoints.Count; i++)
			{
				w = fn[i - 1].Dot(ftn[i - 1]).Add(qn[i - 1]);
				yield return w;
			}
		}
	}
}
