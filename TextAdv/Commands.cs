using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdv.Items;

namespace TextAdv {
    public interface ICommand {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="world">The world to execute the command in.</param>
        /// <returns>Wether we should pass forward the world one tick.</returns>
        bool Execute(World world);
    }

    public static class Command {
        /// <summary>
        /// Parses input from the user and spits out the proper command.
        /// </summary>
        /// <param name="input">User input</param>
        /// <param name="world">The world the command will be executed in.</param>
        /// <returns>The parsed command. Returns null if invalid input was entered.</returns>
        public static ICommand Parse(string input, World world) {
            if (input == null) return null;

            input = input.Trim().ToLower();
            ICommand cmd = MoveCommand.Parse(input);
            if (cmd != null) {
                return cmd;
            }
            cmd = InventoryCommand.Parse(input);
            if (cmd != null) {
                return cmd;
            }
            cmd = PickUpCommand.Parse(input, world.Player);
            if (cmd != null) {
                return cmd;
            }
            cmd = DropCommand.Parse(input, world.Player);
            if (cmd != null) {
                return cmd;
            }
            return null;
        }
    }

    public class MoveCommand : ICommand {
        public Direction Where { get; private set; }

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

        public static MoveCommand Parse(string input) {
            foreach (var item in DirectionStrings) {
                if (item.Item2.Contains(input)) {
                    return new MoveCommand(item.Item1);
                }
            }
            return null;
        }

        public MoveCommand(Direction dir) {
            Where = dir;
        }

        public bool Execute(World world) {
            if (world.Player.Move(Where)) {
                return true;
            }
            Console.WriteLine("You can't go that way.");
            return false;
        }
    }

    public class PickUpCommand : ICommand {
        public IItem Item { get; private set; }

        public PickUpCommand(IItem item) {
            Item = item;
        }

        static readonly string[] pickUpStrings = { "pick up", "take" };

        public static PickUpCommand Parse(string input, PlayerActor player) {
            foreach (var cmd in pickUpStrings) {
                if (input.StartsWith(cmd)) {
                    input = input.Substring(cmd.Length).Trim();

                    foreach (var item in player.CurrentPosition.GetItems()) {
                        if (item.Name.ToLower().Contains(input)) {
                            return new PickUpCommand(item);
                        }
                    }
                    return null;
                }
            }
            return null;
        }

        public bool Execute(World world) {
            return Item.PickUp(world.Player);
        }
    }

    public class DropCommand : ICommand {
        static readonly string[] dropStrings = { "dr", "drop" };

        public IItem Item { get; private set; }

        public DropCommand(IItem item) {
            Item = item;
        }

        public static DropCommand Parse(string input, PlayerActor player) {
            string[] cmd = input.Split();
            if (cmd.Length > 0 && dropStrings.Contains(cmd[0])) {
                if (cmd.Length == 1) {
                    Console.WriteLine("You need to specify what you want to drop.");
                    return null;
                }
                foreach (var item in player.Inventory) {
                    bool success = true;
                    for (int i = 1; i < cmd.Length; i++) {
                        if (!item.Name.ToLower().Contains(cmd[i])) {
                            success = false;
                            break;
                        }
                    }
                    if (success) {
                        return new DropCommand(item);
                    }
                    else {
                        string msg = "You don't have a";
                        for (int i = 1; i < cmd.Length; i++) {
                            msg += " " + cmd[i];
                        }
                        Console.WriteLine(msg);
                    }
                }
            }
            return null;
        }

        public bool Execute(World world) {
            return Item.Drop(world.Player);
        }
    }

    public class ConsumeCommand : ICommand {
        public IConsumable Item { get; private set; }

        public bool Execute(World world) {
            return Item.Consume(world.Player);
        }
    }

    public class EquipCommand : ICommand {
        public IEquipment Item { get; private set; }

        public bool Execute(World world) {
            return Item.Equip(world.Player);
        }
    }

    public class ThrowCommand : ICommand {
        public Actor Target { get; private set; }
        public IItem Item { get; private set; }

        public bool Execute(World world) {
            return Item.Throw(world.Player, Target);
        }
    }

    public class InventoryCommand : ICommand {
        static readonly string[] inventoryStrings = { "i", "inv", "inventory" };

        public static InventoryCommand Parse(string input) {
            if (inventoryStrings.Contains(input)) {
                return new InventoryCommand();
            }
            return null;
        }

        public bool Execute(World world) {
            Console.WriteLine("Inventory:");
            foreach (var item in world.Player.Inventory) {
                Console.WriteLine(item.Name);
            }
            return true;
        }
    }
}
