using System;
using System.Diagnostics;
using System.Windows.Forms;
using Accord.Math;

namespace DS
{
	public class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var a1 = Math.Atan2(1, 1);
			var a2 = Math.Atan2(1, -1);
			var a3 = Math.Atan2(-1, -1) + 2 * Math.PI;
			var a4 = Math.Atan2(-1, 1) + 2 * Math.PI;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var deterministicModel = new DeterministicModel(0.0002, 0.00052, 10, 20, 0.25, 1);
			var stochasticModel = new StochasticModel(0.0002, 0.00052, 10, 20, 0.25, 1);
			var watch = new Stopwatch();

			watch.Start();
			//var chart = DeterministicTest.Test1(deterministicModel);
			var chart = StochasticTest.Test5(deterministicModel, stochasticModel);
			watch.Stop();

			Console.WriteLine(watch.Elapsed);
			//Console.ReadKey();

			if (chart != null)
				Application.Run(chart);
		}

		private static void Test()
		{
			var a1 = new double[] { 1, 2 };
			var a2 = new double[] { 3, 4 };
			var result = a1.Outer(a2);
		}
	}
}
