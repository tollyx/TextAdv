using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv {
    namespace Items {
        /// <summary>
        /// A generic item that can be placed in inventories.
        /// </summary>
        public interface IItem {
            /// <summary>
            /// The name of the item. Should be unique for each item unless they are (or should appear) identical.
            /// </summary>
            string Name { get; }

            /// <summary>
            /// The description of the item. Is shown when the item is inspected.
            /// </summary>
            string Description { get; }

            /// <summary>
            /// The value of the item in the game's currency.
            /// </summary>
            int Value { get; }

            /// <summary>
            /// The current location of the item.
            /// </summary>
            IInventory Location { get; }

            /// <summary>
            /// Sets the location of the item.
            /// </summary>
            /// <param name="location">The new location for the item.</param>
            void SetLocation(IInventory location);

            /// <summary>
            /// Gets called when the item is used.
            /// </summary>
            /// <param name="user">The Actor which is using the item</param>
            /// <returns>Wether the use attempt was successful or not. (returns false to cancel the use and not pass time)</returns>
            bool OnUse(IActor user);

            /// <summary>
            /// Gets called when the item is picked up.
            /// </summary>
            /// <param name="picker">The Actor which is picking the item up.</param>
            /// <returns>If the pickup  attempt was successful or not. (returns false to cancel the pickup)</returns>
            bool OnPickUp(IActor picker);

            /// <summary>
            /// Drops the item.
            /// </summary>
            /// <param name="dropper">The actor which is doing the dropping.</param>
            /// <returns>Wether the drop attempt was successful or not. (returns false to cancel the drop)</returns>
            bool OnDrop(IActor dropper);

            /// <summary>
            /// Gets called when the item is thrown
            /// </summary>
            /// <param name="thrower">The Actor that is throwing the item</param>
            /// <param name="target">The Actor which is the target of the throw</param>
            /// <returns>Wether the item was successfully thrown, not necessarily hitting the target. (return false to cancel the throw)</returns>
            bool OnThrow(IActor thrower, IActor target);

            /// <summary>
            /// Gets called when the item is given to an Actor
            /// </summary>
            /// <param name="giver">The Actor who is giving the item</param>
            /// <param name="reciever">The who is recieving the item</param>
            /// <returns>Wether the give attempt was successful or not. (returns false to cancel the give)</returns>
            bool OnGive(IActor giver, IActor reciever);

        }

        /// <summary>
        /// An object with an inventory. Actors, Map nodes, Chests etc.
        /// </summary>
        public interface IInventory {
            /// <summary>
            /// Adds an item to the inventory.
            /// </summary>
            /// <param name="item">The item to add</param>
            /// <returns>Wether the item was added successfully or not.</returns>
            bool AddItem(IItem item);

            bool AddItems(params IItem[] items);

            /// <summary>
            /// Removes an item from the inventory.
            /// </summary>
            /// <param name="item">The item to remove</param>
            /// <returns>Wether the item was removed successfully or not.</returns>
            bool RemoveItem(IItem item);

            /// <summary>
            /// Gets a copy of the inventory list.
            /// </summary>
            /// <returns>A copy of the list of items in the inventory.</returns>
            IList<IItem> GetItems();
        }

        /// <summary>
        /// An item that can be consumed.
        /// </summary>
        public interface IConsumable : IItem {
            /// <summary>
            /// Gets called when the item is consumed. The item decides itself if it is removed or not after consumption.
            /// </summary>
            /// <param name="consumer">The Actor that is attempting to consume the item</param>
            /// <returns>Wether the action was successful or not. (return false to not pass time)</returns>
            bool OnConsume(IActor consumer);
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
            /// <summary>
            /// The equip slot the item occupies.
            /// </summary>
            EquipSlot Slot { get; }

            /// <summary>
            /// Gets called when the Actor attempts to equip the item.
            /// </summary>
            /// <param name="wearer">The actor which is attempting to equip the item.</param>
            /// <returns>Wether the attempt was successful or not. (return false to cancel the equip)</returns>
            bool OnEquip(IActor wearer);

            /// <summary>
            /// Gets called when the Actor attempts to unequip the item.
            /// </summary>
            /// <param name="wearer">The actor which is attempting to unequip the item.</param>
            /// <returns>Wether the attempt was successful or not. (return false to cancel the unequip)</returns>
            bool OnUnEquip(IActor wearer);
        }

        /// <summary>
        /// Something (presumably an actor) that can equip items.
        /// </summary>
        public interface IEquipper {
            /// <summary>
            /// Equip the provided item
            /// </summary>
            /// <param name="item">The item to equip.</param>
            /// <returns>Wether the item was successfully equipped or not</returns>
            bool Equip(IEquippable item);

            /// <summary>
            /// UeEquip the item that occupies the provided slot
            /// </summary>
            /// <param name="slot">The item slot to unequip.</param>
            /// <returns>Wether the item was successfully unequipped or not</returns>
            bool UnEquip(EquipSlot slot);

            /// <summary>
            /// UeEquip the provided item
            /// </summary>
            /// <param name="item">The item to unequip.</param>
            /// <returns>Wether the item was successfully unequipped or not</returns>
            bool UnEquip(IEquippable item);

            IList<IEquippable> GetEquippedItems();
        }

        /// <summary>
        /// Base class with default implementations for most item methods.
        /// </summary>
        public abstract class BaseItem : IItem {
            public abstract string Name { get; }

            public abstract string Description { get; }

            public abstract int Value { get; }

            public IInventory Location => _location;

            IInventory _location;

            public BaseItem() {
                _location = null;
            }

            public virtual bool OnDrop(IActor dropper) {
                Program.Say($"You dropped the {Name} to the ground.");
                return true;
            }

            public virtual bool OnGive(IActor giver, IActor reciever) {
                Program.Say($"You gave {reciever.Name} the {Name}.");
                return true;
            }

            public virtual bool OnPickUp(IActor picker) {
                Program.Say($"You picked up the {Name} and put it in your inventory.");
                return true;
            }

            public virtual bool OnThrow(IActor thrower, IActor target) {
                Program.Say($"You threw the {Name} at the {target.Name}!");
                return true;
            }

            public virtual bool OnUse(IActor user) {
                Program.Say($"You don't know how to use the {Name}.");
                return false;
            }

            public override string ToString() => Name;

            public void SetLocation(IInventory location) {
                if (location == null) throw new ArgumentNullException("location");
                if (location != _location) {
                    _location?.RemoveItem(this);
                    _location = location;
                    _location.AddItem(this);
                }
            }

            protected void Erase() {
                _location?.RemoveItem(this);
                _location = null;
            }
        }

        public class Stone : BaseItem {
            public override string Name => "Stone";

            public override string Description => "A grey rock-hard stone.";

            public override int Value => 0;
        }

        public class Potion : BaseItem, IConsumable {
            public override string Name => "Potion";

            public override string Description => "A flask filled with a blue mysterious liquid.";

            public override int Value => 10;

            public bool OnConsume(IActor consumer) {
                Program.Say("You drank the potion. It's bitter, but it didn't seem to have any effect.");
                Erase();
                return true;
            }
        }

        public class TopHat : BaseItem, IEquippable {
            public override string Name => "TopHat";

            public override string Description => "A black, fancy tophat.";

            public override int Value => 10;

            public EquipSlot Slot => EquipSlot.Head;

            public bool OnEquip(IActor wearer) {
                Console.WriteLine("You put on your fancy hat. You're so fancy.");
                return true;
            }

            public bool OnUnEquip(IActor wearer) {
                Console.WriteLine("You took off the top hat. You're not as fancy anymore.");
                return true;
            }
        }

        public static class Extensions {
            /// <summary>
            /// Finds an item in an inventory. If there are multiple matches, it asks the user to specify which one.
            /// </summary>
            /// <param name="inv">The inventory to search</param>
            /// <param name="name">The name of the item. Doesn't have to be exact, but it is expected to be all-lowercase</param>
            /// <returns>An item or null if none was found.</returns>
            public static IItem FindItem(this IInventory inv, string name) {
                if (inv == null) throw new ArgumentNullException("inv");
                var items = inv.FindItems(name).ToList();
                if (items.Count > 0) {
                    if (items.Count == 1) {
                        return items.First();
                    }
                    else {
                        // Don't bother asking the user if all the items are identical
                        if (items.Skip(1).All(i => i.Name == items[0].Name)) {
                            return items.First();
                        }

                        return PromptSpecify(items);
                    }
                }
                return null;
            }

            static T PromptSpecify<T>(IList<T> list) {
                string str = "Please specify: ";
                for (int i = 0; i < list.Count; i++) {
                    str += $"{i + 1}: {list[i]}";
                    if (i < list.Count - 1) {
                        str += ", ";
                    }
                }
                int sel = Program.AskInt(str) - 1;
                if (sel >= 0 && sel < list.Count) {
                    return list[sel];
                }
                else {
                    return default(T);
                }
            }

            /// <summary>
            /// Finds all matching items in an inventory.
            /// </summary>
            /// <param name="inv">The inventory to search</param>
            /// <param name="name">The name of the wanted item.</param>
            /// <returns>A list of items or null if none was found.</returns>
            public static IEnumerable<IItem> FindItems(this IInventory inv, string name) {
                if (inv == null) throw new ArgumentNullException("inv");
                return inv.GetItems().Where((item) => item.Name.ToLower().Contains(name));
            }
        }
    }
}
