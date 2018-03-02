using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.Engine.DataStructures
{
    public struct DamageData
    {
        public int SourceID;
        public int TargetID;

        public int Magnitude;

        public AttackType DmgAttackType;
        public ElementType DmgElement;
        public bool DamagesWalls;
    }
}
