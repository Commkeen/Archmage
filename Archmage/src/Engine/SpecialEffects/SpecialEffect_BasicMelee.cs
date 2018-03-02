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
    class SpecialEffect_BasicMelee:SpecialEffect
    {
        int _timer;
        IntVector2 _target;

        public Color color;

        public SpecialEffect_BasicMelee(int instanceID)
            : base("specialEffect_basicMelee", instanceID)
        {
            color = new Color(libtcod.TCODColor.darkRed);
        }

        public void Init(IntVector2 source, IntVector2 destination, OnSpecialEffectEndCallback onEnd)
        {
            _onEnd = onEnd;
            _target = destination;
            _timer = 0;
            Awake = true;
        }

        public override void Update()
        {
            if (!Awake)
                return;

            _timer++;
            if (_timer >= 5)
            {
                EndEffect();
            }
        }

        public override List<Tuple<IntVector2, char, Color>> Draw()
        {
            List<Tuple<IntVector2, char, Color>> sprites = new List<Tuple<IntVector2, char, Color>>();
            sprites.Add(new Tuple<IntVector2, char, Color>(_target, '#', color));
            return sprites;
        }


    }
}
