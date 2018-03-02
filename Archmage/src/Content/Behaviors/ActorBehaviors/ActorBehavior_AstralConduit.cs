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
    class ActorBehavior_AstralConduit:ActorBehavior
    {

        public ActorBehavior_AstralConduit(int instanceID, int affectedActor)
            : base("b_astralConduit", instanceID, affectedActor)
        {
        }

        public override TurnResult OnActorStartTurn()
        {
            return base.OnActorStartTurn();
        }

        public override DamageData OnSendDamage(DamageData damage)
        {
            if (damage.DmgAttackType == AttackType.ARCANE)
            {
                damage.Magnitude++;
            }

            return damage;
        }
    }
}
