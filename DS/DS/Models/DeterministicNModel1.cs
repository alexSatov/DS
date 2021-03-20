namespace DS.Models
{
    /// <summary>
    /// Детерминированная версия модели <see cref="NModel1"/>
    /// </summary>
    public class DeterministicNModel1 : NModel1
    {
        public DeterministicNModel1(int n, double[] b, double[,] d, double px, double py) : base(n, b, d, px, py)
        {
        }

        public override BaseNModel Copy()
        {
            return new DeterministicNModel1(N, B, D, Px, Py);
        }

        protected override double F_i(double[] x, int i)
        {
            var sum = 0.0;
            for (var j = 0; j < N; j++)
            {
                sum += D[i, j] * B_2[j] * x[j] * (1 - x[j]);
            }

            return Phi * sum;
        }
    }
}
