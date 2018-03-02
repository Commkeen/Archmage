using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Items;
using Archmage.System;
using Archmage.Engine.DataStructures;
using libtcod;

namespace Archmage.Panels
{
    /// <summary>
    /// Displays the player's items.
    /// </summary>
    class ItemsPanel : Panel
    {
        List<Item> _itemList;

        public ItemsPanel(List<Item> items)
        {
            _windowPosition = new IntVector2();
            _windowSize = new IntVector2();

            _windowPosition.X = 0;
            _windowPosition.Y = 30;
            _windowSize.X = 50;
            _windowSize.Y = 20;
            _itemList = items;
        }

        public override void DrawPanel()
        {
            TCODConsole.root.setForegroundColor(TCODColor.white);
            Display.DrawWindowFrame(_windowPosition, _windowSize);
            Display.DrawString(new IntVector2(22, 1) + _windowPosition, "(U)se");// - (I)nspect - (D)rop");

            int lineX = 1;
            int lineY = 2;
            string lineText = "";
            char lineIndex = 'a';
            for (int i = 0; i < _itemList.Count; i++)
            {
                lineY++;
                if (lineY > _windowSize.Y - 2)
                {
                    lineY = 1;
                    lineX += 15;
                }
                lineText = lineIndex + "-" + _itemList[i].Name;
                DrawStringInPanel(new IntVector2(lineX, lineY), lineText);
                lineIndex++;
            }
        }
    }
}
