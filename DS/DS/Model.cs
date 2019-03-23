using Accord.Statistics.Distributions.Univariate;

namespace DS
{
	public abstract class Model
	{
		public double A1 { get; set; }
		public double A2 { get; set; }
		public double B1 { get; set; }
		public double B2 { get; set; }
		public double Px { get; set; }
		public double Py { get; set; }
		public double D12 { get; set; }
		public double D21 { get; set; }
		public double MaxX { get; set; }

		protected Model(double maxX = 100)
		{
			MaxX = maxX;
		}

		public PointX GetNextPoint(PointX current)
		{
			return new PointX(F(current.X1, current.X2), G(current.X1, current.X2));
		}

		public abstract Model Copy();

		protected abstract double F(double x1, double x2);
		protected abstract double G(double x1, double x2);
	}

	public class DeterministicModel : Model
	{
		public override Model Copy()
		{
			return new DeterministicModel
			{
				A1 = A1,
				A2 = A2,
				B1 = B1,
				B2 = B2,
				Px = Px,
				Py = Py,
				D12 = D12,
				D21 = D21,
				MaxX = MaxX
			};
		}

		protected override double F(double x1, double x2)
		{
			return B1 * (A1 * x1 * (B1 - Px * x1) + D12 * x2 * (B2 - Px * x2)) / (Px * Py);
		}

		protected override double G(double x1, double x2)
		{
			return B2 * (A2 * x2 * (B2 - Px * x2) + D21 * x1 * (B1 - Px * x1)) / (Px * Py);
		}
	}

	public class StochasticModel : Model
	{
		public double Eps { get; set; }
		public double Sigma1 { get; set; }
		public double Sigma2 { get; set; }
		public double Sigma3 { get; set; }

		private readonly NormalDistribution random = new NormalDistribution();

		public override Model Copy()
		{
			return new StochasticModel
			{
				A1 = A1,
				A2 = A2,
				B1 = B1,
				B2 = B2,
				Px = Px,
				Py = Py,
				D12 = D12,
				D21 = D21,
				MaxX = MaxX,
				Eps = Eps,
				Sigma1 = Sigma1,
				Sigma2 = Sigma2,
				Sigma3 = Sigma3
			};
		}

		protected override double F(double x1, double x2)
		{
			var pb = B1 / (Px * Py);
			return pb * (A1 * x1 * (B1 - Px * x1) + D12 * x2 * (B2 - Px * x2)) + Eps * Sigma1 * random.Generate()
				+ pb * D12 * x2 * Sigma3 * Eps * random.Generate();
		}

		protected override double G(double x1, double x2)
		{
			return B2 * (A2 * x2 * (B2 - Px * x2) + D21 * x1 * (B1 - Px * x1)) / (Px * Py)
				+ Eps * Sigma2 * random.Generate();
		}
	}
}
