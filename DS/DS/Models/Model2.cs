using DS.MathStructures.Points;

namespace DS.Models
{
    /// <inheritdoc cref="IModel"/>
    public abstract class Model2 : IModel
    {
        public double A { get; set; }
        public double B { get; set; }

        /// <inheritdoc />
        public double AbsInf => 100;

        protected Model2(double a, double b)
        {
            A = a;
            B = b;
        }

        /// <inheritdoc />
        public PointX GetNextPoint(PointX current)
        {
            return new PointX(F(current.X1, current.X2), G(current.X1, current.X2));
        }

        /// <inheritdoc />
        public abstract IModel Copy();

        protected abstract double F(double x1, double x2);
        protected abstract double G(double x1, double x2);
    }
}
