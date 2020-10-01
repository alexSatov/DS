namespace DS.Models
{
    /// <summary>
    /// Детерминированная версия модели <see cref="Model3"/>
    /// </summary>
    public class DeterministicModel3 : Model3
    {
        public DeterministicModel3(double a, double b, double c, double d) : base(a, b, c, d)
        {
        }

        public override BaseModel Copy()
        {
            return new DeterministicModel3(A, B, C, D);
        }

        protected override double F(double x1, double x2)
        {
            return A * x1 * (1 - x1) - B * x1 * x2;
        }

        protected override double G(double x1, double x2)
        {
            return D * x1 * x2 - C * x2;
        }
    }
}
