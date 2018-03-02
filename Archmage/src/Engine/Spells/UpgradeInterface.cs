using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.Scenes;

namespace Archmage.Engine.Spells
{
    class UpgradeInterface
    {
        PlayScene _scene;

        public int selectedSpell;

        public UpgradeInterface(PlayScene scene)
        {
            _scene = scene;

            selectedSpell = 0;

            //maxTierShown = _scene.GetStudent().SpellSchoolLevels[0];
        }

        public void Update()
        {
            Spell spell = _scene.GetStudent().Spells[selectedSpell];
            if (Input.LastKeyPressed == "escape")
            {
                //TODO: Close this screen
                _scene.CloseUpgradePanel();
            }
            else if (Input.LastKeyPressed == "south")
            {
                selectedSpell++;
                if (selectedSpell >= _scene.GetStudent().Spells.Count)
                    selectedSpell = 0;
            }
            else if (Input.LastKeyPressed == "north")
            {
                selectedSpell--;
                if (selectedSpell < 0)
                    selectedSpell = _scene.GetStudent().Spells.Count - 1;
            }

            else if (Input.LastKeyPressed == "enter")
            {
                if (spell.UpgradeLevel < spell.MaxUpgradeLevel)
                {
                    if (_scene.GetStudent().CurrentEssence >= spell.UpgradeCost(spell.UpgradeLevel+1))
                    {
                        _scene.GetStudent().CurrentEssence -= spell.UpgradeCost(spell.UpgradeLevel + 1);
                         spell.GainLevel();
                    }
                }
            }
        }



    }
}
