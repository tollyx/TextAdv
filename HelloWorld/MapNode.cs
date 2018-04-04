using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    public class MapNode
    {
        public string Name { private set; get; }
        public string Description { private set; get; }
        Dictionary<Direction, MapNode> neighbours = new Dictionary<Direction, MapNode>();

        public MapNode(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public MapNode SetNeighbour(Direction direction, MapNode node, bool bothWays = false)
        {
            neighbours[direction] = node;
            if (bothWays)
            {
                node.SetNeighbour(direction.Opposite(), this, false);
            }
            return this;
        }

        public MapNode RemoveNeighbour(Direction direction, bool bothWays = false)
        {
            if (neighbours.ContainsKey(direction))
            {
                MapNode node = neighbours[direction];
                neighbours.Remove(direction);
                if (bothWays)
                {
                    node.RemoveNeighbour(direction.Opposite(), false);
                }
            }
            return this;
        }

        public MapNode GetNeighbour(Direction dir)
        {
            if (neighbours.ContainsKey(dir))
            {
                return neighbours[dir];
            }
            return null;
        }

        public IList<Direction> GetDirections()
        {
            return neighbours.Keys.ToList();
        }
    }
}
