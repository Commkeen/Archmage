using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Behaviors;
using Archmage.Actors;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Engine.Spells;

namespace Archmage.Content.Behaviors.ActorBehaviors
{
    class ActorBehavior_ScaledSkin:ActorBehavior
    {

        public ActorBehavior_ScaledSkin(int instanceID, int affectedActor)
            : base("b_scaledSkin", instanceID, affectedActor)
        {
            ArmorBonus = 2;
        }

        public override TurnResult OnActorStartTurn()
        {
            return base.OnActorStartTurn();
        }

        public override void OnArmorAbsorbsDamage()
        {
            _scene.WriteMessage("Your scales absorb some damage.");
        }
    }
}
