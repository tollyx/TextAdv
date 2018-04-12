using System;
using System.Collections.Generic;
using System.Linq;
using TextAdv.Items;

namespace TextAdv {
    public interface IActor : IInventory {
        string Name { get; }
        MapNode Location { get; }
        void Tick();
        event ActorMovedEvent ActorMoved;
        bool Move(Direction dir);
        void SetLocation(MapNode node);
        void Erase();
    }

    public abstract class BaseActor : IActor, IEquipper {
        public event ActorMovedEvent ActorMoved;

        public MapNode Location { get; protected set; }

        public string Name { get; protected set; }

        List<IItem> _inventory;

        Dictionary<EquipSlot, IEquippable> _equipment;

        public IDictionary<EquipSlot, IEquippable> Equipment => _equipment;

        public BaseActor() {
            _inventory = new List<IItem>();
            _equipment = new Dictionary<EquipSlot, IEquippable>();
            Location = null;
        }

        public abstract void Tick();

        public virtual bool Move(Direction dir) {
            MapNode node = Location.GetNeighbour(dir);
            if (node != null) {
                Location.RemoveActor(this);
                node.AddActor(this);
                Location = node;
                ActorMoved?.Invoke(this, new ActorMovedEventArgs(Location, node, dir));
                return true;
            }
            Program.Error("Cannot move player", "The player location is null");
            return false;
        }

        public void SetLocation(MapNode node) {
            if (node == null) throw new ArgumentNullException("node");
            if (node != Location) {
                Location?.RemoveActor(this);
                node.AddActor(this);
                Location = node;
                ActorMoved?.Invoke(this, new ActorMovedEventArgs(Location, node, Direction.None));
            }
        }

        public override string ToString() => Name;

        public bool Equip(IEquippable item) {
            if (item == null) throw new ArgumentNullException("item");
            if (_equipment.ContainsKey(item.Slot) || !item.OnEquip(this)) {
                return false;
            }
            else {
                Program.Say($"You equipped the {item.Name}");
                _equipment.Add(item.Slot, item);
                _inventory.Remove(item);
                return true;
            }
        }

        public bool UnEquip(EquipSlot slot) {
            if (_equipment.ContainsKey(slot) && _equipment[slot].OnUnEquip(this)) {
                var item = _equipment[slot];
                _equipment.Remove(slot);
                _inventory.Add(item);
                return true;
            }
            return false;
        }

        public bool UnEquip(IEquippable item) {
            if (_equipment.ContainsValue(item) && item.OnUnEquip(this)) {
                _equipment.Remove(item.Slot);
                _inventory.Add(item);
                return true;
            }
            return false;
        }

        public bool AddItem(IItem item) {
            if (item == null) throw new ArgumentNullException("item");
            if (!_inventory.Contains(item)) {
                _inventory.Add(item);
                item.SetLocation(this);
            }
            return true;
        }

        public bool RemoveItem(IItem item) {
            return _inventory.Remove(item);
        }

        public IList<IItem> GetItems() {
            return _inventory.ToList();
        }

        public IList<IEquippable> GetEquippedItems() {
            return _equipment.Values.ToList();
        }

        public bool AddItems(params IItem[] items) {
            bool success = false;
            foreach (var item in items) {
                success = AddItem(item) || success;
            }
            return success;
        }

        public void Erase() {
            Location?.RemoveActor(this);
            Location = null;
        }
    }

    public delegate void ActorMovedEvent(object sender, ActorMovedEventArgs args);

    public class ActorMovedEventArgs : EventArgs {
        public ActorMovedEventArgs(MapNode from, MapNode to, Direction dir) {
            From = from;
            To = to;
            Direction = dir;
        }
        public MapNode From { get; set; }
        public MapNode To { get; set; }
        public Direction Direction { get; private set; }
    }
}