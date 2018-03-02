using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Behaviors;
using Archmage.Engine.Spells;
using Archmage.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Content.SpecialEffects;
using Archmage.Content.Behaviors.ActorBehaviors;

namespace Archmage.Content.Spells
{
    class Spell_Dispel:Spell
    {

        public Spell_Dispel(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "dispel";
            Name = "Dispel";
            TargetingType = SpellTargetingType.ENEMY;
            Range = 5;
            CooldownTime = 10;
            AttentionCost = 3;
            SoulCost = 1;

            Description = "Dispels all effects on the target.";
        }

        public override bool CastAtTarget(IntVector2 target)
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            List<int> targets = _scene.GetActorsAtPosition(target);
            //We can't cast if there's nobody there
            if (targets.Count == 0)
            {
                return false;
            }
            Actor tgtActor = _scene.GetGameObjectPool().GetActor(targets[0]);

            _scene.WriteMessage("You disrupt all spells on the monster!");

            List<int> behaviors = new List<int>(tgtActor.GetBehaviors());
            foreach (int i in behaviors)
            {
                tgtActor.RemoveBehavior(i);
            }

            StartCooldown();

            return true;
        }
    }
}
