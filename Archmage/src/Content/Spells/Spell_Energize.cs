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
    class Spell_Energize : Spell
    {

        int currentBehaviorID;

        public Spell_Energize(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "energize";
            Name = "Energize";
            CooldownTime = 10;
            AttentionCost = 2;
            TargetingType = SpellTargetingType.NONE;
            currentBehaviorID = 0;
            SoulCost = 1;

            Description = "While active, turns your magical attacks into energy attacks, allowing them to damage magic-resistant enemies.";
        }

        public override bool Cast()
        {
            currentBehaviorID = _scene.GetGameObjectPool().CreateActorBehavior("b_energize", OwnerID);
            _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID).ParentAbility = this;
            _scene.GetGameObjectPool().GetActor(OwnerID).AddBehavior(currentBehaviorID);

            _scene.WriteMessage("Your hands crackle with energy!");
            _scene.WriteMessage("Your spells will be infused with energy damage.");

            IsActive = true;
            return true;
        }

        public override void Deactivate()
        {
            _scene.WriteMessage("The energy around your hands fades.");
            ActorBehavior b = _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID);
            if (b != null)
                b.Alive = false;
            IsActive = false;
            _scene.GetGameObjectPool().GetActor(OwnerID).RemoveBehavior(currentBehaviorID);
            StartCooldown();
        }
    }
}
