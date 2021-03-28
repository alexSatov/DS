using System;
using System.Linq;
using DS.Helpers;
using DS.Models;

namespace DS.ConsoleApp
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Console.WriteLine(string.Join("; ", args));
            Console.WriteLine($"Processor count: " + Environment.ProcessorCount);

            var model = GetModel_2();

            var points = PhaseTrajectory.Get(model, new[] { 0.5, 0.5 }, 2000, 1000)
                .Select(p => (p[0], p[1]));

            points.SaveToFile("zik.txt");
        }

        private static DeterministicNModel1 GetModel_2(double d12 = 0.0014, double d21 = 0.0075)
        {
            var d = new[,]
            {
                { 0.0002, d12 },
                { d21, 0.00052 }
            };

            return new DeterministicNModel1(2, new double[] { 10, 20 }, d, 0.25, 1);
        }
    }
}
