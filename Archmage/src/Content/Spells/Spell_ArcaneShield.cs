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
    class Spell_ArcaneShield:Spell
    {

        public Spell_ArcaneShield(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "arcaneShield";
            Name = "Mystic Aura";
            TargetingType = SpellTargetingType.NONE;

            CooldownTime = 10;
            AttentionCost = 2;

            MaxUpgradeLevel = 5;

            Description = "Creates a shield to protect you from damage.  The shield can be broken by rapid magical attacks, and is useless against physical attacks.";
        }

        public override bool Cast()
        {
            BehaviorIDs.Add(_scene.GetGameObjectPool().CreateActorBehavior("b_genericShield", OwnerID));
            _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).ShieldLayers = 1;
            if (UpgradeLevel == 5)
            {
                _scene.GetGameObjectPool().GetActorBehavior(BehaviorIDs[0]).ShieldLayers = 3;
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
                return "-3 Cooldown";
            if (level == 2)
                return "Casts instantly";
            if (level == 3)
                return "-1 Attention Cost";
            if (level == 4)
                return "Mystic Barrier: 2 extra shield points";


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
