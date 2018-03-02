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
    class Spell_ScaledSkin : Spell
    {

        public Spell_ScaledSkin(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "scaledSkin";
            Name = "Scaled Skin";
            TargetingType = SpellTargetingType.NONE;

            Description = "While active, protects against physical attacks.";
        }

        public override bool Cast()
        {
            _scene.WriteMessage("You grow thick scales over your skin!");
            BehaviorIDs.Add(_scene.GetGameObjectPool().CreateActorBehavior("b_scaledSkin", OwnerID));
            _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).ParentAbility = this;
            _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).ArmorBonus = 1;
            _scene.GetGameObjectPool().GetActor(OwnerID).AddBehavior(BehaviorIDs[0]);

            IsActive = true;
            return true;
        }

        public override void Deactivate()
        {
            _scene.WriteMessage("Your scales fall off.");
            ActorBehavior b = _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]);
            if (b != null)
                b.Alive = false;
            IsActive = false;
            _scene.GetGameObjectPool().GetActor(OwnerID).RemoveBehavior(BehaviorIDs[0]);
            BehaviorIDs.Clear();
            StartCooldown();
        }
    }
}
