using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv {
    public class World {
        public static World Singleton { get; private set; }

        public Random Rng { get; private set; }

        public PlayerActor Player { get; private set; }

        List<MapNode> _nodes;
        public IReadOnlyList<MapNode> Nodes { get => _nodes; }

        List<IActor> _actors;
        public IReadOnlyList<IActor> Actors { get => _actors; }

        public World(string playername, int? seed = null) {
            if (Singleton == null) {
                Singleton = this;
            }
            Rng = new Random(seed.GetValueOrDefault(Environment.TickCount));

            _nodes = new List<MapNode> {
                new MapNode("Castle Gates", "A large castle is in front of you. Its gates are open."),
                new MapNode("Castle Entrance", "Just imagine a grandiouse castle entrance from some fantasy RPG"),
                new MapNode("Castle West Hall", "A hall like many others, filled with unimportant doors."),
                new MapNode("Castle East Hall", "A hall like many others, filled with unimportant doors."),
                new MapNode("Bedroom", "A pretty large bedroom, this sure is a castle huh?"),
            };
            Nodes[0].SetNeighbour(Direction.In, Nodes[1], true);
            Nodes[0].AddItem(new Items.Stone());
            Nodes[0].AddItem(new Items.Stone());
            Nodes[1].SetNeighbour(Direction.West, Nodes[2], true);
            Nodes[1].SetNeighbour(Direction.East, Nodes[3], true);
            for (int i = 0; i < 5; i++) {
                Nodes[2].AddItem(new Items.Potion());
            }
            Nodes[1].AddItem(new Items.TopHat());
            Nodes[3].SetNeighbour(Direction.North, Nodes[4], true);
            Player = new PlayerActor(playername);
            _actors = new List<IActor>();

            AddActor(Player, Nodes[0]);
        }

        public void AddActor(IActor actor, MapNode location) {
            if (!_actors.Contains(actor)) {
                _actors.Add(actor);
            }
            actor.SetLocation(location);
        }

        public bool RemoveActor(IActor actor) {
            if (_actors.Remove(actor)) {
                actor.Erase();
                return true;
            }
            return false;
        }

        public void Tick() {
            foreach (var act in _actors) {
                act.Tick();
            }
        }

        public void Cleanup() {
            if (Singleton == this) {
                Singleton = null;
            }

        }
    }
}
