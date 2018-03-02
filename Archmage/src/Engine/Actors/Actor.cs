using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;

namespace Archmage.Actors
{
    class Actor:GameObject, ITakesTurns
    {
        //Energy determines if you can act this turn
        protected int _currentEnergy;
        protected int _energyPerTick; //How much energy this actor gets every tick
        protected int _energyToAct; //How much energy you need to have in order to act
        protected bool _currentlyTakingTurn; //Whether you're taking a turn right now
        protected int _priority; //When multiple actors have a turn this tick, the higher priority number goes first.

        public int MovementEnergyCost { get; protected set; } //The energy cost to move/wait a turn

        public IntVector2 Position { get; protected set; }

        public string Name { get; protected set; }

        public char Sprite { get; protected set; }
        protected Color SpriteColor { get; set; }

        public int CurrentHealth { get; protected set; }
        public int MaxHealth { get; protected set; }

        protected int ArmorPoints { get; set; }

        protected int EvasionPoints { get; set; }

        protected int DisplacementPoints { get; set; }

        public int SightRange { get; protected set; }

        public int Accuracy { get; protected set; }

        public int XPValue { get; protected set; }

        public bool Passable { get; protected set; }

        protected List<int> _actorBehaviors;

        protected List<string> _attributes;

        #region Calculated Properties

        public int EvasionChance
        {
            get
            {
                return ((GetEvasionPoints() + GetDisplacementPoints()) * 25);
                if (GetEvasionPoints() + GetDisplacementPoints() > 0)
                    return 25;
                return 0;
            }
        }

        public int DisplacementChance
        {
            get
            {
                if (GetDisplacementPoints() > 0)
                    return 25;
                return 0;
            }
        }

        #endregion

        public Actor(string objID, int instanceID):base(objID, instanceID)
        {
            Position = new IntVector2();

            _actorBehaviors = new List<int>();
            _attributes = new List<string>();

            Name = "DEFAULT";

            Sprite = '?';
            SpriteColor = new Color();

            XPValue = 0;

            Accuracy = 100;

            _energyPerTick = 100;
            _currentEnergy = 0;
            _energyToAct = 1000;

            MovementEnergyCost = 100;

            SightRange = 10;

            Passable = false;
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

            bool ignoreTurn = false;
            bool endTurn = false;

            List<int> behaviorsCopy = new List<int>(_actorBehaviors);
            foreach (int b in behaviorsCopy)
            {
                TurnResult result = _scene.GetGameObjectPool().GetActorBehavior(b).OnActorStartTurn();
                ignoreTurn |= result==TurnResult.IGNORE_TURN;
                endTurn |= result==TurnResult.GOTO_ENDTURN;
            }

            if (ignoreTurn)
                return TurnResult.IGNORE_TURN;
            if (endTurn)
                return TurnResult.GOTO_ENDTURN;

            return TurnResult.GOTO_TAKETURN;
        }

        public virtual TurnResult TakeTurn()
        {
            return TurnResult.GOTO_ENDTURN;
        }

        public virtual TurnResult EndTurn()
        {
            _currentlyTakingTurn = false;
            return TurnResult.TURN_OVER;
        }

        #endregion


        #region Actions

        public void WarpToPosition(IntVector2 position)
        {
            this.Position = position;
        }

