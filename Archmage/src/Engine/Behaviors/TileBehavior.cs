using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Mapping;
using Archmage.Engine.DataStructures;

namespace Archmage.Behaviors
{
    class TileBehavior : GameObject, ITakesTurns
    {

        public enum BehaviorLayer { BASE, LIQUID, SURFACE, AIR };
        protected BehaviorLayer _layer;

        //Energy determines if you can act this turn
        protected int _currentEnergy;
        protected int _energyPerTick; //How much energy this actor gets every tick
        protected int _energyToAct; //How much energy it takes to act
        protected bool _currentlyTakingTurn; //Whether you're taking a turn right now
        protected int _priority; //Whether this behavior responds to events and gets to act first

        public IntVector2 AffectedTile { get; set; }

        //Display stuff
        protected char _sprite;
        protected Color _spriteColor;
        protected Color _backColor;

        protected string _description;

        protected int _displayPriority;

        //Obstruction flags
        protected bool _obstructsActors;
        protected bool _obstructsVision;
        protected bool _obstructsProjectiles;
        protected bool _obstructsBlasts;
        protected bool _obstructsItems;
        protected bool _obstructsSurfaceEffects;

        public TileBehavior(string objID, int instanceID, IntVector2 affectedTile)
            : base(objID, instanceID)
        {
            AffectedTile = affectedTile;

            _sprite = ' ';
            _spriteColor = null;
            _backColor = null;

            _layer = BehaviorLayer.BASE;

            _displayPriority = _priority = 10000;

            _description = "";

            _obstructsActors = _obstructsVision = _obstructsProjectiles
                = _obstructsBlasts = _obstructsItems = _obstructsSurfaceEffects = false;

            _currentEnergy = 0;
            _energyPerTick = 100;
            _energyToAct = 1000;
            _currentlyTakingTurn = false;
        }

        #region ITakesTurns members
        public int GetPriority()
        {
            return _priority;
        }

        public bool IsReadyForTurn()
        {
            return (_currentEnergy >= _energyToAct);
        }

        public bool IsCurrentlyTakingTurn()
        {
            return _currentlyTakingTurn;
        }

        public virtual void OnTick()
        {
            _currentEnergy += _energyPerTick;
        }

        public virtual TurnResult StartTurn()
        {
            _currentlyTakingTurn = true;
            return TurnResult.GOTO_TAKETURN;
        }

        public virtual TurnResult TakeTurn()
        {
            return TurnResult.GOTO_ENDTURN;
        }

        public virtual TurnResult EndTurn()
        {
            _currentEnergy -= _energyToAct;
            _currentlyTakingTurn = false;
            return TurnResult.TURN_OVER;
        }
        #endregion

        #region Accessors

        public virtual char GetSprite()
        {
            return _sprite;
        }

        public virtual Color GetSpriteColor()
        {
            return _spriteColor;
        }

        public virtual Color GetBackColor()
        {
            return _backColor;
        }
        public virtual String GetDescription() { return _description; }
        public virtual int GetDisplayPriority() { return _displayPriority; }
        public virtual bool GetObstructsActors() { return _obstructsActors; }
        public virtual bool GetObstructsBlasts() { return _obstructsBlasts; }
        public virtual bool GetObstructsProjectiles() { return _obstructsProjectiles; }
        public virtual bool GetObstructsItems() { return _obstructsItems; }
        public virtual bool GetObstructsSurfaceEffects() { return _obstructsSurfaceEffects; }
        public virtual bool GetObstructsVision(bool prevResult) { return prevResult; }

        public virtual BehaviorLayer GetLayer() { return _layer; }

        #endregion


        #region Tile hooks

        public virtual DamageData OnIncomingDamage(DamageData damage)
        {
            return damage;
        }

        #endregion

    }
}
