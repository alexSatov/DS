using System.Collections.Generic;

namespace DS.MathStructures
{
    public static class Direction
    {
        public static readonly Vector2D Up = new Vector2D(0, 1);
        public static readonly Vector2D UpRight = new Vector2D(1, 1);
        public static readonly Vector2D Right = new Vector2D(1, 0);
        public static readonly Vector2D DownRight = new Vector2D(1, -1);
        public static readonly Vector2D Down = new Vector2D(0, -1);
        public static readonly Vector2D DownLeft = new Vector2D(-1, -1);
        public static readonly Vector2D Left = new Vector2D(-1, 0);
        public static readonly Vector2D UpLeft = new Vector2D(-1, 1);
    }

    public static class Directions
    {
        public static List<Vector2D> All =>
            new List<Vector2D>
            {
                Direction.Up,
                Direction.Right,
                Direction.Down,
                Direction.Left,
                Direction.UpRight,
                Direction.DownRight,
                Direction.DownLeft,
                Direction.UpLeft
            };
    }
}
