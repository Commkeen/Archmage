using Archmage.Actors;
using Archmage.Content.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.SpecialEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Spells.Effects
{
    class Effect_Blast:Effect
    {

        public List<Effect> EffectsOnEachActor { get; protected set; } //Effects that happen to each actor in the blast range.
        public List<Effect> EffectsOnEachTile { get; protected set; } //Effects that happen to each tile in the blast range.
        public List<int> ExcludedActors { get; protected set; } //Exclude these actors from the effect (for example, the caster)

        public Color BlastColor;

        protected AttackData _attack;

        protected int _radius;

        protected List<IntVector2> _affectedTiles;

        public Effect_Blast(AttackData attack, int radius)
        {
            _attack = attack;
            _radius = radius;
            EffectsOnEachActor = new List<Effect>();
            EffectsOnEachTile = new List<Effect>();
            ExcludedActors = new List<int>();
            BlastColor = new Color(libtcod.TCODColor.red);
        }

        public override void Activate()
        {
            StartEffect();
            if (_sourceTile != null)
                _attack.SourceCoordinate = _sourceTile;
            if (_targetTile != null)
                _attack.TargetCoordinate = _targetTile;

            //Assemble a list of the affected tiles
            _affectedTiles = _scene.GetTilesWithinDistance(_attack.TargetCoordinate, _radius, false);
            int i = 0;
            while (i < _affectedTiles.Count)
            {
                if (_scene.IsTileShootableFromLocation(_attack.TargetCoordinate, _affectedTiles[i], _radius))
                {
                    i++;
                }
                else
                {
                    _affectedTiles.RemoveAt(i);
                }
            }

            //Show attack special effect and trigger rest of attack when finished
            int fxId = _scene.GetGameObjectPool().CreateSpecialEffect("specialEffect_basicBlast");
            SpecialEffect_BasicBlast fx = (SpecialEffect_BasicBlast)_scene.GetGameObjectPool().GetSpecialEffect(fxId);

            fx.Init(BlastColor, _attack.SourceCoordinate, _radius, _affectedTiles, new SpecialEffect.OnSpecialEffectEndCallback(OnSpecialEffectEnd));
            _scene.AddSpecialEffect(fxId);
        }

        protected void OnSpecialEffectEnd()
        {
            //Launch the blast attack at each actor in the radius
            foreach (IntVector2 position in _affectedTiles)
            {
                if (_scene.GetActorsAtPosition(position).Count > 0)
                {
                    Actor a = _scene.GetGameObjectPool().GetActor(_scene.GetActorsAtPosition(position)[0]);
                    if (!ExcludedActors.Contains(a.InstanceID)) //Don't affect this actor if it's excluded
                    {
                        bool doesAttackPenetrate = CalculateBlastHit(_attack, a.InstanceID);
                        if (a != null && doesAttackPenetrate)
                        {
                            foreach (Effect e in EffectsOnEachActor)
                            {
                                e.SetTarget(a.InstanceID);
                                e.Activate();
                            }
                        }
                    }
                }
                foreach (Effect e in EffectsOnEachTile)
                {
                    e.SetSource(position);
                    e.Activate();
                }
            }

            EndEffect();
        }
    }
}
