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
    class Spell_XrayVision:Spell
    {

        int currentBehaviorID;

        public Spell_XrayVision(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "xrayVision";
            Name = "Xray Vision";
            TargetingType = SpellTargetingType.NONE;
            currentBehaviorID = 0;

            Description = "debug";
        }

        public override bool Cast()
        {
            currentBehaviorID = _scene.GetGameObjectPool().CreateActorBehavior("b_xrayVision", OwnerID);
            _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID).ParentAbility = this;
            _scene.GetGameObjectPool().GetActor(OwnerID).AddBehavior(currentBehaviorID);

            IsActive = true;
            return true;
        }

        public override void Deactivate()
        {
            ActorBehavior b = _scene.GetGameObjectPool().GetActorBehavior(currentBehaviorID);
            if (b != null)
                b.Alive = false;
            IsActive = false;
            _scene.GetGameObjectPool().GetActor(OwnerID).RemoveBehavior(currentBehaviorID);
            StartCooldown();
        }
    }
}
