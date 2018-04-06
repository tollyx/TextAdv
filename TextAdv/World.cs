using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv {
    public class World {
        public List<MapNode> Nodes { get; private set; }
        public PlayerActor Player { get; private set; }
        public List<BaseActor> Actors { get; private set; }

        public World(string playername) {
            Nodes = new List<MapNode> {
                new MapNode("Castle Gates", "A large castle is in front of you. Its gates are open."),
                new MapNode("Castle Entrance", "Just imagine a grandiouse castle entrance from some fantasy RPG"),
                new MapNode("Castle West Hall", "A hall like many others, filled with unimportant doors."),
                new MapNode("Castle East Hall", "A hall like many others, filled with unimportant doors.")
            };
            Nodes[0].SetNeighbour(Direction.In, Nodes[1], true);
            Nodes[0].Inventory.Add(new Items.Stone(Nodes[0]));
            Nodes[0].Inventory.Add(new Items.Stone(Nodes[0]));
            Nodes[0].Inventory.Add(new Items.Stone(Nodes[0]));
            Nodes[1].SetNeighbour(Direction.West, Nodes[2], true);
            Nodes[1].SetNeighbour(Direction.East, Nodes[3], true);
            Player = new PlayerActor(Nodes[0], playername);
            
            Actors = new List<BaseActor> {
                Player
            };
        }

        public void Tick() {
            foreach (var item in Actors) {
                item.Tick();
            }
        }
    }
}
