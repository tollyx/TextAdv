using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdv.Items;

namespace TextAdv {
    public class PlayerActor : BaseActor {

        public PlayerActor(MapNode position, string name) : base(position) {
            Name = name;
        }

        public override bool Move(Direction dir) {
            if (base.Move(dir)) {
                // We successfully moved! 
                // Let's print our new location.
                LookCommand.Print(CurrentPosition);
                return true;
            }
            return false;
        }

        public override void Tick() {
            // TODO: do stuff
        }
    }
}
