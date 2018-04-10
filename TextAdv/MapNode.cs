using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdv.Items;

namespace TextAdv {
    public class MapNode : IInventory {
        public string Name { private set; get; }
        public string Description { private set; get; }

        Dictionary<Direction, MapNode> neighbours = new Dictionary<Direction, MapNode>();
        List<IItem> _inventory;
        private List<IActor> _actors;

        public MapNode(string name, string description) {
            _inventory = new List<IItem>();
            _actors = new List<IActor>();
            Name = name;
            Description = description;
        }

        public MapNode SetNeighbour(Direction direction, MapNode node, bool bothWays = false) {
            neighbours[direction] = node;
            if (bothWays) {
                node.SetNeighbour(direction.Opposite(), this, false);
            }
            return this;
        }

        public MapNode RemoveNeighbour(Direction direction, bool bothWays = false) {
            if (neighbours.ContainsKey(direction)) {
                MapNode node = neighbours[direction];
                neighbours.Remove(direction);
                if (bothWays) {
                    node.RemoveNeighbour(direction.Opposite(), false);
                }
            }
            return this;
        }

        public MapNode GetNeighbour(Direction dir) {
            if (neighbours.ContainsKey(dir)) {
                return neighbours[dir];
            }
            return null;
        }

        public IList<Direction> GetDirections() {
            return neighbours.Keys.ToList();
        }

        public IList<IActor> GetActors() {
            return _actors.ToList();
        }

        public bool AddActor(IActor actor) {
            _actors.Add(actor);
            return true;
        }

        public bool RemoveActor(IActor actor) {
            return _actors.Remove(actor);
        }

        public IList<IItem> GetItems() {
            return _inventory.ToList();
        }

        public bool AddItem(IItem item) {
            if (!_inventory.Contains(item)) {
                _inventory.Add(item);
                item.SetLocation(this);
                return true;
            }
            return false;
        }

        public bool RemoveItem(IItem item) {
            return _inventory.Remove(item);
        }
    }
}
