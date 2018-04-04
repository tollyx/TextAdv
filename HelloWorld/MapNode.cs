using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    class MapNode
    {
        public string Name { private set { Name = value; } get { return Name; } }
        public string Description { private set { Description = value; } get { return Description; } }
        Dictionary<Direction, MapNode> neighbours = new Dictionary<Direction, MapNode>();

        public MapNode(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public void SetNeighbour(Direction direction, MapNode node, bool bothWays = false)
        {
            neighbours[direction] = node;
            if (bothWays)
            {
                node.SetNeighbour(direction.Opposite(), this, false);
            }
        }
    }
}
