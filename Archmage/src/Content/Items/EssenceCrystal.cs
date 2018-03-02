using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Engine.Items;
using Archmage.Engine.Scenes;
using Archmage.Engine.DataStructures;

namespace Archmage.Content.Items
{
    class EssenceCrystal:Item
    {

        public int Essence { get; set; }

        public EssenceCrystal(int essence, int instanceID)
            : base("essenceCrystal", instanceID)
        {
            Essence = essence;
            Sprite = '$';
            RealName = "Essence Crystal";
            PermanentColor = new Color(libtcod.TCODColor.blue);
        }

        public override bool OnUse(Actor a)
        {
            //Cast actor to student since only students use magic crystals
            //TODO: Maybe make this better somehow...
            Student s = (Student)a;

            s.CurrentEssence+= Essence;

            _scene.WriteMessage("You draw magical essence from the crystal.");
            s.RemoveItem(this);

            return false;
        }
    }
}
