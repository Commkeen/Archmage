using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;

namespace Archmage.Engine.Items
{
    class ItemToken:GameObject
    {
        public IntVector2 Position { get; set; }
        public int ItemID { get; protected set; }
        public bool PositionKnown { get; set; }

        public ItemToken(int instanceID, int itemID)
            : base("itemToken", instanceID)
        {
            Position = new IntVector2();
            ItemID = itemID;
            PositionKnown = false;
        }
    }
}
