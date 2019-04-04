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
		/// <param name="attractor">Элемент аттрактора (равновесия или цикла)</param>
		/// <param name="cycleN">Кратность цикла</param>
		/// <returns></returns>
		public static IEnumerable<double[,]> Get(StochasticModel model, PointX attractor, int cycleN)
		{
			var b = model.GetMatrixF(attractor);
			var bt = b.Transpose();
			var q = model.GetMatrixQ(attractor);
			var w = Lyapunov.SolveDiscreteEquation(b, q);

			for (var i = 0; i < cycleN; i++)
			{
				yield return w;
				w = b.Dot(w).Dot(bt).Add(q);
			}
		}
	}
}
