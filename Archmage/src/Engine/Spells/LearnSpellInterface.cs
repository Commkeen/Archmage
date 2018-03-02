using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.Scenes;
using Archmage.Content.Items;

namespace Archmage.Engine.Spells
{
    class LearnSpellInterface
    {
        PlayScene _scene;

        public Item_Spellbook spellbook;
        public Spell spell;

        public int slotSelected;
        public bool displayConfirmation;

        public LearnSpellInterface(PlayScene scene, Item_Spellbook s)
        {
            _scene = scene;

            spellbook = s;
            spell = _scene.CreateSpell(spellbook.spellID, _scene.GetStudent().InstanceID);

            slotSelected = -1;

            displayConfirmation = false;

            //maxTierShown = _scene.GetStudent().SpellSchoolLevels[0];
        }

        public void Update()
        {
            if (spell == null)
                return;

            if (Input.LastKeyPressed == "escape")
            {
                if (displayConfirmation)
                    displayConfirmation = false;
                else
                    _scene.CloseLearnSpellPanel();
            }
            if (Input.LastTCODKeyPressed.Character == 'n')
            {
                displayConfirmation = false;
            }
            if (Input.LastTCODKeyPressed.Character == 'y')
            {
                if (displayConfirmation)
                {
                    //Learn the spell and get rid of whatever spell was in that slot
                    _scene.GetStudent().OnLearnSpellFromSpellbook(spellbook, slotSelected);
                    _scene.CloseLearnSpellPanel();
                }
            }

            int numInput = Input.GetNumberAsIndex();
            if (!displayConfirmation && numInput > 0 && numInput <= _scene.GetStudent().NumOfSpellSlots)
            {
                displayConfirmation = true;
                slotSelected = numInput;
            }
        }



    }
}
