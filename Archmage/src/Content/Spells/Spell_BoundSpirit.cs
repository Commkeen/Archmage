using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Engine.Spells;
using Archmage.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Content.SpecialEffects;
using Archmage.Behaviors;

namespace Archmage.Content.Spells
{
    class Spell_BoundSpirit : Spell
    {

        int currentBehaviorID;

        public Spell_BoundSpirit(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "boundSpirit";
            Name = "Bound Spirit";
            CooldownTime = 12;
            AttentionCost = 4;
            TargetingType = SpellTargetingType.NONE;
            currentBehaviorID = 0;
            SoulCost = 1;

            Description = "Summons a small spirit that will attack nearby enemies while the spell is active.";
        }

        public override bool Cast()
        {
            currentBehaviorID = _scene.GetGameObjectPool().CreateActorBehavior("b_boundSpirit", OwnerID);
            _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID).ParentAbility = this;
            _scene.GetGameObjectPool().GetActor(OwnerID).AddBehavior(currentBehaviorID);

            _scene.WriteMessage("A small orb materializes and starts to float around you.");

            IsActive = true;
            return true;
        }

        public override void Deactivate()
        {
            _scene.WriteMessage("Your bound spirit dissapates.");
            ActorBehavior b = _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID);
            if (b != null)
                b.Alive = false;
            IsActive = false;
            _scene.GetGameObjectPool().GetActor(OwnerID).RemoveBehavior(currentBehaviorID);
            StartCooldown();
        }
    }
}
