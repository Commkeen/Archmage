using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Engine.Scenes;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Spells;

namespace Archmage.Behaviors
{
    class ActorBehavior:GameObject, ITakesTurns
    {

        //Energy determines if you can act this turn
        protected int _currentEnergy;
        protected int _energyPerTick; //How much energy this actor gets every tick
        protected int _energyToAct; //How much energy it takes to act
        protected bool _currentlyTakingTurn; //Whether you're taking a turn right now
        protected int _priority; //When multiple actors have a turn this tick, the higher priority number goes first.


        public int AffectedActor { get; set; }

        /// <summary>
        /// Starts as null, but can be set by parent ability, if any.
        /// The ActorBehavior can tell the ability if it dies, or destroy itself if the parent ability dies.
        /// </summary>
        public Ability ParentAbility { get; set; }

        /// <summary>
        /// The attributes that this ActorBehavior puts on the affected actor.
        /// </summary>
        public List<string> ActorAttributes { get; set; }

        /// <summary>
        /// The armor bonus, if any, that this behavior gives the affected actor.
        /// </summary>
        public int ArmorBonus { get; set; }

        public int EvasionBonus { get; set; }

        public int DisplacementBonus { get; set; }

        /// <summary>
        /// The shield layers, if any, that this behavior gives the affected actor.
        /// </summary>
        public int ShieldLayers { get; set; }

        /// <summary>
        /// If this behavior gives shields, this is the strength of the outermost shield layer for this behavior.
        /// </summary>
        public int ShieldStrength { get; set; }

        public ActorBehavior(string objID, int instanceID, int affectedActor)
            : base(objID, instanceID)
        {
            AffectedActor = affectedActor;
            ActorAttributes = new List<string>();
            ArmorBonus = 0;
            ShieldLayers = 0;
            ShieldStrength = 0;
        }

        #region ITakesTurns members
        public int GetPriority()
        {
            throw new NotImplementedException();
        }

        public bool IsReadyForTurn()
        {
            throw new NotImplementedException();
        }

        public bool IsCurrentlyTakingTurn()
        {
            throw new NotImplementedException();
        }

        public void OnTick()
        {
            throw new NotImplementedException();
        }

        public TurnResult StartTurn()
        {
            throw new NotImplementedException();
        }

        public TurnResult TakeTurn()
        {
            throw new NotImplementedException();
        }

        public TurnResult EndTurn()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Actor hooks

        public virtual DamageData OnSendDamage(DamageData damage)
        {
            return damage;
        }

        public virtual DamageData OnIncomingDamage(DamageData damage)
        {
            return damage;
        }

        public virtual int OnShieldLayerAbsorbsDamage(int damage)
        {
            return damage;
        }

        public virtual void OnShieldLayerBroken()
        {
            return;
        }

        public virtual void OnArmorAbsorbsDamage()
        {
            return;
        }

        public virtual int OnHeal(int healAmount)
        {
            return healAmount;
        }

        public virtual void OnZeroHP()
        {
            if (ParentAbility != null)
                ParentAbility.Deactivate();
            else
            {
                _scene.GetGameObjectPool().GetActor(AffectedActor).RemoveBehavior(this.InstanceID);
                Alive = false;
            }
        }

        public virtual TurnResult OnActorStartTurn()
        {
            return TurnResult.GOTO_TAKETURN;
        }

        public virtual TurnResult OnEndTurn()
        {
            return TurnResult.TURN_OVER;
        }

        public virtual char GetSprite(char previousSprite)
        {
            return previousSprite;
        }

        public virtual Color GetColor(Color previousColor)
        {
            return previousColor;
        }

        #endregion

    }
}
