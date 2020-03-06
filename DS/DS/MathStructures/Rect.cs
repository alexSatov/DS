namespace DS.MathStructures
{
    public struct Rect
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Top { get; set; }

        public Rect(double left, double right, double bottom, double top)
        {
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }

        public bool Contains(double x, double y)
        {
            return x >= Left && x <= Right && y >= Bottom && y <= Top;
        }

        public bool Contains(IPoint point)
        {
            return Contains(point.X, point.Y);
        }
    }
}
