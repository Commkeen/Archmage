using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.DataStructures
{
    /// <summary>
    /// The AttackResult structure tells us whether an attack attempt was successful, and what it did to an entity's defenses.
    /// 
    /// </summary>
    public struct AttackResult
    {
        public enum EvasionCheckResult { EVASION_FAILED, EVASION_DODGED, DISPLACEMENT_DODGED };
        public enum ShieldCheckResult { SHIELD_BYPASSED, SHIELD_PROTECTED, SHIELD_LAYERBROKE, SHIELD_SHIELDBROKE };

        /// <summary>
        /// Where the attack finally ends up.
        /// </summary>
        public IntVector2 FinalPosition;

        /// <summary>
        /// The Actor, if any, this attack ends up hitting.
        /// </summary>
        public int FinalActorHit;

        /// <summary>
        /// Result of the final target's evasion/displacement check.
        /// </summary>
        public EvasionCheckResult EvasionResult;

        /// <summary>
        /// Result of the final target's shield check.
        /// </summary>
        public ShieldCheckResult ShieldResult;

        /// <summary>
        /// A list of the actors this attack missed, so we know to ignore them when calculating the new trajectory of the attack
        /// </summary>
        public List<int> ActorsMissed;

        /*
        public AttackResult()
        {
            FinalPosition = new IntVector2(0, 0);
            EvasionResult = EvasionCheckResult.EVASION_FAILED;
            ShieldResult = ShieldCheckResult.SHIELD_BYPASSED;
            ArmorResult = 0;
            ResistanceProtected = false;
        }
         * */
    }
}
