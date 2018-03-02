using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.DataStructures
{
    public enum AttackType { MELEE, PHYSICAL, ARCANE, ENERGY, BLAST, DARK, SPECIAL };
    public enum ElementType { NONE, PHYSICAL, CURSE, POISON, PSYCHIC, ELECTRIC, COLD };

    /// <summary>
    /// The AttackData structure represents the type and effect of a single attack.
    /// Every attack in the game is directed from a specific Actor to a target coordinate.
    /// 
    /// Steps for an attack:
    /// 1. Every AttackData is created by an Effect.  The Effect creates an AttackData object with the relevant info and
    /// calls LaunchAttack() in the Effect base class.  If multiple targets are hit at once, LaunchAttack() is called once
    /// for each target.
    /// 2. LaunchAttack() handles all the attack calculations.  It finds the target at the relevant square and first checks
    /// for evasion if necessary, then for shields and resistance.
    /// 
    /// </summary>
    public struct AttackData
    {
        public AttackType Type;
        public ElementType Element;

        public IntVector2 SourceCoordinate;
        public IntVector2 TargetCoordinate;

        /*
        public AttackData()
        {
            Type = AttackType.PHYSICAL;
            Element = ElementType.NONE;

            TargetCoordinate = new IntVector2(0, 0);
        }
         * */
    }
}
