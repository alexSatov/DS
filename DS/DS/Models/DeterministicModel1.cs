namespace DS.Models
{
    /// <summary>
    /// Детерминированная версия модели <see cref="Model1"/>
    /// </summary>
    public class DeterministicModel1 : Model1
    {
        public DeterministicModel1(double a1, double a2, double b1, double b2, double px, double py,
            double d12 = 0, double d21 = 0) : base(a1, a2, b1, b2, px, py, d12, d21)
        {
        }

        /// <inheritdoc />
        public override IModel Copy()
        {
            return new DeterministicModel1(A1, A2, B1, B2, Px, Py, D12, D21);
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
}
