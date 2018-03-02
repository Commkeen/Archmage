using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;
using libtcod;

namespace Archmage.System
{
    public static class Display
    {
        /*
         * The display class contains the constants for the display.  The display window is divided
         * into several sections, each with its own things to display.  
         * 
         * There will be 4 sections for now:  the main map window, the stats window, the message window, and 2 data windows for other things.
         */

        public const int CONSOLE_WIDTH = 100;
        public const int CONSOLE_HEIGHT = 50;

        public static TCODConsole displayConsole;

        const string font1 = "Assets\\terminal.bmp";
        const string font2 = "Assets\\terminal12x12_gs_ro.png";
        const string font3 = "Assets\\terminal16x16_gs_ro.png";

        public static void InitDisplay()
        {
            TCODConsole.setCustomFont(font2, (int)TCODFontFlags.LayoutAsciiInRow, 16, 16); //Init font
            TCODSystem.setFps(30); //Set draw speed
            TCODConsole.initRoot(CONSOLE_WIDTH, CONSOLE_HEIGHT, "Wizard's Peril", false, TCODRendererType.GLSL); //Init the console
            displayConsole = TCODConsole.root;
        }

        public static void ClearScreen()
        {
            displayConsole.clear();
        }

        public static void FlushScreen()
        {
            TCODConsole.flush();
        }

        public static void DrawCharacter(IntVector2 pos, char c)
        {
            displayConsole.putChar(pos.X, pos.Y, c);
        }

        public static void SetBackColorOfPos(IntVector2 pos, Color color)
        {
            displayConsole.setCharBackground(pos.X, pos.Y, new libtcod.TCODColor(color.R, color.G, color.B));
        }

        public static void DrawString(IntVector2 pos, string str)
        {
            displayConsole.print(pos.X, pos.Y, str);
        }

        public static void DrawString(IntVector2 pos, int width, string str)
        {
            displayConsole.printRect(pos.X, pos.Y, width, 0, str);
        }

        public static void DrawWindowFrame(IntVector2 position, IntVector2 size)
        {
            displayConsole.printFrame(position.X, position.Y, size.X, size.Y);
        }
    }
}
