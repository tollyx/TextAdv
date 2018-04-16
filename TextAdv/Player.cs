using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAdv.Items;

namespace TextAdv {
    public class PlayerActor : BaseActor {

        public PlayerActor(string name) : base() {
            Name = name;
        }

        public override bool Move(Direction dir) {
            if (base.Move(dir)) {
                // We successfully moved! 
                // Let's print our new location. 
                LookCommand.Print(Location);
                return true;
            }
            return false;
        }

        public override void Tick() {
            // TODO: do stuff
        }
    }
}
