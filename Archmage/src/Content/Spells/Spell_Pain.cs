﻿using System;
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
    class Spell_Pain:Spell
    {

        int _activeBehaviorID;

        public Spell_Pain(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "pain";
            Name = "Pain";
            TargetingType = SpellTargetingType.ENEMY;
            Range = 3;
            CooldownTime = 10;
            AttentionCost = 3;
            SoulCost = 1;

            _activeBehaviorID = 0;

            Description = "Causes 2 magical damage per turn while active.";
        }

        public override bool CastAtTarget(IntVector2 target)
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            //Can't cast on yourself!
            if (target == currentCaster.Position)
            {
                return false;
            }
            List<int> targets = _scene.GetActorsAtPosition(target);
            //We can't cast if there's nobody there
            if (targets.Count == 0)
            {
                return false;
            }
            Actor tgtActor = _scene.GetGameObjectPool().GetActor(targets[0]);

            _scene.WriteMessage("You begin to magically flay the creature apart!");

            _activeBehaviorID = _scene.GetGameObjectPool().CreateActorBehavior("b_damageOverTime", this.OwnerID);
            ActorBehavior_DamageOverTime behavior = (ActorBehavior_DamageOverTime)_scene.GetGameObjectPool().GetActorBehavior(_activeBehaviorID);

            behavior.ParentAbility = this;
            behavior.Damage.Magnitude = 2;
            behavior.Damage.DmgElement = ElementType.PHYSICAL;
            behavior.AffectedActor = tgtActor.InstanceID;
            behavior.TurnsRemaining = 5;

            tgtActor.AddBehavior(_activeBehaviorID);

            IsActive = true;

            return true;
        }

        public override void Deactivate()
        {
            IsActive = false;
            StartCooldown();

            //Remove behavior from target and kill behavior
            ActorBehavior behavior = _scene.GetGameObjectPool().GetActorBehavior(_activeBehaviorID);
            if (behavior != null)
            {
                Actor targetActor = _scene.GetGameObjectPool().GetActor(behavior.AffectedActor);
                if (targetActor != null)
                {
                    targetActor.RemoveBehavior(behavior.InstanceID);
                }

                behavior.Alive = false;
            }

            _activeBehaviorID = 0;

            base.Deactivate();
        }
    }
}
