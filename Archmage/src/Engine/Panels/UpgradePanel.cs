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

namespace Archmage.Panels
{
    class UpgradePanel : Panel
    {
        PlayScene _scene;
        UpgradeInterface _spellInterface;

        public UpgradePanel(PlayScene scene, UpgradeInterface spellInterface)
        {
            _scene = scene;
            _windowPosition = new IntVector2();
            _windowSize = new IntVector2(Display.CONSOLE_WIDTH, Display.CONSOLE_HEIGHT);

            _spellInterface = spellInterface;
        }

        public override void DrawPanel()
        {
            DrawStringInPanel(25, 5, "Choose a spell to upgrade");
            DrawStringInPanel(25, 6, "Or press Escape when finished.");
            DrawStringInPanel(25, 8, "Essence: " + _scene.GetStudent().CurrentEssence);



            int startX = 10;
            int startY = 10;

            int yOffset = 6;

            List<Spell> playerSpells = _scene.GetStudent().Spells;
            DrawSpellList(playerSpells, _spellInterface.selectedSpell);
            DrawUpgradeList(playerSpells[_spellInterface.selectedSpell]);

            /*
            for (int i = 0; i < playerSpells.Count; i++)
            {
                DrawSpellBox(playerSpells[i], startX, startY + i * 6 + yOffset);
                DrawUpgradeBox(playerSpells[i], startX + 25, startY + i * 6 + yOffset);

            }
             * */
        }

        private void DrawSpellList(List<Spell> spells, int selectedIndex)
        {
            int startX = 2;
            int startY = 18;
            int yOffset = 2;

            for (int i = 0; i < spells.Count; i++)
            {
                if (selectedIndex == i)
                {
                    TCODConsole.root.setForegroundColor(TCODColor.yellow);
                }
                else
                {
                    TCODConsole.root.setForegroundColor(TCODColor.white);
                }
                DrawStringInPanel(new IntVector2(startX, startY + yOffset * i), spells[i].Name);
            }
        }

        private void DrawUpgradeList(Spell spell)
        {
            int startX = 20;
            int startY = 18;
            int yOffset = 2;

            for (int i = 0; i < spell.MaxUpgradeLevel; i++)
            {
                if (spell.UpgradeLevel > i - 1)
                {
                    TCODConsole.root.setForegroundColor(TCODColor.blue);
                }
                else if (spell.UpgradeLevel == i - 1)
                {
                    TCODConsole.root.setForegroundColor(TCODColor.yellow);
                }
                else
                {
                    TCODConsole.root.setForegroundColor(TCODColor.grey);
                }
                DrawStringInPanel(new IntVector2(startX, startY + yOffset * i), spell.UpgradeDescription(i+1));
            }
        }


        //not used
        private void DrawSpellBox(Spell spell, int x, int y)
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
            DrawStringInPanel(xOffset, yOffset, "-------------------");
            DrawStringInPanel(xOffset, yOffset + 1, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 2, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 3, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 4, "-------------------");

            //Draw Text
            TCODConsole.root.setForegroundColor(TCODColor.white);
            DrawStringInPanel(xOffset + 2, yOffset + 1, "           " + spell.CooldownTime + " CD");
            DrawStringInPanel(xOffset + 2, yOffset + 2, spell.Name);
            DrawStringInPanel(xOffset + 2, yOffset + 3, costLine);

        }

        //not used
        private void DrawUpgradeBox(Spell spell, int x, int y)
        {
            int spellBoxWidth = 29;
            int xOffset = x;
            int yOffset = y;

            //Draw panel
            TCODConsole.root.setForegroundColor(TCODColor.white);
            DrawStringInPanel(xOffset, yOffset, "-----------------------------");
            DrawStringInPanel(xOffset, yOffset + 1, "|                           |");
            DrawStringInPanel(xOffset, yOffset + 2, "|                           |");
            DrawStringInPanel(xOffset, yOffset + 3, "|                           |");
            DrawStringInPanel(xOffset, yOffset + 4, "-----------------------------");

            //Draw Text
            TCODConsole.root.setForegroundColor(TCODColor.white);
            //DrawStringInPanel(xOffset + 2, yOffset + 1, spell.NextLevelCost().ToString());
            //DrawStringInPanel(xOffset + 2, yOffset + 2, spell.NextLevelDescription());
        }

    }
}
