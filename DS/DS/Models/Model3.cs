using System;
using DS.MathStructures.Points;

namespace DS.Models
{
    /// <summary>
    /// Модель Саши Беляева
    /// </summary>
    public abstract class Model3 : BaseModel
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }

        /// <inheritdoc />
        public override Func<double, double> LcH => x1 => k * (2 * x1 - 1) * (D * x1 - C);

        /// <inheritdoc />
        public override Func<double, double> LcV => null;

        private readonly double k;

        protected Model3(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;

            k = a / (b * c);
        }

        protected override double DfX1(PointX point)
        {
            return A - 2 * A * point.X1 - B * point.X2;
        }

        protected override double DfX2(PointX point)
        {
            return -B * point.X1;
        }

        protected override double DgX1(PointX point)
        {
            return D * point.X2;
        }

        protected override double DgX2(PointX point)
        {
            return D * point.X1 - C;
        }
    }
}
