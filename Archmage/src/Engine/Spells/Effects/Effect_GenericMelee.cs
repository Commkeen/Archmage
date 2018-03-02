using Archmage.Content.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.SpecialEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Spells.Effects
{
    class Effect_GenericMelee:Effect
    {

        public List<Effect> EffectsOnStrike { get; protected set; } //Effects that happen when the projectile hits anything, shields or not. (TBI)
        public List<Effect> EffectsOnDamage { get; protected set; } //Effects that happen when the projectile gets thru an actor's shields.

        protected AttackData _attack;
        protected int _finalActorHit;

        public Effect_GenericMelee(AttackData attack)
        {
            _attack = attack;
            EffectsOnStrike = new List<Effect>();
            EffectsOnDamage = new List<Effect>();
        }

        public override void Activate()
        {
            StartEffect();
            if (_sourceTile != null)
                _attack.SourceCoordinate = _sourceTile;
            if (_targetTile != null)
                _attack.TargetCoordinate = _targetTile;

            //First check to see if the attack will get to its intended target
            List<IntVector2> path = _scene.GetBestRouteBetweenPositions(_attack.SourceCoordinate, _attack.TargetCoordinate);
            _attack.TargetCoordinate = path.Last();

            //Show attack special effect and trigger rest of attack when finished
            int fxId = _scene.GetGameObjectPool().CreateSpecialEffect("specialEffect_basicMelee");
            SpecialEffect_BasicMelee fx = (SpecialEffect_BasicMelee)_scene.GetGameObjectPool().GetSpecialEffect(fxId);
            
            fx.Init(_attack.SourceCoordinate, _attack.TargetCoordinate, new SpecialEffect.OnSpecialEffectEndCallback(OnSpecialEffectEnd));
            _scene.AddSpecialEffect(fxId);
        }

        protected void OnSpecialEffectEnd()
        {
            int _actorHit = -1;
            if (_scene.GetActorsAtPosition(_attack.TargetCoordinate).Count > 0)
                _actorHit = _scene.GetActorsAtPosition(_attack.TargetCoordinate).First();

            bool attackPenetrates = false;
            bool targetEvades = RollForEvasion(_attack.TargetCoordinate, false);
            if (!targetEvades)
                attackPenetrates = CalculateProjectileHit(_attack, _actorHit);

            foreach (Effect e in EffectsOnStrike)
            {
                e.SetSource(_attack.TargetCoordinate);
                e.Activate();
            }
            if (_actorHit != -1 && !targetEvades && attackPenetrates)
            {
                foreach (Effect e in EffectsOnDamage)
                {
                    e.SetTarget(_actorHit);
                    e.Activate();
                }
            }

            EndEffect();
        }
    }
}
