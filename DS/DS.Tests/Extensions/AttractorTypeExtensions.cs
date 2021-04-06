using System;
using System.Drawing;

namespace DS.Tests.Extensions
{
    public static class AttractorTypeExtensions
    {
        public static Color ToColor(this AttractorType type)
        {
            return type switch
            {
                AttractorType.Infinity => Color.Gray,
                AttractorType.Chaos => Color.White,
                AttractorType.Equilibrium => Color.DeepSkyBlue,
                AttractorType.Cycle2 => Color.DarkRed,
                AttractorType.Cycle3 => Color.Red,
                AttractorType.Cycle4 => Color.DarkOrange,
                AttractorType.Cycle5 => Color.Orange,
                AttractorType.Cycle6 => Color.Goldenrod,
                AttractorType.Cycle7 => Color.Yellow,
                AttractorType.Cycle8 => Color.SpringGreen,
                AttractorType.Cycle9 => Color.Green,
                AttractorType.Cycle10 => Color.DarkGreen,
                AttractorType.Cycle11 => Color.Blue,
                AttractorType.Cycle12 => Color.DarkBlue,
                AttractorType.Cycle13 => Color.Violet,
                AttractorType.Cycle14 => Color.DarkViolet,
                AttractorType.Cycle15 => Color.Magenta,
                AttractorType.Cycle16 => Color.SaddleBrown,
                var attractorType => throw new ArgumentOutOfRangeException(nameof(attractorType), attractorType,
                    "Unknown type")
            };
        }
    }
}
