using System;

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
        public override Func<double, double> LCH => x1 => k * (2 * x1 - 1) * (D * x1 - C);

        /// <inheritdoc />
        public override Func<double, double> LCV => null;

        private readonly double k;

        protected Model3(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;

            k = a / (b * c);
        }
    }
}
