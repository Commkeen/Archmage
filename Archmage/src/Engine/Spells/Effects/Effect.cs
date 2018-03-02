using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Spells.Effects
{
    abstract class Effect
    {
        /// <summary>
        /// This static reference to the PlayScene is updated every time a new PlayScene is constructed.
        /// That way we don't have to pass it in every time.
        /// </summary>
        protected static PlayScene _scene;

        protected IntVector2 _sourceTile;
        protected IntVector2 _targetTile;
        protected int _targetActorID;

        protected Ability _castingAbility; //The ability that triggered this effect
        protected int _casterID;
        protected int _upgradeLevel; //If any

        protected bool _initialized;

        public bool Finished { get; protected set; }

        public delegate void OnEffectEndCallbackDelegate();
        public OnEffectEndCallbackDelegate OnEffectEndCallback;

        #region Constructor and initialization
        public Effect()
        {
            _initialized = false;
            Finished = false;
            _targetTile = new IntVector2(0,0);
            _targetActorID = -1;
            _upgradeLevel = 0;
        }

        /// <summary>
        /// Called by PlayScene on construction to give all Effects a static reference to the scene
        /// </summary>
        /// <param name="scene"></param>
        public static void InitSceneReference(PlayScene scene)
        {
            _scene = scene;
        }

        /// <summary>
        /// Must be called when creating a new Effect!
        /// This keeps any variables common to all Effects from having to be passed through a constructor chain.
        /// </summary>
        public void Init(int casterID, Ability castingAbility)
        {
            _castingAbility = castingAbility;
            _casterID = casterID;
            _initialized = true;
        }
        #endregion

        public abstract void Activate();

        public void StartEffect()
        {
            Finished = false;
        }

        public void EndEffect()
        {
            Finished = true;
            if (OnEffectEndCallback != null)
                OnEffectEndCallback();
        }

        #region Data Mutators
        public void SetSource(IntVector2 sourceTile)
        {
            _sourceTile = sourceTile;
        }

        public void SetTarget(int targetActorID)
        {
            _targetActorID = targetActorID;
        }

        public void SetTarget(IntVector2 targetTile)
        {
            _targetTile = targetTile;
        }
        #endregion

        #region Common behaviors
        protected bool RollForEvasion(IntVector2 targetPosition, bool displacementOnly)
        {
            List<int> tgtIDs = _scene.GetActorsAtPosition(targetPosition);
            if (tgtIDs.Count <= 0)
                return false;

            return _scene.GetGameObjectPool().GetActor(tgtIDs[0]).RollForEvasion(displacementOnly);
        }

        /// <summary>
        /// This method takes data from a projectile that has hit a target, and determines whether it penetrates defenses and affects the target or not.
        /// Evasion is calculated elsewhere, this just takes into account shields and resistances.
        /// This also applies shield damage if necessary.
        /// Currently returns true if the attack penetrates, or false if it does not (or if no actor is present).
        /// </summary>
        /// <param name="atkData"></param>
        /// <returns></returns>
        protected bool CalculateProjectileHit(AttackData atkData, int actorID)
        {
            //Get target entity
            if (actorID == -1)
                return false;
            Actor a = _scene.GetGameObjectPool().GetActor(actorID);
            if (a == null)
                return false;

            //Find out shield result
            AttackResult.ShieldCheckResult shieldResult = a.CalculateAttackShieldPenetration(atkData);

            //Affect the actor's shields as necessary based on shield result
            if (shieldResult == AttackResult.ShieldCheckResult.SHIELD_LAYERBROKE
                    || shieldResult == AttackResult.ShieldCheckResult.SHIELD_SHIELDBROKE)
            {
                a.BreakShieldLayer();
                return false;
            }
            else if (shieldResult == AttackResult.ShieldCheckResult.SHIELD_PROTECTED)
            {
                a.TakeShieldDamage(10);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called on each individual actor in the path of a blast attack.
        /// Takes data from the blast attack, and determines whether it penetrates defenses and affects the target or not.
        /// Evasion is calculated elsewhere, this just takes into account shields and resistances.
        /// This also applies shield damage if necessary.
        /// Currently returns true if the attack penetrates, or false if it does not (or if no actor is present).
        /// 
        /// </summary>
        /// <param name="atkData"></param>
        /// <param name="actor"></param>
        /// <returns></returns>
        protected bool CalculateBlastHit(AttackData atkData, int actorID)
        {
            //Get the targeted entity
            if (actorID == -1)
                return false;
            Actor a = _scene.GetGameObjectPool().GetActor(actorID);
            if (a == null)
                return false;

            //Find out shield result
            AttackResult.ShieldCheckResult shieldResult = a.CalculateAttackShieldPenetration(atkData);

            //Affect the actor's shields as necessary based on shield result
            if (shieldResult == AttackResult.ShieldCheckResult.SHIELD_LAYERBROKE
                    || shieldResult == AttackResult.ShieldCheckResult.SHIELD_SHIELDBROKE)
            {
                a.BreakShieldLayer();
                return false;
            }
            else if (shieldResult == AttackResult.ShieldCheckResult.SHIELD_PROTECTED)
            {
                a.TakeShieldDamage(10);
                return false;
            }

            return true;
        }

        /*
        protected AttackResult LaunchAttack(AttackData atkData)
        {
            //NOTE: Not compatible with multiple actors occupying a single space!
            //Get the entity at the target position
            List<int> tgtIDs = _scene.GetActorsAtPosition(atkData.TargetCoordinate);
            if (tgtIDs.Count <= 0)
                return new AttackResult();

            Actor a = _scene.GetGameObjectPool().GetActor(tgtIDs[0]);
            AttackResult result = a.CalculateAttackSuccess(atkData);

            //TODO: Eventually projectile attacks will continue past enemies that dodge
            if (result.EvasionResult != AttackResult.EvasionCheckResult.EVASION_FAILED)
            {
                result.FinalPosition = IntVector2.ExtendedLineBetweenPoints(atkData.SourceCoordinate, atkData.TargetCoordinate, 50).Last();
            }

            if (result.EvasionResult == AttackResult.EvasionCheckResult.EVASION_FAILED)
            {
                if (result.ShieldResult == AttackResult.ShieldCheckResult.SHIELD_LAYERBROKE
                    || result.ShieldResult == AttackResult.ShieldCheckResult.SHIELD_SHIELDBROKE)
                {
                    a.BreakShieldLayer();
                }
                else if (result.ShieldResult == AttackResult.ShieldCheckResult.SHIELD_PROTECTED)
                {
                    a.TakeShieldDamage(10);
                }
            }

            return result;
        }
         * */

        protected void DealDamage(int targetActorID, DamageData damage)
        {
            Actor a = _scene.GetGameObjectPool().GetActor(targetActorID);
            if (a != null)
                _scene.GetGameObjectPool().GetActor(targetActorID).TakeDamage(damage);
        }
        #endregion

    }
}
