using System;
using System.Diagnostics;
using System.Windows.Forms;
using NUnit.Framework;

namespace DS.Tests.Charts
{
    [TestFixture, Explicit]
    public abstract class ChartTests
    {
        protected ChartForm Chart { get; set; }

        private Stopwatch watch;

        [SetUp]
        [STAThread]
        public void SetUp()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OnSetUp();

            watch = Stopwatch.StartNew();
        }

        [TearDown]
        public void TearDown()
        {
            watch.Stop();

            Console.WriteLine(watch.Elapsed);

            if (Chart != null)
                Application.Run(Chart);
            else
                Console.ReadKey();
        }

        protected abstract void OnSetUp();
    }
}
