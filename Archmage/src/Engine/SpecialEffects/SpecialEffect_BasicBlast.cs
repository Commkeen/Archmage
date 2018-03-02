using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.SpecialEffects;
using Archmage.Engine.Scenes;
using Archmage.Engine.DataStructures;

namespace Archmage.Content.SpecialEffects
{
    class SpecialEffect_BasicBlast:SpecialEffect
    {
        int _timer;
        int _radiusStepIn;
        int _radiusStepOut;
        int _maxRadius;
        IntVector2 _center;
        List<IntVector2> _affectedTiles;

        public Color color;

        public SpecialEffect_BasicBlast(int instanceID)
            : base("specialEffect_basicBlast", instanceID)
        {
            color = new Color(libtcod.TCODColor.darkRed);
            _affectedTiles = new List<IntVector2>();
        }

        public void Init(Color color, IntVector2 center, int maxRadius, List<IntVector2> affectedTiles, OnSpecialEffectEndCallback onEnd)
        {
            this.color = color;
            _onEnd = onEnd;
            _affectedTiles = affectedTiles;
            _center = center;
            _maxRadius = maxRadius;
            _timer = 0;
            _radiusStepIn = 0;
            _radiusStepOut = 0;
            Awake = true;
        }

        public override void Update()
        {
            if (!Awake)
                return;

            _timer++;
            if (_timer >= 3)
            {
                if (_radiusStepIn >= _maxRadius)
                {
                    if (_radiusStepOut >= _maxRadius)
                    {
                        EndEffect();
                    }
                    else
                    {
                        _radiusStepOut++;
                        _timer = 0;
                    }
                }
                else
                {
                    _radiusStepIn++;
                    _timer = 0;
                }
            }
        }

        public override List<Tuple<IntVector2, char, Color>> Draw()
        {
            List<Tuple<IntVector2, char, Color>> sprites = new List<Tuple<IntVector2, char, Color>>();
            foreach (IntVector2 position in _affectedTiles)
            {
                int magnitude = (position - _center).Magnitude();
                if (magnitude <= _radiusStepIn && magnitude >= _radiusStepOut)
                {
                    sprites.Add(new Tuple<IntVector2, char, Color>(position, '*', color));
                }
            }
            return sprites;
        }


    }
}
