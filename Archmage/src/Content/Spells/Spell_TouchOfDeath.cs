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

namespace Archmage.Content.Spells
{
    class Spell_TouchOfDeath:Spell
    {

        IntVector2 currentTarget;

        int damage = 6;

        public Spell_TouchOfDeath(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "touchOfDeath";
            Name = "Touch of Death";
            TargetingType = SpellTargetingType.ENEMY;
            Range = 1;
            CooldownTime = 12;
            AttentionCost = 4;
            SoulCost = 1;

            Description = "Does 6 magic damage to an adjacent enemy.";
        }

        public override bool CastAtTarget(IntVector2 target)
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            currentTarget = target;

            


            //Find out what's at the target
            List<int> targets = _scene.GetActorsAtPosition(currentTarget);

            if (targets.Count == 0)
            {
                return false;
            }
            else
            {
                //The missile hits the first actor at the position
                _scene.WriteMessage("You drain the monster's life with a touch!");
                Actor tgt = _scene.GetGameObjectPool().GetActor(targets.First());
                DamageData dmg = new DamageData();
                dmg.DmgAttackType = AttackType.DARK;
                dmg.SourceID = OwnerID;
                dmg.TargetID = tgt.InstanceID;
                dmg.Magnitude = damage;

                dmg = _scene.GetGameObjectPool().GetActor(OwnerID).OnSendDamage(dmg);

                tgt.TakeDamage(dmg);
            }

            StartCooldown();
            return true;
        }
    }
}
