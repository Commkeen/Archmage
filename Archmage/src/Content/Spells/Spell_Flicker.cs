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
    class Spell_Flicker:Spell
    {

        public Spell_Flicker(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "flicker";
            Name = "Flicker";
            TargetingType = SpellTargetingType.NONE;

            CooldownTime = 8;
            AttentionCost = 1;

            MaxUpgradeLevel = 5;

            Description = ".";
        }

        public override bool Cast()
        {
            BehaviorIDs.Add(_scene.GetGameObjectPool().CreateActorBehavior("b_genericBuff", OwnerID));
            _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).EvasionBonus = 2;
            if (UpgradeLevel > 1)
            {
                _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).EvasionBonus = 3;
            }
            if (UpgradeLevel == 5)
            {
                _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).DisplacementBonus = 1;
            }
            _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).ParentAbility = this;
            _scene.GetGameObjectPool().GetActor(OwnerID).AddBehavior(BehaviorIDs[0]);

            IsActive = true;
            return true;
        }

        public override void Deactivate()
        {
            if (BehaviorIDs.Count > 0)
            {
                ActorBehavior b = _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]);
                if (b != null)
                    b.Alive = false;
                _scene.GetGameObjectPool().GetActor(OwnerID).RemoveBehavior(BehaviorIDs[0]);
                BehaviorIDs.Clear();
            }
            IsActive = false;
            StartCooldown();
        }

        public override string UpgradeDescription(int level)
        {
            if (level == 1)
                return "+1 Evasion";
            if (level == 2)
                return "Half cooldown";
            if (level == 3)
                return "No resource Cost";
            if (level == 4)
                return "Shimmer: 1 extra Displacement point";


            return base.UpgradeDescription(level);
        }

        public override bool GainLevel()
        {
            if (UpgradeLevel == 1)
            {
                CooldownModifier = -3;
                UpgradeLevel++;
                return true;
            }
            if (UpgradeLevel == 2)
            {
                EnergyModifier = 0 - EnergyCost;
                UpgradeLevel++;
                return true;
            }
            if (UpgradeLevel == 3)
            {
                AttentionModifier = -1;
                UpgradeLevel++;
                return true;
            }
            if (UpgradeLevel == 4)
            {
                Name = "Mystic Barrier";
                UpgradeLevel++;
                return true;
            }

            return false;
        }
    }
}
