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
    class Potion_Health : Item
    {


        public Potion_Health(int instanceID)
            : base("tonic_healthSmall", instanceID)
        {
            RealName = "health tonic";
            Sprite = '!';
        }

        public override bool OnUse(Actor a)
        {
            a.HealDamage(6);
            _scene.WriteMessage("You drink the potion.");
            _scene.WriteMessage("You feel your wounds heal!");
            Identify(ObjectID);
            _scene.GetStudent().RemoveItem(this);

            return true;
        }

    }
}