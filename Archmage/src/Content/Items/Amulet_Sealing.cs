using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Items;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;

namespace Archmage.Content.Items
{
    class Amulet_Sealing : Item
    {


        public Amulet_Sealing(int instanceID)
            : base("amuletSealing", instanceID)
        {
            RealName = "Amulet of Sealing";
            Sprite = '&';
            PermanentColor = new Color(libtcod.TCODColor.yellow);
        }

        public override bool OnUse(Actor a)
        {
            /*
            if (_scene.GetMap().GetTileName(a.Position) == "fireRift")
            {
                _scene.WinGame();
            }
            else
            {
                _scene.WriteMessage("You can only use the amulet on the Rift of Fire!");
            }
             * */

            return false;
        }

    }
}
