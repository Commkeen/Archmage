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
    class ActorBehavior_GenericShield:ActorBehavior
    {

        public string AbsorbDamageMessage;
        public string BreakShieldMessage;

        public ActorBehavior_GenericShield(int instanceID, int affectedActor)
            : base("b_genericShield", instanceID, affectedActor)
        {
            ShieldLayers = 1;
            ShieldStrength = 100;
            AbsorbDamageMessage = "Your shield absorbs some damage.";
            BreakShieldMessage = "The attack shatters your shield!";
        }

        public override TurnResult OnActorStartTurn()
        {
            ShieldStrength += 5;
            if (ShieldStrength >= 100)
                ShieldStrength = 100;
            return base.OnActorStartTurn();
        }

        public override int OnShieldLayerAbsorbsDamage(int damage)
        {
            _scene.WriteMessage(AbsorbDamageMessage);
            ShieldStrength -= damage;
            return base.OnShieldLayerAbsorbsDamage(0);
        }

        public override void OnShieldLayerBroken()
        {
            _scene.WriteMessage(BreakShieldMessage);
            ShieldLayers--;
            ShieldStrength = 100;
            if (ShieldLayers <= 0)
                ParentAbility.Deactivate();
            base.OnShieldLayerBroken();
        }

        int colorMin = 0;
        int colorMax = 180;
        int colorStep = 5;
        int currentColor = 0;
        public override Color GetColor(Color previousColor)
        {
            Color result = new Color(0, currentColor += colorStep, 250);
            if (currentColor >= colorMax || currentColor <= colorMin)
            {
                colorStep *= -1;
            }
            return result;
        }
    }
}
