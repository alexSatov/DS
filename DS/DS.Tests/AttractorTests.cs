using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace DS.Tests
{
    [TestFixture]
    public class AttractorTests
    {
        [Test]
        public void Equilibrium()
        {
            var points = new[] { new double[] { 1, 1 } };
            var attractor = Attractor.From(points, "", double.Epsilon);

            attractor.Type.Should().Be(AttractorType.Equilibrium);
            attractor.Points.Should().BeEquivalentTo(new[] { new double[] { 1, 1 } });
            attractor.Params.Should().Be("");
        }

        [Test]
        public void Equilibrium2()
        {
            var points = new[] { new double[] { 1, 1 }, new double[] { 1, 1 } };
            var attractor = Attractor.From(points, "", double.Epsilon);

            attractor.Type.Should().Be(AttractorType.Equilibrium);
            attractor.Points.Should().BeEquivalentTo(new[] { new double[] { 1, 1 } });
            attractor.Params.Should().Be("");
        }

        [Test]
        public void Infinity()
        {
            var points = new[] { new[] { double.MaxValue, double.MaxValue }, new[] { double.MaxValue, double.MaxValue } };
            var attractor = Attractor.From(points, "", double.Epsilon);

            attractor.Type.Should().Be(AttractorType.Infinity);
            attractor.Points.Should().BeEquivalentTo(new[] { new[] { double.MaxValue, double.MaxValue } });
            attractor.Params.Should().Be("");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CyclesAndChaos(bool loop)
        {
            for (var period = 2; period < Attractor.MaxCyclePeriod + 1; period++)
            {
                var points = Enumerable.Range(0, period + 1)
                    .Select(i => new double[] { i, i })
                    .ToArray();

                if (loop)
                    points[^1] = points[0];

                var cycle = Attractor.From(points, "", double.Epsilon);

                if (loop)
                {
                    cycle.Type.Should().Be((AttractorType) period);
                    cycle.Points.Should().BeEquivalentTo(points[..^1]);
                }
                else
                {
                    cycle.Type.Should().Be(AttractorType.Chaos);
                    cycle.Points.Should().BeEquivalentTo(points);
                }

                cycle.Params.Should().Be("");
            }
        }

        [Test]
        public void Chaos()
        {
            var points = Enumerable.Range(0, Attractor.MaxCyclePeriod + 2)
                .Select(i => new double[] { i, i })
                .ToArray();

            points[^1] = points[0]; // trying to loop

            var chaos = Attractor.From(points, "", double.Epsilon);

            chaos.Type.Should().Be(AttractorType.Chaos);
            chaos.Points.Should().BeEquivalentTo(points);
            chaos.Params.Should().Be("");
        }
    }
}
