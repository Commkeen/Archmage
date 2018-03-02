﻿using System;
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
    class Spell_MagicMissile:Spell
    {

        IntVector2 currentTarget;

        int damage = 4;

        public Spell_MagicMissile(PlayScene scene, int ownerID)
            : base(ownerID)
        {
            ID = "magicMissile";
            Name = "Magic Missile";
            TargetingType = SpellTargetingType.ENEMY;
            Range = 6;
            CooldownTime = 6;
            AttentionCost = 1;
            SoulCost = 1;

            Description = "Does 4 magical damage.";
        }

        public override bool CastAtTarget(IntVector2 target)
        {
            Actor currentCaster = _scene.GetGameObjectPool().GetActor(OwnerID);
            currentTarget = target;

            _scene.WriteMessage("You hurl a magic missile!");

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
            fx.Init(currentCaster.Position, currentTarget, new Color(libtcod.TCODColor.blue), new SpecialEffect.OnSpecialEffectEndCallback(OnSpecialEffectEnd));
            _scene.AddSpecialEffect(fxId);

            return true;
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
