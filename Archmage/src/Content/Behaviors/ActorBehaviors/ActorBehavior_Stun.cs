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
    class ActorBehavior_Stun:ActorBehavior
    {

        public int TurnsRemaining { get; set; }

        public ActorBehavior_Stun(int instanceID, int affectedActor)
            : base("b_stun", instanceID, affectedActor)
        {
            TurnsRemaining = 5; //Just a default
        }

        public override TurnResult OnActorStartTurn()
        {
            TurnsRemaining--;
            if (TurnsRemaining <= 0)
            {
                _scene.WriteMessage("The stun wears off.");
                if (ParentAbility != null)
                    ParentAbility.Deactivate();
                else
                {
                    _scene.GetGameObjectPool().GetActor(AffectedActor).RemoveBehavior(this.InstanceID);
                    Alive = false;
                }
            }
            return TurnResult.GOTO_ENDTURN;
        }
    }
}
