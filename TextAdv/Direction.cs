using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv
{
    public enum Direction
    {
        North,
        South,
        East,
        West,
        In,
        Out,
        Up,
        Down,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
        None,
    }

    public static class Extensions
    {
        public static Direction Opposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                case Direction.In: return Direction.Out;
                case Direction.Out: return Direction.In;
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.NorthEast: return Direction.SouthWest;
                case Direction.NorthWest: return Direction.SouthEast;
                case Direction.SouthEast: return Direction.NorthWest;
                case Direction.SouthWest: return Direction.NorthEast;
                default: return Direction.None;
            }
        }
    }
}
