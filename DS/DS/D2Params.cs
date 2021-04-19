using DS.MathStructures;

namespace DS
{
    public record D2Params(Interval<double> IntervalX, Interval<double> IntervalY,
        int Dxi = 0, int Dxj = 1, int Dyi = 1, int Dyj = 0, int CountX = 400, int CountY = 400,
        ByPreviousType ByPreviousType = ByPreviousType.X);
}
