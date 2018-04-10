using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdv.Items;

namespace TextAdv {
    /// <summary>
    /// A command that can be executed.
    /// All needed parameters should have been passed in its constructor.
    /// </summary>
    public interface ICommand {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="world">The world to execute the command in.</param>
        /// <returns>Wether we should pass forward the world one tick.</returns>
        bool Execute(World world);
    }

    /// <summary>
    /// Static class for general command methods and utilities.
    /// </summary>
    public static class Command {

        delegate ICommand CommandDelegate(string[] args, World world);

        // Iterating over this abomination is apparently faster than a dict lookup?!
        // Yes, I benchmarked it myself, but it was probably an unreliable result.
        static readonly (string[], CommandDelegate)[] commands = {
            (new string[]{ "pick", "take", "ta", "get", "grab" }, PickUpCommand.Parse),
            (new string[]{ "dr", "drop" },  DropCommand.Parse),
            (new string[]{ "i", "inv", "inventory", "bag" }, InventoryCommand.Parse ),
            (new string[]{ "dri", "drink", "eat", "consume" }, ConsumeCommand.Parse ),
            (new string[]{ "we", "wear", "eq", "equip" }, EquipCommand.Parse ),
            //(new string[]{ "re", "remove", "uneq", "unequip" }, UnequipCommand.Parse),
            //(new string[]{ "op", "open", "cl", "close" }, OpenCommand.Parse ),
            (new string[]{ "l", "look", "here", "ls" }, LookCommand.Parse ),
            (new string[]{ "clear" }, ClearCommand.Parse ),
        };

        /// <summary>
        /// Parses input from the user and returns the corresponding command, if any was found.
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

