using System;

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
        public override Func<double, double> LCH => _ => 0;

        /// <inheritdoc />
        public override Func<double, double> LCV => null;

        protected Model2(double a, double b)
        {
            A = a;
            B = b;
        }
    }
}
