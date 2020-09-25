using DS.MathStructures.Points;

namespace DS.MathStructures
{
    public readonly struct Rect
    {
        public double Left { get; }
        public double Right { get; }
        public double Bottom { get; }
        public double Top { get; }

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
