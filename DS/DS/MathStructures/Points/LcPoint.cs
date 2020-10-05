namespace DS.MathStructures.Points
{
    public readonly struct LcPoint
    {
        public LcType LcType { get; }
        public int LcIndex { get; }
        public int Index { get; }
        public PointX Point { get; }

        public LcPoint(LcType lcType, int lcIndex, int index, PointX point)
        {
            LcType = lcType;
            LcIndex = lcIndex;
            Index = index;
            Point = point;
        }
    }
}
