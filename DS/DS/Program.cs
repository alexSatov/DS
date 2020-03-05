using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DS
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var deterministicModel = new DeterministicModel(0.0002, 0.00052, 10, 20, 0.25, 1);
            var stochasticModel = new StochasticModel(0.0002, 0.00052, 10, 20, 0.25, 1);
            var watch = new Stopwatch();

            watch.Start();
            var chart = DeterministicTest.Test13(deterministicModel);
            //var chart = StochasticTest.Test8_3(deterministicModel, stochasticModel);
            watch.Stop();

            Console.WriteLine(watch.Elapsed);

            if (chart != null)
                Application.Run(chart);
            else
                Console.ReadKey();
        }

        private static void Test()
        {
        }
    }
}
