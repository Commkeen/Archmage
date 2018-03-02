using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Spells;
using Archmage.Engine.DataStructures;
using libtcod;

namespace Archmage.Panels
{
    class SpellHotbarPanel : Panel
    {
        List<Spell> _spellList;

        public SpellHotbarPanel(List<Spell> spells)
        {
            _windowPosition = new IntVector2();
            _windowSize = new IntVector2();

            _windowPosition.X = 0;
            _windowPosition.Y = 0;
            _windowSize.X = 20;
            _windowSize.Y = 30;
            _spellList = spells;
        }

        public override void DrawPanel()
        {
            for (int i = 0; i < _spellList.Count; i++)
            {
                DrawSpellBox(_spellList[i], i + 1);
            }
        }

        private void DrawSpellBox(Spell spell, int num)
        {
            int spellBoxWidth = 19;
            int xOffset = 0;
            int yOffset = 5 * (num - 1);

            //Line for spell cost
            string costLine = "";
            if (spell != null)
            {
                for (int i = 0; i < spell.AttentionCost; i++)
                    costLine = costLine + "*";
                while (costLine.Length + 2 < spellBoxWidth - 10)
                    costLine = costLine + " ";
                for (int i = 0; i < spell.SoulCost; i++)
                    costLine = costLine + "$";
            }

            //Draw panel
            TCODConsole.root.setForegroundColor(TCODColor.white);
            if (spell == null)
            {
                TCODConsole.root.setForegroundColor(TCODColor.darkGrey);
            }
            else if (spell.IsSelected)
            {
                TCODConsole.root.setForegroundColor(TCODColor.yellow);
            }
            else if (spell.IsActive)
            {
                TCODConsole.root.setForegroundColor(TCODColor.blue);
            }
            
            DrawStringInPanel(xOffset, yOffset, "-------------------");
            DrawStringInPanel(xOffset, yOffset + 1, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 2, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 3, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 4, "-------------------");

            if (spell != null)
            {
                if (spell.CurrentCooldown <= 0)
                {
                    //Draw Text
                    TCODConsole.root.setForegroundColor(TCODColor.white);
                    DrawStringInPanel(xOffset + 2, yOffset + 1, num + "          " + spell.CooldownTime + " CD");
                    DrawStringInPanel(xOffset + 2, yOffset + 2, spell.Name);
                    DrawStringInPanel(xOffset + 2, yOffset + 3, costLine);
                }
                else
                {
                    TCODConsole.root.setForegroundColor(TCODColor.white);
                    DrawStringInPanel(xOffset + 9, yOffset + 2, spell.CurrentCooldown.ToString());
                }
            }

        }

    }
}
