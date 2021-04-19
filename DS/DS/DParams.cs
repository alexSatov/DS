using DS.MathStructures;

namespace DS
{
    public record DParams(Interval<double> Interval, int Di = 0, int Dj = 1, int Count = 400, bool ByPrevious = false);
}
