using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv {
    namespace Items {
        public interface IItem {
            string Name { get; }
            string Description { get; }
            int Value { get; }
            IInventory Location { get; }
            bool Use(IActor user);
            bool PickUp(IActor picker);
            /// <summary>
            /// Drops the item.
            /// </summary>
            /// <param name="dropper">The actor which is doing the dropping.</param>
            /// <returns>Wether the attempt was successful or not.</returns>
            bool Drop(IActor dropper);
            bool Throw(IActor thrower, IActor target);
            bool Give(IActor giver, IActor reciever);

        }

        /// <summary>
        /// An object with an inventory. Actors, Map nodes, Chests etc.
        /// </summary>
        public interface IInventory {
            IList<IItem> Inventory { get; }
        }

        /// <summary>
        /// An item that can be consumed.
        /// </summary>
        public interface IConsumable : IItem {
            bool Consume(BaseActor consumer);
        }

        public enum EquipSlot {
            Head,
            Neck,
            Torso,
            Hands,
            Legs,
            Feet,
            Weapon,
            Finger,
            Wrists,
            Ankles,
        }

        /// <summary>
        /// An item that can equipped
        /// </summary>
        public interface IEquippable : IItem {
            EquipSlot Slot { get; }
            bool Equip(BaseActor wearer);
            bool Unequip(BaseActor wearer);
        }

        public abstract class BaseItem : IItem {
            public abstract string Name { get; }

            public abstract string Description { get; }

            public abstract int Value { get; }

            public IInventory Location => _location;

            IInventory _location;

            public BaseItem(IInventory location) {
                _location = location;
            }

            public virtual bool Drop(IActor dropper) {
                if (_location == dropper) {
                    Console.WriteLine($"You dropped the {Name} to the ground.");
                    dropper.Inventory.Remove(this);
                    dropper.CurrentPosition.Inventory.Add(this);
                    _location = dropper.CurrentPosition;
                    return true;
                }
                Console.WriteLine($"You can't drop a {Name} that you don't have.");
                return false;
            }

            public virtual bool Give(IActor giver, IActor reciever) {
                if (giver == _location) {
                    Console.WriteLine($"You gave {reciever.Name} the {Name}.");
                    giver.Inventory.Remove(this);
                    reciever.Inventory.Add(this);
                    _location = reciever;
                    return true;
                }
                Console.WriteLine($"You can't give a {Name} that you don't have.");
                return false;
            }

            public virtual bool PickUp(IActor picker) {
                if (picker.CurrentPosition == _location) {
                    Console.WriteLine($"You picked up the {Name} and put it in your inventory.");
                    picker.CurrentPosition.Inventory.Remove(this);
                    picker.Inventory.Add(this);
                    _location = picker;
                    return true;
                }
                Console.WriteLine($"You can't pick up a {Name} that you don't have.");
                return false;
            }

            public virtual bool Throw(IActor thrower, IActor target) {
                throw new NotImplementedException();
            }

            public virtual bool Use(IActor user) {
                throw new NotImplementedException();
            }

            public override string ToString() {
                return Name;
            }
        }

        public class Stone : BaseItem {
            public Stone(IInventory location) : base(location) {
            }

            public override string Name => "Stone";

            public override string Description => "A grey rock-hard stone.";

            public override int Value => 0;
        }

        public static class Extensions {
            /// <summary>
            /// Finds an item in an inventory. If there are multiple matches, it asks the user to specify which one.
            /// </summary>
            /// <param name="inv">The inventory to search</param>
            /// <param name="name">The name of the item. Doesn't have to be exact, but it is expected to be all-lowercase</param>
            /// <returns>An item or null if none was found.</returns>
            public static IItem FindItem(this IInventory inv, string name) {
                var items = inv.FindItems(name);
                if (items.Count > 0) {
                    if (items.Count == 1) {
                        return items.First();
                    }
                    else {
                        // Don't bother asking the user if all the items are identical
                        if (items.All(i => i.Name == items[0].Name)) {
                            return items.First();
                        }

                        string str = "Please specify: ";
                        for (int i = 0; i < items.Count; i++) {
                            str += $"{i + 1}.{items[i]}";
                            if (i < items.Count-1) {
                                str += ", ";
                            }
                        }
                        int sel = Program.AskInt(str)-1;
                        if (sel >= 0 && sel < items.Count) {
                            return items[sel];
                        }
                        else {
                            return null;
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// Finds all matching items in an inventory.
            /// </summary>
            /// <param name="inv">The inventory to search</param>
            /// <param name="name">The name of the wanted item.</param>
            /// <returns>A list of items or null if none was found.</returns>
            public static IList<IItem> FindItems(this IInventory inv, string name) {
                try {
                    return inv.Inventory.Where((item) => item.Name.ToLower().Contains(name)).ToList();
                }
                catch (InvalidOperationException) {
                    return null;
                }
            }
        }
    }
}
