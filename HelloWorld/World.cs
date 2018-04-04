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
        public MapNode CurrentNode { get; private set; }

        public World()
        {
            nodes = new List<MapNode>();
            nodes.Add(new MapNode("Castle Gates", "A large castle is in front of you. Its gates are open."));
            nodes.Add(new MapNode("Castle Entrance", ""));
            nodes.Add(new MapNode("Castle West Hall", ""));
            nodes.Add(new MapNode("Castle East Hall", ""));
            nodes[0].SetNeighbour(Direction.In, nodes[1], true);
            nodes[1].SetNeighbour(Direction.West, nodes[2], true);
            nodes[1].SetNeighbour(Direction.East, nodes[3], true);
            CurrentNode = nodes[0];
        }

        public bool Move(Direction dir)
        {
            MapNode node = CurrentNode.GetNeighbour(dir);
            if (node != null)
            {
                CurrentNode = node;
                return true;
            }
            return false;
        }
    }
}
