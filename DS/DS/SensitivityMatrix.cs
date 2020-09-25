using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using DS.MathStructures.Points;
using DS.Models;

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
        public static IEnumerable<double[,]> Get(StochasticModel1 model, IList<PointX> attractorPoints)
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

            var b = fn[^1];
            var q = qn[^1];

            for (var i = attractorPoints.Count - 2; i >= 0; i--)
            {
                b = b.Dot(fn[i]);

                var qfi = qn[i];

                for (var k = i + 1; k < attractorPoints.Count; k++)
                {
                    qfi = fn[k].Dot(qfi).Dot(ftn[k]);
                }

                q = q.Add(qfi);
            }

            var w = Lyapunov.SolveDiscreteEquation(b, q);

            yield return w;

            for (var i = 1; i < attractorPoints.Count; i++)
            {
                w = fn[i - 1].Dot(w).Dot(ftn[i - 1]).Add(qn[i - 1]);
                yield return w;
            }
        }

        public static double[,] Get(StochasticModel1 model, PointX attractor)
        {
            return Get(model, new List<PointX> { attractor }).First();
        }
    }
}
