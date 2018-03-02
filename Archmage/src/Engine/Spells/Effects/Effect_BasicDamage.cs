using Archmage.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Spells.Effects
{
    class Effect_BasicDamage:Effect
    {
        protected DamageData _damage;

        public Effect_BasicDamage(DamageData damage)
        {
            _damage = damage;
        }

        public override void Activate()
        {
            DealDamage(_targetActorID, _damage);
            EndEffect();
        }
    }
}
