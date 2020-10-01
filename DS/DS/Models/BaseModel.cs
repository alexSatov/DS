using System;
using DS.MathStructures.Points;

namespace DS.Models
{
    /// <summary>
    /// Модель исследования
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// Модуль величины, которую можно считать выходом в бесконечность для модели
        /// </summary>
        public double AbsInf => 100;

        /// <summary>
        /// Функция LC_-1 в виде x2 = f(x1)
        /// </summary>
        public abstract Func<double, double> LCH { get; }

        /// <summary>
        /// Функция LC_-1 в виде x1 = f(x2)
        /// </summary>
        public abstract Func<double, double> LCV { get; }

        /// <summary>
        /// Итерация точки
        /// </summary>
        public PointX GetNextPoint(PointX current)
        {
            return new PointX(F(current.X1, current.X2), G(current.X1, current.X2));
        }

        /// <summary>
        /// Создание копии модели (исп. для распараллеливания)
        /// </summary>
        public abstract BaseModel Copy();

        protected abstract double F(double x1, double x2);
        protected abstract double G(double x1, double x2);
    }
}
