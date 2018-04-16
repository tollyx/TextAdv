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
        bool Execute(PlayerActor player);
    }

    /// <summary>
    /// Static class for general command methods and utilities.
    /// </summary>
    public static class Command {

        public delegate ICommand CommandDelegate(string[] args, World world);

        // Iterating over this abomination is apparently faster than a dict lookup?!
        // Yes, I benchmarked it myself, but it was probably an unreliable result.
        // TODO: Move the arrays to static members of the classes
        public static readonly (string[] cmds, CommandDelegate callback)[] commands = {
            (new[]{ "move", "mv", "go", "walk", "run" }, MoveCommand.Parse),
            (new[]{ "pick", "take", "ta", "get", "grab" }, PickUpCommand.Parse),
            (new[]{ "drop", "dr" },  DropCommand.Parse),
            (new[]{ "inv", "i", "inventory", "bag" }, InventoryCommand.Parse),
            (new[]{ "eat", "dri", "drink", "consume" }, ConsumeCommand.Parse),
            (new[]{ "wear", "we", "eq", "equip" }, EquipCommand.Parse),
            (new[]{ "re", "remove", "uneq", "unequip" }, UnequipCommand.Parse),
            //(new[]{ "op", "open" }, OpenCommand.Parse),
            //(new[]{ "cl", "close" }, CloseCommand.Parse),
            (new[]{ "look", "l", "here", "ls" }, LookCommand.Parse),
            (new[]{ "clear" }, ClearCommand.Parse),
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

            // Quick-syntax for moving around, because that's what you constantly do.
            ICommand move = MoveCommand.Parse(args, world);
            if (move != null) {
                return move;
            }

            string cmd = args[0];
            args = args.Skip(1).ToArray();
            
            

            foreach ((var cmds, var callback) in commands) {
                if (cmds.Contains(cmd)) {
                    return callback(args, world);
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

        public static string Syntax => "move <direction>";

        public static string Description => "Moves the player, if possible, in the given direction.";

        static readonly (string[] cmds, Direction dir)[] DirectionStrings = {
            (new[]{ "n", "north" }, Direction.North),
            (new[]{ "s", "south" }, Direction.South),
            (new[]{ "w", "west" }, Direction.West),
            (new[]{ "e", "east" }, Direction.East),
            (new[]{ "nw", "northwest" }, Direction.NorthWest),
            (new[]{ "ne", "northeast" }, Direction.NorthEast),
            (new[]{ "sw", "southwest" }, Direction.SouthWest),
            (new[]{ "se", "southeast" }, Direction.SouthEast),
            (new[]{ "u", "up" }, Direction.Up),
            (new[]{ "d", "down" }, Direction.Down),
            (new[]{ "in" }, Direction.In),
            (new[]{ "out" }, Direction.Out),
        };

        public static ICommand Parse(string[] args, World world) {
            if (args.Count() > 0) {
                foreach ((var cmds, var dir) in DirectionStrings) {
                    if (cmds.Contains(args[0])) {
                        return new MoveCommand(dir);
                    }
                }
            }
            return null;
        }

        public MoveCommand(Direction dir) {
            Where = dir;
        }

        public bool Execute(PlayerActor player) {
            if (player.Move(Where)) {
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

        public static string Syntax => "take <item>";

        public static string Description => "Picks up an item and puts it in the players inventory.";

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

        public bool Execute(PlayerActor player) {
            if (Item.Location != player.Location) {
                Program.Error("Cannot pick up item", "Item is not in the same location as the player.");
                return false;
            }

            if (Item.OnPickUp(player)) {
                Item.SetLocation(player);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Command for making the player pick up all items in the current location
    /// </summary>
    public class PickUpAllCommand : ICommand {
        public static string Syntax => "take all";

        public static string Description => "Takes all items in the room and puts them in the players inventory";

        public bool Execute(PlayerActor player) {
            var list = player.Location.Inventory;
            if (list.Count == 0) {
                Program.Say("There are no items to pick up here.");
                return false;
            }

            bool shouldPassTime = false;
            foreach (var item in list) {
                if (item.OnPickUp(player)) {
                    shouldPassTime = true;
                    item.SetLocation(player);
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

        public static string Syntax => "drop <item>";

        public static string Description => "Drops an item from the players inventory.";

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

        public bool Execute(PlayerActor player) {
            if (Item.Location != player) {
                Program.Error("Cannot drop item", "Item is not in the players inventory.");
                return false;
            }

            if (Item.OnDrop(player)) {
                Item.SetLocation(player.Location);
                return true;
            }
            return false;
        }
    }

    public class ConsumeCommand : ICommand {
        public IItem Item { get; private set; }

        public static string Syntax => "consume <item>";

        public static string Description => "Consumes an item that is in the players inventory";

        public ConsumeCommand(IItem item) {
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

            return new ConsumeCommand(item);
        }

        public bool Execute(PlayerActor player) {
            if (Item.OnConsume(player)) {
                return true;
            }
            return false;
        }
    }

    public class EquipCommand : ICommand {
        public IItem Item { get; private set; }

        public static string Syntax => "wear <item>";

        public static string Description => "Wears an item from the players inventory.";

        public EquipCommand(IItem item) {
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

            return new EquipCommand(item);
        }

        public bool Execute(PlayerActor player) {
            return player.Equip(Item);
        }
    }

    public class UnequipCommand : ICommand {
        public IItem Item { get; private set; }

        public static string Syntax => "remove <item>";

        public static string Description => "Unequips an item and puts it in the players inventory";

        public UnequipCommand(IItem item) {
            Item = item;
        }

        public static ICommand Parse(string[] args, World world) {
            var name = string.Join(" ", args).ToLower();
            foreach (var item in world.Player.Equipment) {
                if (item.Value.Name.ToLower().Contains(name)) {
                    return new UnequipCommand(item.Value);
                }
            }
            return null;
        }

        public bool Execute(PlayerActor player) {
            return player.UnEquip(Item);
        }
    }

    public class InventoryCommand : ICommand {
        public static string Syntax => "inventory";

        public static string Description => "Lists all items in the players inventory";

        public static InventoryCommand Parse(string[] args, World world) {
            return new InventoryCommand();
        }

        public bool Execute(PlayerActor player) {
            Print(player);
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
        public static string Syntax => "look";

        public static string Description => "Shows items, actors in the room and the rooms description.";

        public static LookCommand Parse(string[] args, World world) {
            return new LookCommand();
        }

        public bool Execute(PlayerActor player) {
            Print(player.Location);
            return false;
        }

        public static void Print(MapNode location) {
            Program.Say($"You are here: {location.Name}");
            Program.Say(location.Description);

            string items = "Items: " + string.Join(", ", location.Inventory
                .GroupBy(x => x.Name)
                .Select(x => x.Count() > 1 ? $"{x.Count()}x {x.Key}" : x.Key));
            Program.Say(items);

            string dirs = "Paths: " + string.Join(", ", location.GetDirections());
            Program.Say(dirs);

            string actors = "Actors: " + string.Join(", ", location.GetActors().Where(act => (!(act is PlayerActor))));
            Program.Say(actors);
        }
    }

    public class ClearCommand : ICommand {
        public static string Syntax => "clear";

        public static string Description => "Clears the terminal";

        public static ClearCommand Parse(string[] args, World world) {
            return new ClearCommand();
        }

        public bool Execute(PlayerActor player) {
            Program.Clear();
            return false;
        }
    }

    public class HelpCommand : ICommand {
        public static string Syntax => "help [command]";

        public static string Description => "Lists all commands and more information of a provided command";



        public bool Execute(PlayerActor player) {
            return false;
        }

        static void Help(string command) {
            if (!string.IsNullOrWhiteSpace(command)) {
                foreach (var (cmds, callback) in Command.commands) {

                }
            }
        }
    }
}
