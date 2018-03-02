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
    class Spell_Quickshot:Spell
    {

        IntVector2 currentTarget;

        int damage = 1;

        public Spell_Quickshot(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "quickshot";
            Name = "Quickshot";
            TargetingType = SpellTargetingType.ENEMY;
            Range = 4;
            CooldownTime = 12;
            AttentionCost = 1;

            Description = "Does 1 magical damage without taking a turn.";
        }

        public override bool CastAtTarget(IntVector2 target)
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            currentTarget = target;

            _scene.WriteMessage("You fire off a bolt with barely a thought!");

            //TODO: Find out if the target dodges the attack and then hit a target behind it

            //Check the current trajectory of the missile and adjust for collisions along the way
            List<IntVector2> path = IntVector2.LineBetweenPoints(currentCaster.Position, currentTarget);
            int currentIndex = 1;
            while (currentIndex < path.Count - 1)
            {
                if (!_scene.IsPositionFree(path[currentIndex]))
                {
                    currentTarget = path[currentIndex];
                    path = IntVector2.LineBetweenPoints(currentCaster.Position, currentTarget);
                    currentIndex = 1;
                }
                else
                {
                    currentIndex++;
                }
            }

            int fxId = _scene.GetGameObjectPool().CreateSpecialEffect("specialEffect_basicProjectile");
            SpecialEffect_BasicProjectile fx = (SpecialEffect_BasicProjectile)_scene.GetGameObjectPool().GetSpecialEffect(fxId);
            fx.Init(currentCaster.Position, currentTarget, new Color(libtcod.TCODColor.white), new SpecialEffect.OnSpecialEffectEndCallback(OnSpecialEffectEnd));
            _scene.AddSpecialEffect(fxId);

            return false;
        }

        public void OnSpecialEffectEnd()
        {
            //Find out what's at the target
            List<int> targets = _scene.GetActorsAtPosition(currentTarget);

            if (targets.Count == 0)
            {
                //The missile hits the tile
            }
            else
            {
                //The missile hits the first actor at the position
                Actor tgt = _scene.GetGameObjectPool().GetActor(targets.First());
                DamageData dmg = new DamageData();
                dmg.DmgAttackType = AttackType.ARCANE;
                dmg.SourceID = OwnerID;
                dmg.TargetID = tgt.InstanceID;
                dmg.Magnitude = damage;

                dmg = _scene.GetGameObjectPool().GetActor(OwnerID).OnSendDamage(dmg);

                tgt.TakeDamage(dmg);
            }

            StartCooldown();
        }
    }
}
