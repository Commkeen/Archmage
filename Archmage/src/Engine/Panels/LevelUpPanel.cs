using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Spells;
using Archmage.System;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using libtcod;

/*
namespace Archmage.Panels
{
    class LevelUpPanel : Panel
    {
        PlayScene _scene;
        SpellSelectInterface _spellInterface;

        public LevelUpPanel(PlayScene scene, SpellSelectInterface spellInterface)
        {
            _scene = scene;
            _windowPosition = new IntVector2();
            _windowSize = new IntVector2(Display.CONSOLE_WIDTH, Display.CONSOLE_HEIGHT);

            _spellInterface = spellInterface;
        }

        public override void DrawPanel()
        {
            DrawStringInPanel(25, 5, "Congratulations, you have gained a new spell!");
            DrawStringInPanel(25, 7, "Choose a spell to learn...");

            

            int startX = 10;
            int startY = 10;

            int yOffset = 6;

            int tier = 0;
            List<string> spells = _spellInterface.discipline.GetSpells(tier);
            while (spells.Count > 0)
            {

                for (int i = 0; i < spells.Count; i++)
                {
                    Spell spell = _scene.CreateSpell(spells[i], _scene.GetStudent().InstanceID);
                    
                    bool available = (tier <= _spellInterface.maxTierShown);

                    bool alreadyKnown = false;
                    foreach (Spell s in _scene.GetStudent().Spells)
                    {
                        if (s.ID == spell.ID)
                            alreadyKnown = true;
                    }
                    bool selected = (tier == _spellInterface.selectedTier && i == _spellInterface.selectedSpell);
                    if (selected)
                    {
                        Display.displayConsole.printRect(65, 10, 30, 20, spell.Description);
                    }
                    DrawSpellBox(spell, startX + i*20, startY + tier*yOffset, available, alreadyKnown, selected);
                }
                tier++;
                spells = _spellInterface.discipline.GetSpells(tier);
            }
        }

        private void DrawSpellBox(Spell spell, int x, int y, bool available, bool alreadyKnown, bool selected)
        {
            int spellBoxWidth = 19;
            int xOffset = x;
            int yOffset = y;

            //Line for spell cost
            string costLine = "";
            for (int i = 0; i < spell.AttentionCost; i++)
                costLine = costLine + "*";
            while (costLine.Length + 2 < spellBoxWidth - 10)
                costLine = costLine + " ";
            for (int i = 0; i < spell.SoulCost; i++)
                costLine = costLine + "$";

            //Draw panel
            TCODConsole.root.setForegroundColor(TCODColor.white);
            if (selected)
            {
                TCODConsole.root.setForegroundColor(TCODColor.yellow);
            }
            else if (alreadyKnown)
            {
                TCODConsole.root.setForegroundColor(TCODColor.blue);
            }
            DrawStringInPanel(xOffset, yOffset, "-------------------");
            DrawStringInPanel(xOffset, yOffset + 1, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 2, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 3, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 4, "-------------------");

            if (available)
            {
                //Draw Text
                TCODConsole.root.setForegroundColor(TCODColor.white);
                DrawStringInPanel(xOffset + 2, yOffset + 1, "           " + spell.CooldownTime + " CD");
                DrawStringInPanel(xOffset + 2, yOffset + 2, spell.Name);
                DrawStringInPanel(xOffset + 2, yOffset + 3, costLine);
            }
            else
            {
                TCODConsole.root.setForegroundColor(TCODColor.white);
                DrawStringInPanel(xOffset + 4, yOffset + 2, "?????");
            }

        }

    }
}
*/