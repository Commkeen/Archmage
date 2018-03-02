using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Spells;
using libtcod;

namespace Archmage.Panels
{
    abstract class Panel
    {
        protected IntVector2 _windowPosition;
        protected IntVector2 _windowSize;

        public abstract void DrawPanel();

        protected void DrawCharacterInPanel(IntVector2 position, char c)
        {
            Display.DrawCharacter(position + _windowPosition, c);
        }

        protected void SetCharacterBackgroundInPanel(IntVector2 position, Color color)
        {
            Display.SetBackColorOfPos(position + _windowPosition, color);
        }

        protected void DrawStringInPanel(int x, int y, string str)
        {
            Display.DrawString(new IntVector2(x, y) + _windowPosition, _windowSize.X - x, str);
        }

        protected void DrawStringInPanel(int x, int y, int width, string str)
        {
            Display.DrawString(new IntVector2(x, y) + _windowPosition, width, str);
        }

        protected void DrawStringInPanel(IntVector2 position, string str)
        {
            Display.DrawString(position + _windowPosition, str);
        }
    }
}
