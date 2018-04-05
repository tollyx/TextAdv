﻿using System;
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
            bool Use(Actor user);
            bool PickUp(Actor picker);
            bool Drop(Actor dropper);
            bool Throw(Actor thrower, Actor target);
            bool Give(Actor giver, Actor reciever);

        }

        public interface IInventory {
            IList<IItem> Inventory { get; }
        }

        public interface IConsumable : IItem {
            bool Consume(Actor consumer);
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

        public interface IEquipment : IItem {
            EquipSlot Slot { get; }
            bool Equip(Actor wearer);
            bool Unequip(Actor wearer);
        }

        public class Stone : IItem {
            public string Name => "Stone";

            public string Description => "A grey, rock-hard stone.";

            public int Value => 0;

            public IInventory Location => _location;

            IInventory _location;

            public Stone(IInventory location) {
                _location = location;
            }

            public bool Drop(Actor dropper) {
                dropper.Inventory.Remove(this);
                dropper.CurrentPosition.Inventory.Add(this);
                _location = dropper.CurrentPosition;
                return true;
            }

            public bool Give(Actor giver, Actor reciever) {
                giver.Inventory.Remove(this);
                reciever.Inventory.Add(this);
                _location = reciever;
                return true;
            }

            public bool PickUp(Actor picker) {
                picker.CurrentPosition.Inventory.Remove(this);
                picker.Inventory.Add(this);
                return true;
            }

            public bool Throw(Actor thrower, Actor target) {
                throw new NotImplementedException();
            }

            public bool Use(Actor user) {
                throw new NotImplementedException();
            }
        }
    }
}
