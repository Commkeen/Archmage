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
    class SpecialEffect_BasicProjectile:SpecialEffect
    {

        List<IntVector2> path;
        int _currentLocation;
        Color _projectileColor;

        public SpecialEffect_BasicProjectile(int instanceID)
            : base("specialEffect_basicProjectile", instanceID)
        {

        }

        public void Init(IntVector2 source, IntVector2 destination, Color projectileColor, OnSpecialEffectEndCallback onEnd)
        {
            _onEnd = onEnd;
            path = IntVector2.LineBetweenPoints(source, destination);
            _currentLocation = 0;
            _projectileColor = projectileColor;
            Awake = true;
        }

        public override void Update()
        {
            if (!Awake)
                return;

            _currentLocation++;
            if (_currentLocation >= path.Count - 1)
            {
                EndEffect();
            }
        }

        public override List<Tuple<IntVector2, char, Color>> Draw()
        {
            List<Tuple<IntVector2, char, Color>> sprites = new List<Tuple<IntVector2, char, Color>>();
            sprites.Add(new Tuple<IntVector2, char, Color>(path[_currentLocation], '*', _projectileColor));
            return sprites;
        }


    }
}
