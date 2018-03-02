using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;
using Archmage.System;
using libtcod;

namespace Archmage.Engine.Scenes
{
    class WinGameScene : Scene
    {

        AnimatedImage logo;

        public WinGameScene()
            : base()
        {
            logo = new AnimatedImage(@"Assets\WinBanner.png", new IntVector2(100, 25), 1);
        }

        public override void UpdateScene()
        {
            if (Input.LastKeyPressed == "enter")
            {
                //Enter
                Game.GameInstance.GotoMainMenu();
            }
        }

        public override void DrawScene()
        {

            TCODConsole.root.setForegroundColor(TCODColor.white);

            logo.Draw(new IntVector2(0, 0), 0);

            Display.DrawString(new IntVector2(25, 30), "Press Enter to return to the menu.");
        }


    }
}
