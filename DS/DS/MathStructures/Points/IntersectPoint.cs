namespace DS.MathStructures.Points
{
    public readonly struct IntersectPoint
    {
        public bool IsIntersect { get; }
        public PointX? Point { get; }

        public IntersectPoint(bool isIntersect, PointX? point)
        {
            IsIntersect = isIntersect;
            Point = point;
        }
    }
}
