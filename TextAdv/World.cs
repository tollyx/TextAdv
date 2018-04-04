using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    public class World
    {
        List<MapNode> nodes;
        public PlayerActor Player { get; private set; }
        public List<Actor> Actors { get; private set; }

        public World(string playername)
        {
            nodes = new List<MapNode>
            {
                new MapNode("Castle Gates", "A large castle is in front of you. Its gates are open."),
                new MapNode("Castle Entrance", ""),
                new MapNode("Castle West Hall", ""),
                new MapNode("Castle East Hall", "")
            };
            nodes[0].SetNeighbour(Direction.In, nodes[1], true);
            nodes[1].SetNeighbour(Direction.West, nodes[2], true);
            nodes[1].SetNeighbour(Direction.East, nodes[3], true);
            Player = new PlayerActor(nodes[0], playername);
            Actors = new List<Actor>
            {
                Player
            };
        }

        public void Tick()
        {
            foreach (var item in Actors)
            {
                item.Tick();
            }
        }
    }
}