        public bool RollForEvasion(bool displacementOnly)
        {
            bool result = false;
            Random rand = new Random();
            int evadeRoll = rand.Next(100);

            if (displacementOnly)
            {
                if (evadeRoll < DisplacementChance)
                    result = true;
            }
            else
            {
                if (evadeRoll < EvasionChance)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Calculate how an attack interacts with this entity's shields.
        /// Does not change any state, just returns a result!
        /// </summary>
        /// <param name="atk"></param>
        /// <returns></returns>
        public AttackResult.ShieldCheckResult CalculateAttackShieldPenetration(AttackData atk)
        {
            Random rand = new Random();

            //Check against shields
            AttackResult.ShieldCheckResult shieldResult = AttackResult.ShieldCheckResult.SHIELD_BYPASSED;
            if (GetShieldLayers() > 0)
            {
                if (atk.Type == AttackType.ARCANE
                    || atk.Type == AttackType.BLAST
                    || atk.Type == AttackType.ENERGY)
                {
                    int shieldRoll = rand.Next(100);
                    if (shieldRoll < GetShieldStrength())
                    {
                        shieldResult = AttackResult.ShieldCheckResult.SHIELD_PROTECTED;
                    }
                    else if (GetShieldLayers() > 1)
                    {
                        shieldResult = AttackResult.ShieldCheckResult.SHIELD_LAYERBROKE;
                    }
                    else
                    {
                        shieldResult = AttackResult.ShieldCheckResult.SHIELD_SHIELDBROKE;
                    }
                }
                else if (atk.Type == AttackType.DARK)
                {
                    shieldResult = AttackResult.ShieldCheckResult.SHIELD_PROTECTED;
                }
            }


            return shieldResult;
        }

        /// <summary>
        /// Call this to damage an actor.
        /// Armor and resistances are applied here.
        /// If the actor is killed by the damage, that happens here.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(DamageData damage)
        {
            //_scene.WriteMessage(_scene.GetAttackDescription(damage));

            //Check armor
            if (damage.DmgAttackType == AttackType.PHYSICAL)
            {
                Random r = new Random();
                for (int i = 0; i < ArmorPoints; i++)
                {
                    if (r.Next(100) < 80)
                    {
                        damage.Magnitude--;
                        if (damage.Magnitude < 0)
                            damage.Magnitude = 0;
                    }
                }
            }

            //TODO: Check resistances

            DamageData adjustedDamage = OnIncomingDamage(damage);
            CurrentHealth = CurrentHealth - adjustedDamage.Magnitude;

            //Write the effect of the attack
            if (ObjectID != "playerStudent")
            {
                if (adjustedDamage.Magnitude > 0)
                {
                    //_scene.WriteMessage("The " + Name + " takes " + adjustedDamage.Magnitude + " damage!");
                }
                else
                {
                    _scene.WriteMessage("The " + Name + " takes no damage!");
                }
            }

            if (CurrentHealth <= 0)
            {
                OnZeroHP();
            }
        }

        /// <summary>
        /// Breaks the actor's outermost shield layer.  If it has no shields nothing happens.
        /// If this is the last shield layer of the current protective spell, that spell is deactivated here.
        /// </summary>
        public void BreakShieldLayer()
        {
            OnShieldLayerBroken();
        }

        /// <summary>
        /// Take damage to the outermost shield.  This does not calculate shield shatter chance, that happens elsewhere!
        /// If this actor has no shields, nothing happens.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeShieldDamage(int damage)
        {
            OnShieldLayerAbsorbsDamage(damage);
        }

        public void HealDamage(int heal)
        {
            CurrentHealth = CurrentHealth + heal;
            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;
        }

        public void SpendEnergy(int energy)
        {
            _currentEnergy -= energy;
        }

        #endregion

        #region Behavior Management

        public void AddBehavior(int id)
        {
            _actorBehaviors.Add(id);
        }

        public void RemoveBehavior(int id)
        {
            //TODO: Maybe delay removing behavior from list, in case behavior is removed during iteration?
            //TODO: Maybe instead of having this method, just autoremove dead behaviors?
            _actorBehaviors.Remove(id);
        }

        public List<int> GetBehaviors()
        {
            return new List<int>(_actorBehaviors);
        }

        #endregion

        #region Behavior hooks

        public virtual DamageData OnSendDamage(DamageData damage)
        {
            DamageData adjustedDamage = damage;
            List<int> behaviorsCopy = new List<int>(_actorBehaviors);
            foreach (int id in behaviorsCopy)
            {
                adjustedDamage = _scene.GetGameObjectPool().GetActorBehavior(id).OnSendDamage(adjustedDamage);
            }
            return adjustedDamage;
        }

        /// <summary>
        /// Cycles thru all behaviors on this actor and mitigates damage accordingly.
        /// Returns the adjusted damage totals.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        protected virtual DamageData OnIncomingDamage(DamageData damage)
        {
            DamageData adjustedDamage = damage;
            List<int> behaviorsCopy = new List<int>(_actorBehaviors);
            foreach (int id in behaviorsCopy)
            {
                adjustedDamage = _scene.GetGameObjectPool().GetActorBehavior(id).OnIncomingDamage(adjustedDamage);
            }
            return adjustedDamage;
        }

        public virtual void OnShieldLayerAbsorbsDamage(int damage)
        {
            List<int> behaviorsCopy = new List<int>(_actorBehaviors);
            foreach (int id in behaviorsCopy)
            {
                _scene.GetGameObjectPool().GetActorBehavior(id).OnShieldLayerAbsorbsDamage(damage);
            }
        }

        public virtual void OnShieldLayerBroken()
        {
            List<int> behaviorsCopy = new List<int>(_actorBehaviors);
            foreach (int id in behaviorsCopy)
            {
                _scene.GetGameObjectPool().GetActorBehavior(id).OnShieldLayerBroken();
            }
        }

        protected virtual void OnArmorAbsorbsDamage()
        {
            //TODO
        }

        protected virtual void OnZeroHP()
        {
            List<int> behaviorsCopy = new List<int>(_actorBehaviors);
            foreach (int id in behaviorsCopy)
            {
                _scene.GetGameObjectPool().GetActorBehavior(id).OnZeroHP();
            }
            Alive = false;

            //TODO: Drop essence and other items
        }

        #endregion

        #region Data Accessors modified by behaviors

        public Color GetSpriteColor()
        {
            Color result = SpriteColor;
            foreach (int id in _actorBehaviors)
            {
                result = _scene.GetGameObjectPool().GetActorBehavior(id).GetColor(result);
            }
            return result;
        }

        public int GetShieldLayers()
        {
            int result = 0;
            foreach (int id in _actorBehaviors)
            {
                result += _scene.GetGameObjectPool().GetActorBehavior(id).ShieldLayers;
            }
            return result;
        }

        public int GetShieldStrength()
        {
            int result = 0;
            foreach (int id in _actorBehaviors)
            {
                result += _scene.GetGameObjectPool().GetActorBehavior(id).ShieldStrength;
            }
            return result;
        }

        public int GetEvasionPoints()
        {
            int result = EvasionPoints;
            foreach (int id in _actorBehaviors)
            {
                result += _scene.GetGameObjectPool().GetActorBehavior(id).EvasionBonus;
            }
            return result;
        }

        public int GetDisplacementPoints()
        {
            int result = DisplacementPoints;
            foreach (int id in _actorBehaviors)
            {
                result += _scene.GetGameObjectPool().GetActorBehavior(id).DisplacementBonus;
            }
            return result;
        }

        public int GetArmorPoints()
        {
            int result = ArmorPoints;
            foreach (int id in _actorBehaviors)
            {
                result += _scene.GetGameObjectPool().GetActorBehavior(id).ArmorBonus;
            }
            return result;
        }

        #endregion

        #region Attribute management

        public void AddAttribute(string attribute)
        {
            if (!_attributes.Contains(attribute))
                _attributes.Add(attribute);
        }

        public bool HasAttribute(string attribute)
        {
            List<string> AllAttributes = new List<string>(_attributes);
            foreach (int id in _actorBehaviors)
            {
                AllAttributes.AddRange(_scene.GetGameObjectPool().GetActorBehavior(id).ActorAttributes);
            }

            return AllAttributes.Contains(attribute);
        }

        public void RemoveAttribute(string attribute)
        {
            _attributes.Remove(attribute);
        }

        

        #endregion
    }
}
