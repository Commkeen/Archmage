using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Engine.Spells;

namespace Archmage.Actors
{
    class Monster:Actor
    {
        public IntVector2 LastKnownPlayerPosition { get; protected set; }

        public List<MonsterAbility> Abilities { get; protected set; }

        public int AttentionUsed
        {
            get
            {
                int att = 0;
                foreach (MonsterAbility a in Abilities)
                {
                    if (a.IsActive)
                        att += a.AttentionCost;
                }
                return att;
            }
        }
        public int MaxAttention { get; protected set; }

        public bool CanMove { get; protected set; }

        public Monster(string objectID, int instanceID)
            : base(objectID, instanceID)
        {
            LastKnownPlayerPosition = null;
            Abilities = new List<MonsterAbility>();

            MaxAttention = 1;
            CanMove = true;
        }

        public override TurnResult StartTurn()
        {
            foreach (MonsterAbility m in Abilities)
            {
                m.CooldownTick();
            }

            TurnResult result = base.StartTurn();

            if (result == TurnResult.IGNORE_TURN || result == TurnResult.GOTO_ENDTURN)
            {
                _currentlyTakingTurn = false;
                _currentEnergy -= MovementEnergyCost;
                return result;
            }

            //If we can't see the player and don't know where the player might be, sleep
            if (LastKnownPlayerPosition == null
                || LastKnownPlayerPosition.Equals(Position))
            {
                if (!_scene.IsTileVisibleFromLocation(Position, _scene.GetStudent().Position, SightRange))
                {
                    _currentlyTakingTurn = false;
                    _currentEnergy -= MovementEnergyCost;
                    return TurnResult.IGNORE_TURN;
                }
            }
                 

            return TurnResult.GOTO_TAKETURN;
        }

        public override TurnResult TakeTurn()
        {
            Student student = _scene.GetStudent();

            int distanceToStudent = (Position - student.Position).Magnitude();

            bool actionTaken = false;
            //If I can't see the player but I have a saved position, move towards him
            if (!_scene.IsTileVisibleFromLocation(Position, student.Position, SightRange))
            {
                if (LastKnownPlayerPosition != null)
                {
                    List<IntVector2> pathToPlayer = _scene.GetBestRouteBetweenPositions(Position, LastKnownPlayerPosition);
                    if (pathToPlayer.Count > 1 && _scene.IsPositionFree(pathToPlayer[1]))
                    {
                        WarpToPosition(pathToPlayer[1]);
                    }
                }
                _currentEnergy -= MovementEnergyCost;
                actionTaken = true;
            }
            else
            {
                //We can see the player, so save his position
                if (LastKnownPlayerPosition == null) //If the monster just saw the player this turn
                    OnNoticePlayer(); //Maybe have the monster growl or something, just for flavor
                LastKnownPlayerPosition = new IntVector2(student.Position);
            }

            //If I have a "priority" spell, cast it
            if (!actionTaken)
            {
                List<MonsterAbility> abils = ValidAbilities(student.Position);
                foreach (MonsterAbility a in abils)
                {
                    if (a.Priority)
                    {
                        if (a.Cast(student.Position))
                        {
                            _currentEnergy -= a.EnergyCost;
                            actionTaken = true;
                            break;
                        }
                    }
                }
            }

            //Get all attacks available from current range, roll to see if we cast one
            if (!actionTaken)
            {
                int roll = new Random().Next(100);
                List<MonsterAbility> abils = ValidAbilities(student.Position);
                foreach (MonsterAbility a in abils)
                {
                    if (a.UseChance > roll)
                    {
                        if (a.Cast(student.Position))
                        {
                            _currentEnergy -= a.EnergyCost;
                            actionTaken = true;
                            break;
                        }
                    }
                    else
                    {
                        roll -= a.UseChance;
                    }
                }
            }

            //Move towards player
            if (!actionTaken)
            {
                List<IntVector2> pathToPlayer = _scene.GetBestRouteBetweenPositions(Position, LastKnownPlayerPosition);
                if (pathToPlayer.Count > 1 && _scene.IsPositionFree(pathToPlayer[1]) && CanMove)
                {
                    WarpToPosition(pathToPlayer[1]);
                }

                _currentEnergy -= MovementEnergyCost;
                actionTaken = true;
            }

            return TurnResult.GOTO_ENDTURN;
        }

        public virtual void OnNoticePlayer()
        {
            return;
        }

        private List<MonsterAbility> ValidAbilities(IntVector2 tgtPos)
        {
            List<MonsterAbility> abils = new List<MonsterAbility>();
            foreach (MonsterAbility a in Abilities)
            {
                if (CanCastAbility(a, tgtPos))
                {
                    abils.Add(a);
                }
            }
            return abils;
        }

        private bool CanCastAbility(MonsterAbility ability, IntVector2 tgtPos)
        {
            bool canCast = true;

            if (ability.IsActive)
                canCast = false;

            if (ability.CurrentCooldown > 0)
                canCast = false;

            if ((ability.Range != 0 && !_scene.IsTileShootableFromLocation(Position, tgtPos, ability.Range))
                || (ability.Range == 1 && (Math.Abs(tgtPos.X - Position.X)>1 || (Math.Abs(tgtPos.Y - Position.Y)>1)))) //Melee range
                canCast = false;

            if (ability.AttentionCost != 0 && AttentionUsed + ability.AttentionCost > MaxAttention)
                canCast = false;

            return canCast;
        }

        protected override void OnZeroHP()
        {
            _scene.WriteMessage("You kill the " + Name + "!");
            base.OnZeroHP();
        }
    }

    class MonsterAbility:Ability
    {
        public bool Priority { get; set; } //The monster will always cast this if able
        public int UseChance { get; set; } //Chance the monster will use this if able, out of 100.

        public MonsterAbility(int ownerID):base(ownerID)
        {
            Priority = false;
            UseChance = 100;
        }
    }
}
