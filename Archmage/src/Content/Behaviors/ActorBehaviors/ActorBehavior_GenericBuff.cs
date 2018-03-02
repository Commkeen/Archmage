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

namespace Archmage.Content.Behaviors.ActorBehaviors
{
    class ActorBehavior_GenericBuff:ActorBehavior
    {

        public ActorBehavior_GenericBuff(int instanceID, int affectedActor)
            : base("b_genericBuff", instanceID, affectedActor)
        {
            
        }
    }
}