            foreach (var item in commands) {
                if (item.Item1.Contains(cmd)) {
                    return item.Item2(args, world);
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Moves the player character in a direction.
    /// </summary>
    public class MoveCommand : ICommand {
        public Direction Where { get; private set; }

        static readonly (string[], Direction)[] DirectionStrings = {
            (new string[]{ "n", "north" }, Direction.North),
            (new string[]{ "s", "south" }, Direction.South),
            (new string[]{ "w", "west" }, Direction.West),
            (new string[]{ "e", "east" }, Direction.East),
            (new string[]{ "nw", "northwest" }, Direction.NorthWest),
            (new string[]{ "ne", "northeast" }, Direction.NorthEast),
            (new string[]{ "sw", "southwest" }, Direction.SouthWest),
            (new string[]{ "se", "southeast" }, Direction.SouthEast),
            (new string[]{ "u", "up" }, Direction.Up),
            (new string[]{ "d", "down" }, Direction.Down),
            (new string[]{ "in" }, Direction.In),
            (new string[]{ "out" }, Direction.Out),
        };

        public static MoveCommand Parse(string input) {
            foreach (var item in DirectionStrings) {
                if (item.Item1.Contains(input)) {
                    return new MoveCommand(item.Item2);
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

    /// <summary>
    /// Command for the item to pick up an item
    /// </summary>
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

                IItem item = world.Player.Location.FindItem(name);
                if (item != null) return new PickUpCommand(item);

                Program.Say($"You can't see any {name}");
            }
            else {
                Program.Say("You need to specify what you want to pick up.");
            }
            return null;
        }

        public bool Execute(World world) {
            if (Item.Location != world.Player.Location) {
                Program.Error("Cannot pick up item", "Item is not in the same location as the player.");
                return false;
            }

            if (Item.OnPickUp(world.Player)) {
                Item.SetLocation(world.Player);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Command for making the player pick up all items in the current location
    /// </summary>
    public class PickUpAllCommand : ICommand {
        public bool Execute(World world) {
            var list = world.Player.Location.GetItems();
            if (list.Count == 0) {
                Program.Say("There are no items to pick up here.");
                return false;
            }

            bool shouldPassTime = false;
            foreach (var item in list) {
                if (item.OnPickUp(world.Player)) {
                    shouldPassTime = true;
                    item.SetLocation(world.Player);
                }
                
            }
            return shouldPassTime;
        }
    }

    /// <summary>
    /// Command for dropping an item from the players inventory to the current location.
    /// </summary>
    public class DropCommand : ICommand {
        public IItem Item { get; private set; }

        public DropCommand(IItem item) {
            Item = item;
        }

        public static DropCommand Parse(string[] args, World world) {
            if (args.Length == 0) {
                Program.Say("You need to specify what you want to drop.");
                return null;
            }

            string cmd = String.Join(" ", args);

            IItem item = world.Player.FindItem(cmd);
            if (item != null) {
                return new DropCommand(item);
            } 
            else {
                Program.Say($"You don't have a {cmd}");
                return null;
            }
        }

        public bool Execute(World world) {
            if (Item.Location != world.Player) {
                Program.Error("Cannot drop item", "Item is not in the players inventory.");
                return false;
            }

            if (Item.OnDrop(world.Player)) {
                Item.SetLocation(world.Player.Location);
                return true;
            }
            return false;
        }
    }

    public class ConsumeCommand : ICommand {
        public IConsumable Item { get; private set; }

        public ConsumeCommand(IConsumable item) {
            Item = item;
        }

        public static ConsumeCommand Parse(string[] args, World world) {
            if (args.Length == 0) {
                Program.Say("You need to specify what you want to pick up.");
                return null;
            }

            string name = String.Join(" ", args);
            IItem item = world.Player.FindItem(name);
            if (item == null) {
                Program.Say($"You don't have any {name}.");
                return null;
            }

            if (item is IConsumable) {
                return new ConsumeCommand(item as IConsumable);
            }
            else {
                Program.Say($"You can't eat the {item.Name}.");
                return null;
            }
        }

        public bool Execute(World world) {
            if (Item.OnConsume(world.Player)) {
                return true;
            }
            return false;
        }
    }

    public class EquipCommand : ICommand {
        public IEquippable Item { get; private set; }

        public EquipCommand(IEquippable item) {
            Item = item;
        }

        public static EquipCommand Parse(string[] args, World world) {
            if (args.Length == 0) {
                Program.Say("You need to specify what you want to wear.");
                return null;
            }
            string name = String.Join(" ", args);

            IItem item = world.Player.FindItem(name);
            if (item == null) {
                Program.Say($"You don't have any {name}.");
                return null;
            }

            if (item is IEquippable) {
                return new EquipCommand(item as IEquippable);
            }
            else {
                Program.Say($"You can't equip the {item.Name}.");
               return null;
            }
        }

        public bool Execute(World world) {
            if (Item.OnEquip(world.Player)) {
                world.Player.Equip(Item);
                return true;
            }
            return false;
        }
    }

    public class UnequipCommand : ICommand {
        public IEquippable Item { get; private set; }

        public bool Execute(World world) {
            if (Item.OnUnEquip(world.Player)) {
                world.Player.UnEquip(Item);
                return true;
            }
            return false;
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
            foreach (var item in act.GetItems()) {
                Program.Say(item.Name);
            }
        }
    }

    public class LookCommand : ICommand {
        public static LookCommand Parse(string[] args, World world) {
            return new LookCommand();
        }

        public bool Execute(World world) {
            Print(world.Player.Location);
            return false;
        }

        public static void Print(MapNode location) {
            Program.Say($"You are here: {location.Name}");
            Program.Say(location.Description);

            string items = "Items: " + string.Join(", ", location.GetItems());
            Program.Say(items);

            string dirs = "Paths: " + string.Join(", ", location.GetDirections());
            Program.Say(dirs);

            string actors = "Actors: " + string.Join(", ", location.GetActors().Where(act => (!(act is PlayerActor) && act.Location == location)));
            Program.Say(actors);
        }
    }

    public class ClearCommand : ICommand {

        public static ClearCommand Parse(string[] args, World world) {
            return new ClearCommand();
        }

        public bool Execute(World world) {
            Program.Clear();
            return false;
        }
    }
}
