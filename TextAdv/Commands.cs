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

        delegate ICommand CommandDelegate(string[] args, World world);

        static readonly Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>{
            { "pick", PickUpCommand.Parse },
            { "take", PickUpCommand.Parse },
            { "ta", PickUpCommand.Parse },
            { "get", PickUpCommand.Parse },
            { "grab", PickUpCommand.Parse },

            { "dr", DropCommand.Parse },
            { "drop",  DropCommand.Parse },

            { "i", InventoryCommand.Parse },
            { "inv", InventoryCommand.Parse },
            { "inventory", InventoryCommand.Parse },
            { "bag", InventoryCommand.Parse },

            { "drink", ConsumeCommand.Parse },
            { "dri", ConsumeCommand.Parse },
            { "eat", ConsumeCommand.Parse },
            { "consume", ConsumeCommand.Parse },

            { "we", EquipCommand.Parse },
            { "wear", EquipCommand.Parse },
            { "eq", EquipCommand.Parse },
            { "equip", EquipCommand.Parse },

            //{ "re", UnequipCommand.Parse },
            //{ "remove", UnequipCommand.Parse },
            //{ "uneq", UnequipCommand.Parse },
            //{ "unequip", UnequipCommand.Parse },

            //{ "op", OpenCommand.Parse },
            //{ "open", OpenCommand.Parse },
            //{ "cl", OpenCommand.Parse}, 
            //{ "close", OpenCommand.Parse },

            { "l", LookCommand.Parse },
            { "look", LookCommand.Parse },
            { "here", LookCommand.Parse },
            { "ls", LookCommand.Parse },

            { "clear", ClearCommand.Parse },

        };

        /// <summary>
        /// Parses input from the user and spits out the proper command.
        /// </summary>
        /// <param name="input">User input</param>
        /// <param name="world">The world the command will be executed in.</param>
        /// <returns>The parsed command. Returns null if invalid input was entered.</returns>
        public static ICommand Parse(string input, World world) {
            string[] args = input.ToLower().Split().Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            if (args.Length == 0) return null;

            string cmd = args[0];
            args = args.Skip(1).ToArray();

            ICommand move = MoveCommand.Parse(cmd);
            if (move != null) {
                return move;
            }

            if (commands.ContainsKey(cmd)) {
                return commands[cmd](args, world);
            }
            return null;
        }
    }

    public class MoveCommand : ICommand {
        public Direction Where { get; private set; }

        static readonly (Direction, string[])[] DirectionStrings = {
            (Direction.North,       new string[]{ "n", "north" }),
            (Direction.South,       new string[]{ "s", "south" }),
            (Direction.West,        new string[]{ "w", "west" }),
            (Direction.East,        new string[]{ "e", "east" }),
            (Direction.NorthWest,   new string[]{ "nw", "northwest" }),
            (Direction.NorthEast,   new string[]{ "ne", "northeast" }),
            (Direction.SouthWest,   new string[]{ "sw", "southwest" }),
            (Direction.SouthEast,   new string[]{ "se", "southeast" }),
            (Direction.Up,          new string[]{ "u", "up" }),
            (Direction.Down,        new string[]{ "d", "down" }),
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
            Program.Say("You can't go that way.");
            return false;
        }
    }

    public class PickUpCommand : ICommand {
        public IItem Item { get; private set; }

        public PickUpCommand(IItem item) {
            Item = item;
        }

        public static ICommand Parse(string[] args, World world) {
            if (args.Length > 0) {
                if (args.Length == 1 && args[0] == "all") {
                    return new PickUpAllCommand();
                }
                string name = String.Join(" ", args);

                IItem item = world.Player.CurrentPosition.FindItem(name);
                if (item != null) return new PickUpCommand(item);

                Program.Say($"You can't see any {name}");
            }
            else {
                Program.Say("You need to specify what you want to pick up.");
            }
            return null;
        }

        public bool Execute(World world) {
            return Item.PickUp(world.Player);
        }
    }

    public class PickUpAllCommand : ICommand {
        public bool Execute(World world) {
            bool shouldPassTime = false;
            var list = world.Player.CurrentPosition.Inventory.ToList();
            foreach (var item in list) {
                shouldPassTime = item.PickUp(world.Player) || shouldPassTime;
            }
            return shouldPassTime;
        }
    }

    public class DropCommand : ICommand {
        public IItem Item { get; private set; }

        public DropCommand(IItem item) {
            Item = item;
        }

        public static ICommand Parse(string[] args, World world) {
            if (args.Length > 0) {
                string cmd = String.Join(" ", args);

                IItem item = world.Player.FindItem(cmd);
                if (item != null) return new DropCommand(item);

                Program.Say($"You don't have a {cmd}");
            }
            else {
                Program.Say("You need to specify what you want to drop.");
            }
            return null;
        }

        public bool Execute(World world) {
            return Item.Drop(world.Player);
        }
    }

    public class ConsumeCommand : ICommand {
        public IConsumable Item { get; private set; }

        public ConsumeCommand(IConsumable item) {
            Item = item;
        }

        public static ICommand Parse(string[] args, World world) {
            if (args.Length > 0) {
                string name = String.Join(" ", args);

                IItem item = world.Player.FindItem(name);
                if (item != null) {
                    if (item is IConsumable) {
                        return new ConsumeCommand(item as IConsumable);
                    }
                    else {
                        Program.Say($"You can't eat the {item.Name}.");
                    }
                }
                else {
                    Program.Say($"You don't have any {name}.");
                }
            }
            else {
                Program.Say("You need to specify what you want to pick up.");
            }
            return null;
        }

        public bool Execute(World world) {
            return Item.Consume(world.Player);
        }
    }

    public class EquipCommand : ICommand {
        public IEquippable Item { get; private set; }

        public EquipCommand(IEquippable item) {
            Item = item;
        }

        public static ICommand Parse(string[] args, World world) {
            if (args.Length > 0) {
                string name = String.Join(" ", args);

                IItem item = world.Player.FindItem(name);
                if (item != null) {
                    if (item is IEquippable) {
                        return new EquipCommand(item as IEquippable);
                    }
                    else {
                        Program.Say($"You can't equip the {item.Name}.");
                    }
                }
                else {
                    Program.Say($"You don't have any {name}.");
                }
            }
            else {
                Program.Say("You need to specify what you want to wear.");
            }
            return null;
        }

        public bool Execute(World world) {
            return Item.Equip(world.Player);
        }
    }

    public class UnequipCommand : ICommand {
        public IEquippable Item { get; private set; }

        public bool Execute(World world) {
            return Item.Unequip(world.Player);
        }
    }

    public class InventoryCommand : ICommand {
        public static InventoryCommand Parse(string[] args, World world) {
            return new InventoryCommand();
        }

        public bool Execute(World world) {
            Print(world.Player);
            return false;
        }

        public static void Print(IActor act) {
            Program.Say("Inventory:");
            foreach (var item in act.Inventory) {
                Program.Say(item.Name);
            }
        }
    }

    public class LookCommand : ICommand {
        public static ICommand Parse(string[] args, World world) {
            return new LookCommand();
        }

        public bool Execute(World world) {
            Print(world.Player.CurrentPosition);
            return false;
        }

        public static void Print(MapNode location) {
            Program.Say($"You are here: {location.Name}");
            Program.Say(location.Description);

            string items = "Items: " + string.Join(", ", location.Inventory);
            Program.Say(items);

            string dirs = "Paths: " + string.Join(", ", location.GetDirections());

            Program.Say(dirs);
        }
    }

    public class ClearCommand : ICommand {

        public static ICommand Parse(string[] args, World world) {
            return new ClearCommand();
        }

        public bool Execute(World world) {
            Program.Clear();
            return false;
        }
    }
}
