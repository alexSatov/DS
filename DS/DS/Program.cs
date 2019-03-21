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

			var model = new Model { A1 = 0.0002, A2 = 0.00052, B1 = 10, B2 = 20, Px = 0.25, Py = 1 };
			var watch = new Stopwatch();

			watch.Start();
			var chart = Test.Test7(model);
			watch.Stop();

			Console.WriteLine(watch.Elapsed);
			//Console.ReadKey();

			if (chart != null)
				Application.Run(chart);
		}
	}
}
