using Archmage.Actors;
using Archmage.Content.SpecialEffects;
using Archmage.Engine.DataStructures;
using Archmage.Engine.System;
using Archmage.SpecialEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Spells.Effects
{
    class Effect_PickRandomTargetsInRadius:Effect
    {

        public List<Effect> EffectsOnEachTarget { get; protected set; } //Effects that happen to each target that gets hit.

        protected int _radius;
        protected int _minTargets;
        protected int _maxTargets;

        public List<IntVector2> _remainingTargets;

        public Effect_PickRandomTargetsInRadius(int radius, int minTargets, int maxTargets)
        {
            _radius = radius;
            _minTargets = minTargets;
            _maxTargets = maxTargets;
            
            EffectsOnEachTarget = new List<Effect>();
        }

        public override void Activate()
        {
            StartEffect();

            _remainingTargets = new List<IntVector2>();

            List<IntVector2> _possibleTiles = _scene.GetTilesWithinDistance(_targetTile, _radius, false);
            _possibleTiles = _possibleTiles.OrderBy(a => Utility.random.Next()).ToList(); //Randomize

            int targetsToPick = Utility.random.Next(_minTargets, _maxTargets);

            for (int i = 0; i < targetsToPick && i < _possibleTiles.Count; i++)
            {
                _remainingTargets.Add(_possibleTiles[i]);
            }

            StartNextEffect();
        }

        protected void StartNextEffect()
        {
            if (_remainingTargets.Count == 0)
            {
                EndEffect();
            }
            else
            {
                foreach (Effect e in EffectsOnEachTarget)
                {
                    e.SetTarget(_remainingTargets.First());
                    e.OnEffectEndCallback = StartNextEffect;
                    e.Activate();
                    _remainingTargets.RemoveAt(0);
                }
            }
        }
    }
}
