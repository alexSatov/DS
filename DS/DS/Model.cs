using Accord.Statistics.Distributions.Univariate;
using DS.MathStructures;

namespace DS
{
    public abstract class Model
    {
        public double A1 { get; set; }
        public double A2 { get; set; }
        public double B1 { get; set; }
        public double B2 { get; set; }
        public double Px { get; set; }
        public double Py { get; set; }
        public double D12 { get; set; }
        public double D21 { get; set; }
        public double MaxX { get; set; }

        public double Pb1 { get; set; }
        public double Pb2 { get; set; }

        protected Model(double a1, double a2, double b1, double b2, double px, double py,
            double d12 = 0, double d21 = 0, double maxX = 100)
        {
            MaxX = maxX;
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

        public PointX GetNextPoint(PointX current)
        {
            return new PointX(F(current.X1, current.X2), G(current.X1, current.X2));
        }

        public abstract Model Copy();

        public double[,] GetMatrixF(PointX attractor)
        {
            var f = new double[2, 2];

            f[0, 0] = DFByX1(attractor.X1);
            f[0, 1] = DFByX2(attractor.X2);
            f[1, 0] = DGByX1(attractor.X1);
            f[1, 1] = DGByX2(attractor.X2);

            return f;
        }

        protected abstract double F(double x1, double x2);
        protected abstract double G(double x1, double x2);

        private double DFByX1(double x1)
        {
            return Pb1 * (A1 * B1 - 2 * A1 * Px * x1);
        }

        private double DFByX2(double x2)
        {
            return Pb1 * (D12 * B2 - 2 * D12 * Px * x2);
        }

        private double DGByX1(double x1)
        {
            return Pb2 * (D21 * B1 - 2 * D21 * Px * x1);
        }

        private double DGByX2(double x2)
        {
            return Pb2 * (A2 * B2 - 2 * A2 * Px * x2);
        }
    }

    public class DeterministicModel : Model
    {
        public DeterministicModel(double a1, double a2, double b1, double b2, double px, double py,
            double d12 = 0, double d21 = 0, double maxX = 100) : base(a1, a2, b1, b2, px, py, d12, d21, maxX)
        {
        }

        public override Model Copy()
        {
            return new DeterministicModel(A1, A2, B1, B2, Px, Py, D12, D21, MaxX);
        }

        protected override double F(double x1, double x2)
        {
            return Pb1 * (A1 * x1 * (B1 - Px * x1) + D12 * x2 * (B2 - Px * x2));
        }

        protected override double G(double x1, double x2)
        {
            return Pb2 * (A2 * x2 * (B2 - Px * x2) + D21 * x1 * (B1 - Px * x1));
        }
    }

    public class StochasticModel : Model
    {
        public double Eps { get; set; }
        public double Sigma1 { get; set; }
        public double Sigma2 { get; set; }
        public double Sigma3 { get; set; }

        private readonly NormalDistribution random = new NormalDistribution();

        public StochasticModel(double a1, double a2, double b1, double b2, double px, double py, double d12 = 0,
            double d21 = 0, double eps = 0, double sigma1 = 0, double sigma2 = 0, double sigma3 = 0, double maxX = 100)
            : base(a1, a2, b1, b2, px, py, d12, d21, maxX)
        {
            Eps = eps;
            Sigma1 = sigma1;
            Sigma2 = sigma2;
            Sigma3 = sigma3;
        }

        public override Model Copy()
        {
            return new StochasticModel(A1, A2, B1, B2, Px, Py, D12, D21, Eps, Sigma1, Sigma2, Sigma3, MaxX);
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
