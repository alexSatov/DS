using DS.MathStructures.Points;

namespace DS.Models
{
    public abstract class Model1 : IModel
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

        public double AbsInf => 100;

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

        public PointX GetNextPoint(PointX current)
        {
            return new PointX(F(current.X1, current.X2), G(current.X1, current.X2));
        }

        public abstract IModel Copy();

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
}
