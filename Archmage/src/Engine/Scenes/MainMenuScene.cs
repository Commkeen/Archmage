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
    class MainMenuScene : Scene
    {
        int selected;

        AnimatedImage logo;

        public MainMenuScene():base()
        {
            selected = 0;
        }

        public override void InitScene()
        {
            logo = new AnimatedImage(@"Assets\TitleLogo.png", new IntVector2(100, 25), 1);
        }

        public override void UpdateScene()
        {
            if (Input.LastKeyPressed == "north")
            {
                //Left
                if (selected > 0)
                    selected--;
            }
            else if (Input.LastKeyPressed == "south")
            {
                //Right
                if (selected < 1)
                    selected++;
            }
            else if (Input.LastKeyPressed == "enter")
            {
                //Enter
                if (selected == 0)
                    Game.GameInstance.GotoDisciplineSelection();
                else if (selected == 1)
                    Game.GameInstance.QuitGame();
            }
        }

        public override void DrawScene()
        {
            logo.Draw(new IntVector2(0, 0), 0);

            TCODConsole.root.setForegroundColor(TCODColor.white);

            Display.DrawString(new IntVector2(50, 43), "New Game");
            if (selected == 0)
                Display.DrawString(new IntVector2(49, 43), ">");
            Display.DrawString(new IntVector2(50, 45), "Exit Game");
            if (selected == 1)
                Display.DrawString(new IntVector2(49, 45), ">");

            Display.DrawString(new IntVector2(0, 49), "v2.0.0");
        }


    }
}
