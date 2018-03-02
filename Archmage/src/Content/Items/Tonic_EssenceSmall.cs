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
    class Tonic_EssenceSmall : Item
    {


        public Tonic_EssenceSmall(int instanceID)
            : base("tonic_essenceSmall", instanceID)
        {
            RealName = "essence tonic";
            Sprite = '!';
        }

        public override bool OnUse(Actor a)
        {
            if (a is Student)
            {
                ((Student)a).CurrentEssence += 5;
                _scene.WriteMessage("You drink the potion.");
                _scene.WriteMessage("You feel a surge of magical power!");
                Identify(ObjectID);
            }
            _scene.GetStudent().RemoveItem(this);

            return true;
        }

    }
}