using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Behaviors;
using Archmage.Actors;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Engine.Spells;
using Archmage.SpecialEffects;
using Archmage.Content.SpecialEffects;

namespace Archmage.Content.Behaviors.ActorBehaviors
{
    class ActorBehavior_BoundSpirit:ActorBehavior
    {
        IntVector2 currentTarget;

        int range = 5;
        int damage = 1;

        public ActorBehavior_BoundSpirit(int instanceID, int affectedActor)
            : base("b_boundSpirit", instanceID, affectedActor)
        {
        }

        public override TurnResult OnActorStartTurn()
        {
            Actor parent = _scene.GetGameObjectPool().GetActor(AffectedActor);
            
            List<int> enemies = _scene.GetActorsWithinDistance(parent.Position, range);
            enemies.Remove(parent.InstanceID);

            Actor tgtEnemy = null;
            for (int i = 0; i < enemies.Count; i++)
            {
                tgtEnemy = _scene.GetGameObjectPool().GetActor(enemies[i]);
                if (_scene.IsTileShootableFromLocation(parent.Position, tgtEnemy.Position, range))
                {
                    break;
                }
                else
                {
                    tgtEnemy = null;
                }
            }

            if (tgtEnemy != null)
            {
                currentTarget = tgtEnemy.Position;
                _scene.WriteMessage("Your bound spirit shoots a monster!");
                int fxId = _scene.GetGameObjectPool().CreateSpecialEffect("specialEffect_basicProjectile");
                SpecialEffect_BasicProjectile fx = (SpecialEffect_BasicProjectile)_scene.GetGameObjectPool().GetSpecialEffect(fxId);
                fx.Init(parent.Position, currentTarget, new Color(libtcod.TCODColor.lightBlue), new SpecialEffect.OnSpecialEffectEndCallback(OnSpecialEffectEnd));
                _scene.AddSpecialEffect(fxId);
            }

            return base.OnActorStartTurn();
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
                dmg.SourceID = AffectedActor;
                dmg.TargetID = tgt.InstanceID;
                dmg.Magnitude = damage;

                dmg = _scene.GetGameObjectPool().GetActor(AffectedActor).OnSendDamage(dmg);

                tgt.TakeDamage(dmg);
            }
        }
    }
}
