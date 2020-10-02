using System;
using DS.MathStructures.Points;

namespace DS.Models
{
    /// <summary>
    /// Моя модель (А. Сатов)
    /// </summary>
    public abstract class Model1 : BaseModel
    {
        public double A1 { get; set; }
        public double A2 { get; set; }
        public double B1 { get; set; }
        public double B2 { get; set; }
        public double Px { get; set; }
        public double Py { get; set; }
        public double D12 { get; set; }
        public double D21 { get; set; }

        public double Pb1 { get; set; }
        public double Pb2 { get; set; }

        /// <inheritdoc />
        public override Func<double, double> LcH => _ => 40;

        /// <inheritdoc />
        public override Func<double, double> LcV => _ => 20;

        protected Model1(double a1, double a2, double b1, double b2, double px, double py,
            double d12 = 0, double d21 = 0)
        {
            A1 = a1;
            A2 = a2;
            B1 = b1;
            B2 = b2;
            Px = px;
            Py = py;
            D12 = d12;
            D21 = d21;

            Pb1 = B1 / (Px * Py);
            Pb2 = B2 / (Px * Py);
        }

        protected override double DfX1(PointX point)
        {
            return Pb1 * A1 * (B1 - 2 * Px * point.X1);
        }

        protected override double DfX2(PointX point)
        {
            return Pb1 * D12 * (B2 - 2 * Px * point.X2);
        }

        protected override double DgX1(PointX point)
        {
            return Pb2 * D21 * (B1 - 2 * Px * point.X1);
        }

        protected override double DgX2(PointX point)
        {
            return Pb2 * A2 * (B2 - 2 * Px * point.X2);
        }
    }
}
