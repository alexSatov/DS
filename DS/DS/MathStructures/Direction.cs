using System;
using System.Collections.Generic;
using System.Linq;

namespace DS.MathStructures
{
    public enum Direction
    {
        Up, Right, Down, Left
    }

    public static class Directions
    {
        public static IEnumerable<Direction> All => Enum.GetValues(typeof(Direction)).Cast<Direction>();

        public static T ChooseHorizontally<T>(Direction direction, T left, T right, T defaultValue)
        {
            return direction switch
            {
                Direction.Left => left,
                Direction.Right => right,
                _ => defaultValue
            };
        }

        public static T ChooseVertically<T>(Direction direction, T down, T up, T defaultValue)
        {
            return direction switch
            {
                Direction.Down => down,
                Direction.Up => up,
                _ => defaultValue
            };
        }
    }
}
