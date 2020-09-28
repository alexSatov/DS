namespace DS.Models
{
    /// <summary>
    /// Детерминированная версия модели <see cref="Model1"/>
    /// </summary>
    public class DeterministicModel2 : Model2
    {
        public DeterministicModel2(double a, double b) : base(a, b)
        {
        }

        /// <inheritdoc />
        public override IModel Copy()
        {
            return new DeterministicModel2(A, B);
        }

        protected override double F(double x1, double x2)
        {
            return 1 - A * x2 * x2 + B * x1;
        }

        protected override double G(double x1, double x2)
        {
            return x1;
        }
    }
}
