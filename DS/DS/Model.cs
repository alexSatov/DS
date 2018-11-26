namespace DS
{
	public class Model
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

		public Model(double maxX = 100)
		{
			MaxX = maxX;
		}

		public PointX GetNextPoint(PointX current)
		{
			return new PointX(F(current.X1, current.X2), G(current.X1, current.X2));
		}

		public Model Copy()
		{
			return new Model
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

		private double F(double x1, double x2)
		{
			return B1 * (A1 * x1 * (B1 - Px * x1) + D12 * x2 * (B2 - Px * x2)) / (Px * Py);
		}

		private double G(double x1, double x2)
		{
			return B2 * (A2 * x2 * (B2 - Px * x2) + D21 * x1 * (B1 - Px * x1)) / (Px * Py);
		}
	}
}
