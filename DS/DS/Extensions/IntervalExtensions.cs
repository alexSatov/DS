using System;
using System.Collections.Generic;
using System.Linq;
using DS.MathStructures;

namespace DS.Extensions
{
    public static class IntervalExtensions
    {
        public static IEnumerable<double> Range(this Interval<double> interval, int count)
        {
            if (interval.Direction == IntervalDirections.None)
                throw new ArgumentException("Must have direction", nameof(interval));
            if (count < 2)
                throw new ArgumentException("Must be greater than 1", nameof(count));

            var step = (interval.Max - interval.Min) / (count - 1);

            return interval.Direction == IntervalDirections.Asc
                ? Enumerable.Range(0, count).Select(i => interval.Start + i * step)
                : Enumerable.Range(0, count).Select(i => interval.Start - i * step);
        }
    }
}
