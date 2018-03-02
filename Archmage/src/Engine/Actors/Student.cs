using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Engine.Spells;
using Archmage.Engine.Items;
using Archmage.Command;
using Archmage.Behaviors;
using Archmage.Content.Behaviors.TileBehaviors;
using Archmage.Content.Items;
using Archmage.Tiles;

namespace Archmage.Actors
{
    class Student:Actor
    {

        //Active spells take up attention.
        //The student cannot use more than his max attention.
        public int AttentionUsed
        {
            get
            {
                int att = 0;
                foreach (Spell s in Spells)
                {
                    if (s != null && s.IsActive)
                        att += s.AttentionCost;
                }
                return att;
            }
        }
        public int MaxAttention { get; protected set; }
        public int CurrentEssence { get; set; }

        public int NumOfSpellSlots { get; protected set; }
        public List<Spell> Spells { get; protected set; }
        public List<Item> Items { get; protected set; }

        public int Turns { get; protected set; } //How many turns the game has run

        public CommandInterface Command { get; protected set; }
        bool _turnOver; //Internal flag to decide when the Student is done with his turn.

        #region Energy costs of various actions

        const int moveCost = 100;

        #endregion

        public Student(int instanceID):base("playerStudent", instanceID)
        {
            Command = new CommandInterface(_scene);

            NumOfSpellSlots = 6;
            Spells = new List<Spell>();
            Items = new List<Item>();

            //EvasionPoints = 5;

            Sprite = '@';
            SpriteColor = new Color(libtcod.TCODColor.white);

            Name = "Wanda Weatherby";
            MaxHealth = 150;
            CurrentHealth = MaxHealth;
            MaxAttention = 3;
            CurrentEssence = 50;
        }

        #region State modifiers

        public void AddSpell(Spell s)
        {
            //TODO: Check that we don't have too many
            Spells.Add(s);
        }

        public void RemoveSpell(Spell s)
        {
            //TODO: Cleanup this spell and any current Behaviors related to it
            Spells.Remove(s);
        }

        public bool AddItem(Item i)
        {
            if (Items.Count < 26)
            {
                Items.Add(i);
                return true;
            }
            return false;
        }

        public void RemoveItem(Item i)
        {
            Items.Remove(i);
            //TODO: should we kill the item too
        }

        #endregion

        #region Turn methods

        public override TurnResult StartTurn()
        {
            TurnResult result = base.StartTurn();

            foreach (Spell s in Spells)
            {
                if (s != null)
                    s.CooldownTick();
            }

            Turns++;

            //TODO: This is a hack and needs to be fixed asap
            if (result == TurnResult.GOTO_ENDTURN)
                _currentEnergy -= moveCost;

            return result;
        }

        public override TurnResult TakeTurn()
        {
            base.TakeTurn();

            _turnOver = false;

            Command.Update(this);

            if (_turnOver)
                return TurnResult.GOTO_ENDTURN;
            else
                return TurnResult.GOTO_TAKETURN;

        }

        #endregion

        #region Actions

        public void OnMoveCommand(IntVector2 deltaPosition)
        {
            if (_scene.IsPositionFree(Position + deltaPosition))
            {
                Position += deltaPosition;

                //Pickup items on the tile
                List<int> items = _scene.GetItemTokensAtPosition(Position);
                foreach (int i in items)
                {
                    ItemToken token = _scene.GetGameObjectPool().GetItemToken(i);
                    if (token != null)
                    {
                        Item item = _scene.GetGameObjectPool().GetItem(token.ItemID);
                        if (item != null)
                        {
                            if (item.ObjectID == "magicCrystal") //Use crystals on pickup
                            {
                                item.OnUse(this);
                            }
                            else
                            {
                                AddItem(item);
                            }
                            token.Alive = false;
                        }
                    }
                }

                _currentEnergy -= moveCost;
                _turnOver = true;
            }
            else if (_scene.GetMap().DoesTileHaveFeature(Position + deltaPosition, Tile_SimpleFeatureType.DOOR))
            {
                OnOpenDoor(Position + deltaPosition);
            }
            else
            {
                _scene.WriteMessage("Something is in your way!");
            }
        }

