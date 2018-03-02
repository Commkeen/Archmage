using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Engine.DataStructures;

namespace Archmage.SpecialEffects
{
    /// <summary>
    /// A SpecialEffect is an animation or sound clip that blocks gameplay until it is finished.
    /// </summary>
    class SpecialEffect:GameObject
    {

        public delegate void OnSpecialEffectEndCallback();
        protected OnSpecialEffectEndCallback _onEnd;

        public bool Awake { get; set; }

        public SpecialEffect(string objectID, int instanceID)
            : base(objectID, instanceID)
        {
            Awake = false;
            _onEnd = null;
        }


        public virtual void Update()
        {
            
        }

        public virtual List<Tuple<IntVector2, char, Color>> Draw()
        {
            return new List<Tuple<IntVector2, char, Color>>();
        }

        protected void EndEffect()
        {
            if (_onEnd != null)
                _onEnd();
            Alive = false;
        }

    }
}
