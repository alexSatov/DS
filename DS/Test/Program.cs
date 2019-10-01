using System;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Test().GetAwaiter().GetResult();
		}

		public static async Task Test()
		{
			var task1 = FirstTaskAsync().ConfigureAwait(false);
			var task2 = SecondTaskAsync().ConfigureAwait(false);

			await task1;
			await task2;
		}

		public static async Task FirstTaskAsync()
		{
			Console.WriteLine($"First task waiting...{DateTime.Now}");
			Thread.Sleep(10000);
			await Task.Delay(0).ConfigureAwait(false);
		}

		public static async Task SecondTaskAsync()
		{
			Console.WriteLine($"Second task completed...{DateTime.Now}");
			await Task.Delay(0).ConfigureAwait(false);
		}
	}
}
