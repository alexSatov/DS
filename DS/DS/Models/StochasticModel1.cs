using Accord.Statistics.Distributions.Univariate;
using DS.MathStructures.Points;

namespace DS.Models
{
    /// <summary>
    /// Стохастическая версия модели <see cref="Model1"/>
    /// </summary>
    public class StochasticModel1 : Model1
    {
        public double Eps { get; set; }
        public double Sigma1 { get; set; }
        public double Sigma2 { get; set; }
        public double Sigma3 { get; set; }

        private readonly NormalDistribution random = new NormalDistribution();

        public StochasticModel1(double a1, double a2, double b1, double b2, double px, double py, double d12 = 0,
            double d21 = 0, double eps = 0, double sigma1 = 0, double sigma2 = 0, double sigma3 = 0)
            : base(a1, a2, b1, b2, px, py, d12, d21)
        {
            Eps = eps;
            Sigma1 = sigma1;
            Sigma2 = sigma2;
            Sigma3 = sigma3;
        }

        /// <inheritdoc />
        public override IModel Copy()
        {
            return new StochasticModel1(A1, A2, B1, B2, Px, Py, D12, D21, Eps, Sigma1, Sigma2, Sigma3);
        }

        public double[,] GetMatrixQ(PointX attractor)
        {
            var q = new double[2, 2];

            q[0, 0] = Sigma1 * Sigma1 + Pb1 * Pb1 * D12 * D12 * attractor.X2 * attractor.X2 * Sigma3 * Sigma3;
            q[0, 1] = 0;
            q[1, 0] = 0;
            q[1, 1] = Sigma2 * Sigma2;

            return q;
        }

        protected override double F(double x1, double x2)
        {
            return Pb1 * (A1 * x1 * (B1 - Px * x1) + D12 * x2 * (B2 - Px * x2)) + Eps * Sigma1 * random.Generate()
                + Pb1 * D12 * x2 * Sigma3 * Eps * random.Generate();
        }

        protected override double G(double x1, double x2)
        {
            return Pb2 * (A2 * x2 * (B2 - Px * x2) + D21 * x1 * (B1 - Px * x1))
                + Eps * Sigma2 * random.Generate();
        }
    }
}
