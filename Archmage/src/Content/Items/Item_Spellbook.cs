using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archmage.Engine.Items;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Spells;

namespace Archmage.Content.Items
{
    class Item_Spellbook : Item
    {
        public string spellID;


        public Item_Spellbook(int instanceID)
            : base("itemSpellbook", instanceID)
        {
            spellID = "magicDart";
            RealName = "Spellbook";
            Sprite = '+';
            PermanentColor = new Color(libtcod.TCODColor.yellow);
        }

        public void InitSpellbook(string spellID)
        {
            this.spellID = spellID;
            RealName = "Spellbook of " + _scene.CreateSpell(spellID, _scene.GetStudent().InstanceID).Name;
        }

        public override bool OnUse(Actor a)
        {
            if (a is Student)
            {
                ((Student)a).OnReadSpellbook(this);
            }
            return false;
        }
    }
}
