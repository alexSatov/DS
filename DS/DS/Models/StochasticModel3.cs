using Accord.Statistics.Distributions.Univariate;

namespace DS.Models
{
    /// <summary>
    /// Стохастическая версия модели <see cref="Model3"/>
    /// Аддитивный шум
    /// </summary>
    public class StochasticModel3 : Model3
    {
        public double Eps { get; set; }
        public double Sigma1 { get; set; }
        public double Sigma2 { get; set; }

        private readonly NormalDistribution random = new NormalDistribution();

        public StochasticModel3(double a, double b, double c, double d,
            double eps = 0, double sigma1 = 1, double sigma2 = 1) : base(a, b, c, d)
        {
            Eps = eps;
            Sigma1 = sigma1;
            Sigma2 = sigma2;
        }

        public override BaseModel Copy()
        {
            return new StochasticModel3(A, B, C, D, Eps, Sigma1, Sigma2);
        }

        protected override double F(double x1, double x2)
        {
            return A * x1 * (1 - x1) - B * x1 * x2 + Eps * Sigma1 * random.Generate();
        }

        protected override double G(double x1, double x2)
        {
            return D * x1 * x2 - C * x2 + Eps * Sigma2 * random.Generate();
        }
    }
}
