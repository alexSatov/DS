using Accord.Statistics.Distributions.Univariate;

namespace DS.Models
{
    /// <summary>
    /// Стохастическая версия модели <see cref="Model2"/>
    /// </summary>
    public class StochasticModel2 : Model2
    {
        public double Eps { get; set; }
        public double Sigma { get; set; }

        private readonly NormalDistribution random = new NormalDistribution();

        public StochasticModel2(double a, double b, double eps = 0, double sigma = 1) : base(a, b)
        {
            Eps = eps;
            Sigma = sigma;
        }

        public override BaseModel Copy()
        {
            return new StochasticModel2(A, B, Eps, Sigma);
        }

        protected override double F(double x1, double x2)
        {
            return 1 - A * x2 * x2 + B * x1 + Eps * Sigma * random.Generate();
        }

        protected override double G(double x1, double x2)
        {
            return x1;
        }
    }
}
