using System.Collections.Generic;
using TextAdv.Items;

namespace TextAdv {
    public abstract class Actor : IInventory {
        public event ActorMovedEvent ActorMoved;

        public MapNode CurrentPosition { get; private set; }

        public IList<IItem> Inventory => _inventory;

        List<IItem> _inventory;

        public Actor(MapNode position) {
            _inventory = new List<IItem>();
            CurrentPosition = position;
        }

        public abstract void Tick();

        public bool Move(Direction dir) {
            MapNode node = CurrentPosition.GetNeighbour(dir);
            if (node != null) {
                ActorMoved?.Invoke(this, new ActorMovedEventArgs(CurrentPosition, node, dir));
                CurrentPosition = node;
                return true;
            }
            return false;
        }

        public void SetLocation(MapNode node) {
            ActorMoved?.Invoke(this, new ActorMovedEventArgs(CurrentPosition, node, Direction.None));
            CurrentPosition = node;
        }
    }

    public delegate void ActorMovedEvent(object sender, ActorMovedEventArgs args);

    public class ActorMovedEventArgs : System.EventArgs {
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