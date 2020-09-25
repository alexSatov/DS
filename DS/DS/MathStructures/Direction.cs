using System.Collections.Generic;
using DS.MathStructures.Vectors;

namespace DS.MathStructures
{
    public static class Direction
    {
        public static Vector2D Up => new Vector2D(0, 1);
        public static Vector2D UpRight => new Vector2D(1, 1);
        public static Vector2D Right => new Vector2D(1, 0);
        public static Vector2D DownRight => new Vector2D(1, -1);
        public static Vector2D Down => new Vector2D(0, -1);
        public static Vector2D DownLeft => new Vector2D(-1, -1);
        public static Vector2D Left => new Vector2D(-1, 0);
        public static Vector2D UpLeft => new Vector2D(-1, 1);

        public static IEnumerable<Vector2D> GetAll()
        {
            yield return Up;
            yield return Right;
            yield return Down;
            yield return Left;
            yield return UpRight;
            yield return DownRight;
            yield return DownLeft;
            yield return UpLeft;
        }
    }
}
