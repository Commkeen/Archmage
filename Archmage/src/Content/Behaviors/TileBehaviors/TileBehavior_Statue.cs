using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Behaviors;
using Archmage.Engine.DataStructures;

namespace Archmage.Content.Behaviors.TileBehaviors
{
    class TileBehavior_Statue:TileBehavior
    {
        public int Durability { get; set; }

        public TileBehavior_Statue(int instanceID, IntVector2 affectedTile)
            : base("statue", instanceID, affectedTile)
        {
            Durability = 10;
        }

        public override DamageData OnIncomingDamage(DamageData damage)
        {
            if (damage.DamagesWalls)
            {
                Durability -= 5;
                if (Durability <= 0)
                    _scene.GetMap().RemoveTileBehavior(InstanceID);
            }

            return damage;
        }
    }
}
