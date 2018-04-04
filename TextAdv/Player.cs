using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAdv
{
    public class PlayerActor : Actor
    {
        public string Name { get; private set; }

        public PlayerActor(MapNode position, string name) : base(position)
        {
            Name = name;
        }
    }
}
