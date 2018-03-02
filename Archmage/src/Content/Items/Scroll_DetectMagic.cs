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
    class Scroll_DetectMagic:Item
    {


        public Scroll_DetectMagic(int instanceID)
            : base("scroll_detectMagic", instanceID)
        {
            Sprite = '?';
            PermanentColor = new Color(libtcod.TCODColor.blue);
        }

        public override bool OnUse(Actor a)
        {
            int crystalCounter = 0;
            foreach (ItemToken t in _scene.GetGameObjectPool().GetAllAliveItemTokens())
            {
                Item i = _scene.GetGameObjectPool().GetItem(t.ItemID);
                if (i.ObjectID == "magicCrystal")
                {
                    t.PositionKnown = true;
                    crystalCounter++;
                }
            }

            if (crystalCounter > 0)
            {
                _scene.WriteMessage("You sense the presence of magical energy.");
            }
            else
            {
                _scene.WriteMessage("Your mind reaches out into the area, but it doesn't seem to find what it's looking for.");
            }

            return true;
        }

    }
}
