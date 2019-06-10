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
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var deterministicModel = new DeterministicModel(0.0002, 0.00052, 10, 20, 0.25, 1);
			var stochasticModel = new StochasticModel(0.0002, 0.00052, 10, 20, 0.25, 1);
			var watch = new Stopwatch();

			watch.Start();
			//var chart = DeterministicTest.Test6(deterministicModel);
			var chart = StochasticTest.Test4(deterministicModel, stochasticModel);
			watch.Stop();

			Console.WriteLine(watch.Elapsed);
			//Console.ReadKey();

			if (chart != null)
				Application.Run(chart);
		}

		private static void Test()
		{
		}
	}
}
