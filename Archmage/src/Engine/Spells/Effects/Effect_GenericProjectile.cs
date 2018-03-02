using Archmage.Content.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.SpecialEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Spells.Effects
{
    class Effect_GenericProjectile:Effect
    {

        public List<Effect> EffectsOnStrike { get; protected set; } //Effects that happen when the projectile hits anything, shields or not. (TBI)
        public List<Effect> EffectsOnDamage { get; protected set; } //Effects that happen when the projectile gets thru an actor's shields.

        public Color ProjectileColor;

        protected AttackData _attack;
        protected int _finalActorHit;

        private List<Effect> _remainingSubEffects;

        public Effect_GenericProjectile(AttackData attack)
        {
            _attack = attack;
            EffectsOnStrike = new List<Effect>();
            EffectsOnDamage = new List<Effect>();
            ProjectileColor = new Color(libtcod.TCODColor.blue);
        }

        public override void Activate()
        {
            StartEffect();

            _remainingSubEffects = new List<Effect>();

            _finalActorHit = -1;
            if (_sourceTile != null)
                _attack.SourceCoordinate = _sourceTile;
            if (_targetTile != null)
                _attack.TargetCoordinate = _targetTile;

            //First check to see if the projectile will get to its intended target
            List<IntVector2> path = _scene.GetPathModifiedForCollisions(_attack.SourceCoordinate, _attack.TargetCoordinate);
            _attack.TargetCoordinate = path.Last();

            //Figure out if the attack will be evaded or not
            List<int> actorsThatEvaded = new List<int>();
            bool evaded = RollForEvasion(_attack.TargetCoordinate, false);
            while (evaded)
            {
                actorsThatEvaded.Add(_scene.GetActorsAtPosition(_attack.TargetCoordinate).First());
                _attack.TargetCoordinate = IntVector2.ExtendedLineBetweenPoints(_attack.SourceCoordinate, _attack.TargetCoordinate, 50).Last();
                _attack.TargetCoordinate = _scene.GetPathModifiedForCollisions(_attack.SourceCoordinate, _attack.TargetCoordinate, actorsThatEvaded).Last();
                evaded = RollForEvasion(_attack.TargetCoordinate, false);
            }

            path = _scene.GetPathModifiedForCollisions(_attack.SourceCoordinate, _attack.TargetCoordinate, actorsThatEvaded);

            //Show attack special effect and trigger rest of attack when finished
            int fxId = _scene.GetGameObjectPool().CreateSpecialEffect("specialEffect_basicProjectile");
            SpecialEffect_BasicProjectile fx = (SpecialEffect_BasicProjectile)_scene.GetGameObjectPool().GetSpecialEffect(fxId);
            fx.Init(_attack.SourceCoordinate, path.Last(), ProjectileColor, new SpecialEffect.OnSpecialEffectEndCallback(OnSpecialEffectEnd));
            _scene.AddSpecialEffect(fxId);
        }

        protected void OnSpecialEffectEnd()
        {
            if (_scene.GetActorsAtPosition(_attack.TargetCoordinate).Count > 0)
                _finalActorHit = _scene.GetActorsAtPosition(_attack.TargetCoordinate).First();
            bool doesAttackPenetrate = CalculateProjectileHit(_attack, _finalActorHit);

            foreach (Effect e in EffectsOnStrike)
            {
                e.SetSource(_attack.TargetCoordinate);
                e.SetTarget(_attack.TargetCoordinate);
                _remainingSubEffects.Add(e);
            }

            if (_finalActorHit != -1 && doesAttackPenetrate)
            {
                foreach (Effect e in EffectsOnDamage)
                {
                    e.SetTarget(_finalActorHit);
                    _remainingSubEffects.Add(e);
                }
            }

            StartNextSubEffect();
        }

        protected void StartNextSubEffect()
        {
            if (_remainingSubEffects.Count == 0)
            {
                EndEffect();
            }
            else
            {
                Effect subEffect = _remainingSubEffects.First();
                subEffect.OnEffectEndCallback = StartNextSubEffect;
                _remainingSubEffects.RemoveAt(0);
                subEffect.Activate();
            }
        }
    }
}
