﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextAdv.Items;

namespace TextAdv {
    public interface IActor : IInventory {
        /// <summary>
        /// The name of the actor
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The actor's location
        /// </summary>
        MapNode Location { get; }

        /// <summary>
        /// The equipment the actor is currently wearing
        /// </summary>
        IReadOnlyDictionary<EquipSlot, IItem> Equipment { get; }

        /// <summary>
        /// Update the actor by one tick
        /// </summary>
        void Tick();

        /// <summary>
        /// Event that gets called each time the actor moves
        /// </summary>
        event ActorMovedEvent ActorMoved;

        /// <summary>
        /// Equip an item
        /// </summary>
        /// <param name="item">The item to equip</param>
        /// <returns>Wether the item was successfully equipped or not</returns>
        bool Equip(IItem item);

        /// <summary>
        /// Unequip the item in the specified equipment slot
        /// </summary>
        /// <param name="slot">The slot to unequip</param>
        /// <returns>Wether the item was successfully unequipped or not</returns>
        bool UnEquip(EquipSlot slot);

        /// <summary>
        /// Unequip an item
        /// </summary>
        /// <param name="item">The item to unequip</param>
        /// <returns>Wether the item was successfully unequipped or not</returns>
        bool UnEquip(IItem item);

        /// <summary>
        /// Move in a direction
        /// </summary>
        /// <param name="dir">The direction to move</param>
        /// <returns>Wether we moved successfully or not</returns>
        bool Move(Direction dir);

        /// <summary>
        /// Set the actor's location
        /// </summary>
        /// <param name="node">The new location</param>
        void SetLocation(MapNode node);

        /// <summary>
        /// Erase this actor from the world
        /// </summary>
        void Erase();
    }

    public abstract class BaseActor : IActor {
        public event ActorMovedEvent ActorMoved;

        public MapNode Location { get; protected set; }

        public string Name { get; protected set; }

        public IReadOnlyCollection<IItem> Inventory => _inventory;

        public IReadOnlyDictionary<EquipSlot, IItem> Equipment => _equipment;

        List<IItem> _inventory;

        Dictionary<EquipSlot, IItem> _equipment;


        public BaseActor() {
            _inventory = new List<IItem>();
            _equipment = new Dictionary<EquipSlot, IItem>();
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

        public bool Equip(IItem item) {
            if (item == null) throw new ArgumentNullException("item");
            if (_equipment.ContainsKey(item.Slot)) {
                Program.Say($"The {_equipment[item.Slot].Name} is in the way!");
                return false;
            }
            if (item.OnEquip(this)) {
                _equipment.Add(item.Slot, item);
                _inventory.Remove(item);
                return true;
            }
            return false;
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

        public bool UnEquip(IItem item) {
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