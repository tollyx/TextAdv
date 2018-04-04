using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv
{
    public enum CommandType
    {
        None,
        Move,
        Use,
    }

    public interface ICommand
    {
        CommandType Type { get; }
        bool Execute(World world);
    }

    public static class Command
    {
        public static ICommand Parse(string input)
        {
            input = input?.Trim().ToLower();
            MoveCommand move = ParseDirection(input);
            if (move.Where != Direction.None)
            {
                return move;
            }
            return new NoneCommand();
        }

        static readonly (Direction, string[])[] DirectionStrings = {
            (Direction.North,       new string[]{ "north", "n" }),
            (Direction.West,        new string[]{ "west", "w" }),
            (Direction.East,        new string[]{ "east", "e" }),
            (Direction.NorthWest,   new string[]{ "northwest", "nw" }),
            (Direction.NorthEast,   new string[]{ "northeast", "ne" }),
            (Direction.SouthWest,   new string[]{ "southwest", "sw" }),
            (Direction.SouthEast,   new string[]{ "southeast", "se" }),
            (Direction.Up,          new string[]{ "up", "u" }),
            (Direction.Down,        new string[]{ "down", "d" }),
            (Direction.In,          new string[]{ "in" }),
            (Direction.Out,         new string[]{ "out" }),
        };

        public static MoveCommand ParseDirection(string input)
        {
            foreach (var item in DirectionStrings)
            {
                if (item.Item2.Contains(input))
                {
                    return new MoveCommand(item.Item1);
                }
            }
            return new MoveCommand(Direction.None);
        }
    }

    public class MoveCommand : ICommand
    {
        public CommandType Type { get { return CommandType.Move; } }
        public Direction Where { get; private set; }

        public MoveCommand(Direction dir)
        {
            Where = dir;
        }

        public bool Execute(World world)
        {
            if (world.Player.Move(Where))
            {
                return true;
            }
            Console.WriteLine("You can't go that way.");
            return false;
        }
    }

    public class NoneCommand : ICommand
    {
        public CommandType Type { get { return CommandType.None; } }

        public bool Execute(World world)
        {
            return false;
        }
    }
}
