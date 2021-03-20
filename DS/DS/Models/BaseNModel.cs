using System;

namespace DS.Models
{
    /// <summary>
    /// N-мерная модель исследования
    /// </summary>
    public abstract class BaseNModel
    {
        public int N { get; }

        protected BaseNModel(int n)
        {
            N = n;
        }

        /// <summary>
        /// Модуль величины, которую можно считать выходом в бесконечность для модели
        /// </summary>
        public virtual double AbsInf => 100;

        /// <summary>
        /// Итерация точки
        /// </summary>
        public double[] GetNextPoint(double[] current)
        {
            if (current.Length != N)
            {
                throw new ArgumentException($"Point must be size of {N}");
            }

            var next = new double[N];
            for (var i = 0; i < N; i++)
            {
                next[i] = F_i(current, i);
            }

            return next;
        }

        /// <summary>
        /// Создание копии модели (исп. для распараллеливания)
        /// </summary>
        public abstract BaseNModel Copy();

        protected abstract double F_i(double[] x, int i);
    }
}
