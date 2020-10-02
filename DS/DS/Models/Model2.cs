using System;
using DS.MathStructures.Points;

namespace DS.Models
{
    /// <summary>
    /// Модель Ряшко и Башкирцевой
    /// </summary>
    public abstract class Model2 : BaseModel
    {
        public double A { get; set; }
        public double B { get; set; }

        /// <inheritdoc />
        public override Func<double, double> LcH => _ => 0;

        /// <inheritdoc />
        public override Func<double, double> LcV => null;

        protected Model2(double a, double b)
        {
            A = a;
            B = b;
        }

        protected override double DfX1(PointX point)
        {
            return B;
        }

        protected override double DfX2(PointX point)
        {
            return -2 * A * point.X2;
        }

        protected override double DgX1(PointX point)
        {
            return 1;
        }

        protected override double DgX2(PointX point)
        {
            return 0;
        }
    }
}
