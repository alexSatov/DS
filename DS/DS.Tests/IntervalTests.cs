﻿using DS.MathStructures;
using FluentAssertions;
using NUnit.Framework;

namespace DS.Tests
{
    [TestFixture]
    public class IntervalTests
    {
        [Test]
        public void CorrectMinMax()
        {
            var interval = new Interval<int>(1, 3);

            interval.Direction.Should().Be(IntervalDirections.Asc);
            interval.Start.Should().Be(1);
            interval.End.Should().Be(3);
            interval.Min.Should().Be(1);
            interval.Max.Should().Be(3);

            interval = new Interval<int>(1, 1);

            interval.Direction.Should().Be(IntervalDirections.None);
            interval.Start.Should().Be(1);
            interval.End.Should().Be(1);
            interval.Min.Should().Be(1);
            interval.Max.Should().Be(1);

            interval = new Interval<int> { Start = 4, End = 2 };

            interval.Direction.Should().Be(IntervalDirections.Desc);
            interval.Start.Should().Be(4);
            interval.End.Should().Be(2);
            interval.Min.Should().Be(2);
            interval.Max.Should().Be(4);
        }
    }
}
