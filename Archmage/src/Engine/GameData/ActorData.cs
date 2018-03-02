using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.GameData
{
    public class ActorData
    {
        public int x;
        public int y;

        public string actorType;

        public ActorData(int x, int y, string actorType)
        {
            this.x = x;
            this.y = y;
            this.actorType = actorType;
        }
    }
}
