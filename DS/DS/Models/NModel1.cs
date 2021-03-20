using System;
using Accord.Math;

namespace DS.Models
{
    /// <summary>
    /// Моя модель (А. Сатов)
    /// </summary>
    /// <example>
    /// замена переменной была линейная:
    /// px x1/I1 = w1
    /// px x2/I2 = w2
    /// по идее на параметры она не влияет.
    /// произошла замена имен параметров:
    /// b1 -> I1 = 10
    /// b2 -> I2 = 20
    /// alpha1 -> alpha11 = 0.0002
    /// alpha2 -> alpha22 = 0.00052
    /// D12 -> alpha12
    /// D21 -> alpha21
    /// и переменные w1, w2 должны быть на (0,1)
    /// </example>
    public abstract class NModel1 : BaseNModel
    {
        public double[] B { get; }
        public double[] B_2 { get; }
        public double[,] D { get; }
        public double Px { get; }
        public double Py { get; }
        public double Phi { get; }

        public override double AbsInf => 2;

        protected NModel1(int n, double[] b, double[,] d, double px, double py) : base(n)
        {
            if (b.Length != n || d.GetLength(0) != n || d.GetLength(1) != n)
            {
                throw new ArgumentException($"All components of the model must be size of {n}");
            }

            B = b;
            B_2 = b.Pow(2);
            D = d;
            Px = px;
            Py = py;
            Phi = 1 / (px * py);
        }
    }
}
