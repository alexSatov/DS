using System;
using System.Linq;
using Accord.Math;
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
        public virtual double AbsInf => 100;

        /// <summary>
        /// Функция LC_-1 в виде x2 = f(x1)
        /// </summary>
        public abstract Func<double, double> LcH { get; }

        /// <summary>
        /// Функция LC_-1 в виде x1 = f(x2)
        /// </summary>
        public abstract Func<double, double> LcV { get; }

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

        /// <summary>
        /// Возвращает матрицу Q для нахождения доверительной полосы хаоса по LC
        /// </summary>
        public virtual double[,] GetLcQMatrix()
        {
            return Matrix.Identity(2);
        }

        /// <summary>
        /// Возвращает значение матрицы якоби в точке
        /// </summary>
        public double[,] GetJacobiMatrix(PointX point)
        {
            return new[,]
            {
                { DfX1(point), DfX2(point) },
                { DgX1(point), DgX2(point) }
            };
        }

        /// <summary>
        /// Возвращает горизонтальную LC-1 по диапазону X1
        /// </summary>
        public Lc GetLc0H(double minX1, double maxX1, int lcPointsCount = 100)
        {
            var stepX1 = (maxX1 - minX1) / lcPointsCount;
            var lc0HPoints = Enumerable.Range(0, lcPointsCount + 1)
                .Select(i => minX1 + i * stepX1)
                .Select(x => new PointX(x, LcH(x)));

            return new Lc(lc0HPoints);
        }

        /// <summary>
        /// Возвращает вертикальную LC-1 по диапазону X2
        /// </summary>
        public Lc GetLc0V(double minX2, double maxX2, int lcPointsCount = 100)
        {
            var stepX2 = (maxX2 - minX2) / lcPointsCount;
            var lc0VPoints = Enumerable.Range(0, lcPointsCount + 1)
                .Select(i => minX2 + i * stepX2)
                .Select(x => new PointX(LcV(x), x));

            return new Lc(lc0VPoints, LcType.V);
        }

        protected abstract double DfX1(PointX point);
        protected abstract double DfX2(PointX point);
        protected abstract double DgX1(PointX point);
        protected abstract double DgX2(PointX point);

        protected abstract double F(double x1, double x2);
        protected abstract double G(double x1, double x2);
    }
}
