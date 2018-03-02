using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archmage.Actors;
using Archmage.Engine.Spells;
using Archmage.Engine.Scenes;
using Archmage.Engine.DataStructures;
using Archmage.System;

namespace Archmage.Command
{
    class CommandInterface
    {

        PlayScene _scene;

        public bool TargetingMode { get; protected set; }
        public bool UseItemMode { get; protected set; }
        public bool DropItemMode { get; protected set; }
        public bool InspectItemMode { get; protected set; }
        public bool GameOverMode { get; protected set; }

        TargetingCursor _cursor;

        public CommandInterface(PlayScene scene)
        {
            _scene = scene;
            TargetingMode = false;
            UseItemMode = false;
            DropItemMode = false;
            InspectItemMode = false;
            GameOverMode = false;
            _cursor = new TargetingCursor(new IntVector2(0, 0));
        }

        public void Update(Student s)
        {
            if (GameOverMode)
            {
                if (Input.GetLetterAsIndex() != -1 || Input.LastKeyPressed == "enter")
                    _scene.LoseGame();
            }

            else if (TargetingMode)
            {
                Update_TargetingMode(s);
            }
            else if (UseItemMode)
            {
                Update_UseItemMode(s);
            }
            else if (InspectItemMode)
            {
                Update_InspectItemMode(s);
            }
            else if (DropItemMode)
            {
                Update_DropItemMode(s);
            }
            //End targeting mode logic
            else
            {
                Update_NormalMode(s);
            }
        }

        public void Draw(IntVector2 windowPosition)
        {
            if (TargetingMode)
            {
                _cursor.Draw(windowPosition);
            }
        }

        public void InitTargetingMode(Student s, IntVector2 initPosition)
        {
            TargetingMode = true;
            _cursor.Source = s.Position;
            _cursor.Destination = initPosition;
        }

        public void EndTargetingMode()
        {
            TargetingMode = false;
        }

        public void InitGameOverMode()
        {
            GameOverMode = true;
        }


        #region Helper Methods

        void Update_TargetingMode(Student s)
        {
            _cursor.Source.X = s.Position.X;
            _cursor.Source.Y = s.Position.Y;
            //Get selected spell
            Spell selectedSpell = null;
            foreach (Spell spell in s.Spells)
            {
                if (spell.IsSelected)
                {
                    selectedSpell = spell;
                }
            }
            //Make sure a spell is selected
            if (selectedSpell == null)
            {
                throw new Exception("Targeting is enabled but no spell is selected!");
            }
            int selectedSpellIndex = s.Spells.IndexOf(selectedSpell);
            _cursor.Range = selectedSpell.Range;
            //On cursor movement command
            IntVector2 direction = Input.GetMovementInput();
            if (direction != null)
            {
                _cursor.Destination = _cursor.Destination + direction;
            }

            //Targeting cancel command
            if (Input.LastKeyPressed == "escape")
            {
                TargetingMode = false;
                selectedSpell.IsSelected = false;
                _scene.WriteMessage("Actually, you don't even like that spell.");
            }

            //Fire spell command
            if (Input.LastKeyPressed == "enter" || Input.GetNumberAsIndex() == selectedSpellIndex + 1)
            {
                if (_cursor.InRange)
                    s.OnCastTargetedSpell(selectedSpell, new IntVector2(_cursor.Destination.X, _cursor.Destination.Y));
            }
        }

        void Update_UseItemMode(Student s)
        {
            if (Input.LastKeyPressed == "escape")
            {
                UseItemMode = false;
                _scene.WriteMessage("You decide to save your items for a rainy day.");
            }
            else
            {
                int index = Input.GetLetterAsIndex();
                if (index != -1) //If we got a valid index
                {
                    if (index > s.Items.Count - 1)
                    {
                        _scene.WriteMessage("You don't seem to have that item.");
                    }
                    else
                    {
                        s.OnUseItem(s.Items[index]);
                        UseItemMode = false;
                    }
                }
            }
        }

        void Update_DropItemMode(Student s)
        {
            if (Input.LastKeyPressed == "escape")
            {
                DropItemMode = false;
                _scene.WriteMessage("You decide against parting with any of your precious items.");
            }
        }

        void Update_InspectItemMode(Student s)
        {
            if (Input.LastKeyPressed == "escape")
            {
                InspectItemMode = false;
                _scene.WriteMessage("You stop distracting yourself with your loot.");
            }
        }

        void Update_NormalMode(Student s)
        {
            //On movement command
            IntVector2 direction = Input.GetMovementInput();
            if (direction != null)
            {
                s.OnMoveCommand(direction);
                return;
            }

            //On up/down command
            if (Input.GetLetterInput() == '<')
            {
                if (_scene.GetMap().DoesTileHaveFeature(s.Position, Tiles.Tile_SimpleFeatureType.STAIRS_UP))
                {
                    _scene.UseStairs(s.Position);
                    return;
                }
            }
            if (Input.GetLetterInput() == '>')
            {
                if (_scene.GetMap().DoesTileHaveFeature(s.Position, Tiles.Tile_SimpleFeatureType.STAIRS_DOWN))
                {
                    _scene.UseStairs(s.Position);
                    return;
                }
            }

            //On cast command
            int num = Input.GetNumberAsIndex();
            if (num > -1)
            {
                s.OnSelectSpell(num - 1);
            }

            //On item command
            switch (Input.GetLetterInput())
            {
                case 'u':
                    UseItemMode = true;
                    _scene.WriteMessage("Select an item to use...");
                    break;
                case 'p': //Cheat code
                    //s.AddBehavior(_scene.GetGameObjectPool().CreateActorBehavior("b_invincibility", s.InstanceID));
                    break;
                case 'Q':
                    _scene.LoseGame();
                    break;
                default:
                    break;
            }

            //On spellbook screen command
            if (Input.GetLetterInput() == 's')
            {
                _scene.OpenSpellbookPanel();
            }
        }

        #endregion
    }
}
