using DS.MathStructures;

namespace DS
{
    public record D2Params(Interval<double> IntervalX, Interval<double> IntervalY,
        int Dxi, int Dxj, int Dyi, int Dyj, ByPreviousType ByPreviousType);
}
