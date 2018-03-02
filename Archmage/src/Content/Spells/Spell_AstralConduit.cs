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
    class Spell_AstralConduit : Spell
    {

        int currentBehaviorID;

        public Spell_AstralConduit(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "astralConduit";
            Name = "Astral Conduit";
            CooldownTime = 8;
            AttentionCost = 2;
            TargetingType = SpellTargetingType.NONE;
            currentBehaviorID = 0;

            Description = "While active, the astral conduit will add 3 damage to all your magical attacks.";
        }

        public override bool Cast()
        {
            currentBehaviorID = _scene.GetGameObjectPool().CreateActorBehavior("b_astralConduit", OwnerID);
            _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID).ParentAbility = this;
            _scene.GetGameObjectPool().GetActor(OwnerID).AddBehavior(currentBehaviorID);

            _scene.WriteMessage("Your hands shimmer with a blue light!");
            _scene.WriteMessage("Your spells feel more powerful.");

            IsActive = true;
            return true;
        }

        public override void Deactivate()
        {
            _scene.WriteMessage("The light around your hands fades.");
            ActorBehavior b = _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID);
            if (b != null)
                b.Alive = false;
            IsActive = false;
            _scene.GetGameObjectPool().GetActor(OwnerID).RemoveBehavior(currentBehaviorID);
            StartCooldown();
        }
    }
}
