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
    class ActorBehavior_DamageOverTime:ActorBehavior
    {
        public int TurnsRemaining { get; set; }
        public DamageData Damage;

        public ActorBehavior_DamageOverTime(int instanceID, int affectedActor)
            : base("b_damageOverTime", instanceID, affectedActor)
        {
            TurnsRemaining = 5; //Just a default

            Damage = new DamageData();
            Damage.DmgAttackType = AttackType.DARK;
            Damage.SourceID = affectedActor;
            Damage.TargetID = affectedActor;
            Damage.Magnitude = 1;
        }

        public override TurnResult OnActorStartTurn()
        {
            TurnsRemaining--;

            Actor tgt = _scene.GetGameObjectPool().GetActor(AffectedActor);
            DamageData dmg = Damage;
            tgt.TakeDamage(dmg);

            if (TurnsRemaining <= 0)
            {
                if (ParentAbility != null)
                    ParentAbility.Deactivate();
                else
                {
                    _scene.GetGameObjectPool().GetActor(AffectedActor).RemoveBehavior(this.InstanceID);
                    Alive = false;
                }
            }

            return TurnResult.GOTO_TAKETURN;
        }
    }
}
