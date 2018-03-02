using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.Scenes;
using Archmage.Content.Items;
using Archmage.Engine.DataStructures;

namespace Archmage.Engine.Spells
{
    class SpellbookInterface
    {
        PlayScene _scene;

        public Item_Spellbook spellbook;
        public Spell spellbookSpell;
        public List<Spell> spellList;

        public int slotSelected;
        public bool displayConfirmation;

        public string instructionsText;
        public string descriptionText;

        public bool readSpellbookMode;

        public SpellbookInterface(PlayScene scene)
        {
            _scene = scene;

            slotSelected = 1;

            displayConfirmation = false;

            spellList = _scene.GetStudent().Spells;

            descriptionText = "";
            instructionsText = "";

            readSpellbookMode = false;
        }

        public void ReadSpellbook(Item_Spellbook book)
        {
            readSpellbookMode = true;
            spellbook = book;
            spellbookSpell = _scene.CreateSpell(book.spellID, _scene.GetStudent().InstanceID);
        }

        public void Update()
        {
            int numOfSpellSlots = _scene.GetStudent().NumOfSpellSlots;
            if (slotSelected > numOfSpellSlots)
            {
                slotSelected = numOfSpellSlots;
            }
            if (!readSpellbookMode && slotSelected <= 0)
            {
                slotSelected = 1;
            }
            if (slotSelected > 0 && slotSelected <= spellList.Count)
            {
                descriptionText = spellList[slotSelected-1].Description;
            }
            else if (readSpellbookMode)
            {
                descriptionText = spellbookSpell.Description;
            }
            else
            {
                descriptionText = "";
            }

            instructionsText = "Select a spell with the number keys.  Esc to exit.";

            if (readSpellbookMode)
            {
                instructionsText = "Select a slot for your new spell.  Esc to cancel.";
            }

            if (readSpellbookMode && displayConfirmation)
            {
                instructionsText = "Are you sure you want to put your new spell in that slot? Y/N";
            }

            if (Input.LastKeyPressed == "escape")
            {
                if (displayConfirmation)
                    displayConfirmation = false;
                else
                    _scene.CloseSpellbookPanel();
            }
            if (Input.LastKeyPressed == "enter")
            {
                if (readSpellbookMode && slotSelected != 0)
                    displayConfirmation = true;
            }
            if (Input.LastTCODKeyPressed.Character == 'n')
            {
                displayConfirmation = false;
            }
            if (Input.LastTCODKeyPressed.Character == 'y')
            {
                if (displayConfirmation && readSpellbookMode)
                {
                    //Learn the spell and get rid of whatever spell was in that slot
                    _scene.GetStudent().OnLearnSpellFromSpellbook(spellbook, slotSelected);
                    _scene.CloseSpellbookPanel();
                }
            }

            int numInput = Input.GetNumberAsIndex();
            if (!displayConfirmation && numInput > 0 && numInput <= _scene.GetStudent().NumOfSpellSlots)
            {
                slotSelected = numInput;
            }

            if (!displayConfirmation)
            {
                IntVector2 movementInput = Input.GetMovementInput();
                if (movementInput != null)
                {
                    slotSelected += movementInput.Y;
                    if (slotSelected > numOfSpellSlots)
                        slotSelected = numOfSpellSlots;
                    if (slotSelected < 0)
                        slotSelected = 0;

                    if (movementInput.X != 0)
                    {
                        if (slotSelected == 0)
                            slotSelected = 1;
                        else if (readSpellbookMode)
                            slotSelected = 0;
                    }
                }
            }
        }



    }
}
