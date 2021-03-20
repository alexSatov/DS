using System;

namespace DS.MathStructures
{
    public readonly struct Interval<T>
        where T : IComparable
    {
        public T Start { get; init; }
        public T End { get; init; }

        public IntervalDirections Direction => (IntervalDirections) Start.CompareTo(End);

        public T Min => Direction == IntervalDirections.Desc ? End : Start;
        public T Max => Direction == IntervalDirections.Desc ? Start : End;

        public Interval(T start, T end)
        {
            Start = start;
            End = end;
        }
    }

    public enum IntervalDirections
    {
        Asc = -1,
        None = 0,
        Desc = 1
    }
}