        public void OnSelectSpell(int spellNum)
        {
            //Make sure selection is a valid spell
            if (spellNum < 0 || spellNum > Spells.Count - 1)
            {
                return;
            }

            //Make sure no spells are selected
            foreach (Spell s in Spells)
            {
                if (s.IsSelected)
                {
                    throw new Exception("A spell is already selected!");
                }
            }

            Spell chosenSpell = Spells[spellNum];
            //If the chosen spell is active, dispel it
            if (chosenSpell.IsActive)
            {
                chosenSpell.Deactivate();
            }
            else
            {
                //_scene.WriteMessage("Now casting " + chosenSpell.Name);
                //Make sure resources are available
                bool canCastSpell = true;
                if (chosenSpell.AttentionCost > MaxAttention - AttentionUsed)
                {
                    _scene.WriteMessage("You don't have the attention to cast that!");
                    canCastSpell = false;
                }
                if (chosenSpell.SoulCost > CurrentEssence)
                {
                    _scene.WriteMessage("You don't have enough essence to cast that!");
                    canCastSpell = false;
                }
                if (chosenSpell.CurrentCooldown > 0)
                {
                    _scene.WriteMessage("That spell is still on cooldown!");
                    canCastSpell = false;
                }

                if (!canCastSpell)
                {
                    return;
                }

                //Looks like we can cast this spell
                if (chosenSpell.TargetingType == Spell.SpellTargetingType.NONE)
                {
                    //Cast the spell immediately
                    _turnOver = chosenSpell.Cast();
                    if (_turnOver)
                    {
                        CurrentEssence -= chosenSpell.SoulCost;
                    }
                }
                else
                {
                    //Go into targeting mode
                    chosenSpell.IsSelected = true;
                    Command.InitTargetingMode(this, new IntVector2(this.Position));
                }
            }



        }

        public void OnCastTargetedSpell(Spell s, IntVector2 target)
        {
            //TODO
            _turnOver = s.CastAtTarget(target);
            if (_turnOver)
            {
                //_currentEnergy -= moveCost; //TODO: replace with spell use cost
                CurrentEssence -= s.SoulCost;
            }
            s.IsSelected = false;
            Command.EndTargetingMode();
        }

        public void OnReadSpellbook(Item_Spellbook book)
        {
            _scene.OpenSpellbookPanel(book);
        }

        public void OnLearnSpellFromSpellbook(Item_Spellbook book, int slotSelected)
        {
            if (slotSelected > NumOfSpellSlots)
                slotSelected = NumOfSpellSlots;

            if (Spells.Count >= slotSelected)
            {
                OnForgetSpell(Spells[slotSelected - 1]);
                Spells[slotSelected - 1] = _scene.CreateSpell(book.spellID, InstanceID);
            }
            else
            {
                slotSelected = Spells.Count + 1;
                Spells.Add(_scene.CreateSpell(book.spellID, InstanceID));
            }

            RemoveItem(book);
        }

        public void OnForgetSpell(Spell s)
        {
            //Make sure spell is disabled
            s.Deactivate();
            Spells[Spells.IndexOf(s)] = null;
        }

        public void OnUseItem(Item i)
        {
            //TODO
            if (i.OnUse(this))
            {
                _turnOver = true;
                _currentEnergy -= moveCost; //TODO: replace with item use cost
            }
            
        }

        public void OnOpenDoor(IntVector2 doorPosition)
        {
            //TODO
            List<int> behaviorsAtPosition = _scene.GetMap().GetTileBehaviors(doorPosition);
            foreach (int id in behaviorsAtPosition)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                if (b is TileBehavior_Door)
                {
                    if (!(b as TileBehavior_Door).IsOpen)
                    {
                        (b as TileBehavior_Door).OpenDoor();
                    }
                }
            }
        }

        public void OnCloseDoor(IntVector2 doorPosition)
        {
            //TODO
        }

        protected override void OnZeroHP()
        {
            //base.OnZeroHP();
            _scene.LoseGame();
        }

        #endregion
    }
}
