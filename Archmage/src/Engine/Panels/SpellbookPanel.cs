using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Engine.Spells;
using Archmage.System;
using libtcod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Panels
{
    class SpellbookPanel : Panel
    {
        PlayScene _scene;

        IntVector2 _descriptionPosition;
        IntVector2 _descriptionSize;

        IntVector2 _instructionsPosition;
        IntVector2 _instructionsSize;

        List<Spell> _spellList;

        

        SpellbookInterface _interface;

        public SpellbookPanel(PlayScene scene, SpellbookInterface spellbookInterface)
        {
            _scene = scene;
            _interface = spellbookInterface;
            _windowPosition = new IntVector2();
            _windowSize = new IntVector2();
            _descriptionPosition = new IntVector2();
            _descriptionSize = new IntVector2();
            _instructionsPosition = new IntVector2();
            _instructionsSize = new IntVector2();

            _windowPosition.X = 0;
            _windowPosition.Y = 0;
            _windowSize.X = 50;
            _windowSize.Y = 30;
            _descriptionPosition.X = 21;
            _descriptionPosition.Y = 6;
            _descriptionSize.X = 60;
            _instructionsPosition.X = 21;
            _instructionsPosition.Y = 20;
            _instructionsSize.X = 60;
        }

        public override void DrawPanel()
        {
            List<Spell> spellList = _interface.spellList;
            //Draw equipped spells
            for (int i = 0; i < _scene.GetStudent().NumOfSpellSlots; i++)
            {
                Color c = new Color(TCODColor.white);
                if (i >= spellList.Count)
                    c = new Color(TCODColor.grey);
                if (i == _interface.slotSelected - 1)
                {
                    c = new Color(TCODColor.yellow);
                }
                if (i < spellList.Count)
                    DrawSpellBox(spellList[i], i + 1, 0, 0, c, false);
                else
                    DrawSpellBox(null, i + 1, 0, 0, c, false);
            }

            

            //TODO: If we are learning a spell from the spellbook, draw the spellbox for the spellbook spell
            if (_interface.readSpellbookMode)
            {
                Color c = new Color(TCODColor.white);
                if (_interface.slotSelected == 0)
                    c = new Color(TCODColor.yellow);
                DrawSpellBox(_interface.spellbookSpell, 0, 25, 5, c, false);
            }

            TCODConsole.root.setForegroundColor(TCODColor.white);
            DrawStringInPanel(_descriptionPosition.X, _descriptionPosition.Y, _descriptionSize.X, _interface.descriptionText);
            DrawStringInPanel(_instructionsPosition.X, _instructionsPosition.Y, _instructionsSize.X, _interface.instructionsText);

            /*
            if (_interface.displayConfirmation)
                DrawStringInPanel(2, 8, "Are you sure you want to put this spell in slot " + _interface.slotSelected + "? Y/N");
            else
                DrawStringInPanel(2, 8, "Select a slot to place this spell, or press Escape to cancel");
             * */
        }

        private void DrawSpellBox(Spell spell, int num, int xOff, int yOff, Color color, bool showOnCooldown)
        {
            int spellBoxWidth = 19;
            int xOffset = 0 + xOff;
            int yOffset = 5 * (num - 1) + yOff;

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
            /*
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
             */
            TCODConsole.root.setForegroundColor(new TCODColor(color.R, color.G, color.B));
            
            DrawStringInPanel(xOffset, yOffset, "-------------------");
            DrawStringInPanel(xOffset, yOffset + 1, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 2, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 3, "|                 |");
            DrawStringInPanel(xOffset, yOffset + 4, "-------------------");

            if (spell != null)
            {
                if (!showOnCooldown)
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
