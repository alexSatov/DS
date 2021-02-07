using System;
using Accord.Math;

namespace DS.Models
{
    /// <summary>
    /// Моя модель (А. Сатов)
    /// </summary>
    public abstract class NModel1 : BaseNModel
    {
        public double[] B_2 { get; }
        public double[,] D { get; }
        public double Phi { get; }

        protected NModel1(int n, double[] b, double[,] d, double px, double py) : base(n)
        {
            if (b.Length != n || d.GetLength(1) != n || d.GetLength(2) != n)
            {
                throw new ArgumentException($"All components of the model must be size of {n}");
            }

            B_2 = b.Pow(2);
            D = d;

            Phi = 1 / (px * py);
        }
    }
}
