using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.DataStructures;
using libtcod;

namespace Archmage.Engine.Scenes
{
    class StartingDisciplineSelectScreen:Scene
    {
        int selectedDiscipline;

        AnimatedImage arcaneLogo;
        AnimatedImage natureLogo;
        AnimatedImage shadowLogo;

        string arcaneDescription;
        string natureDescription;
        string shadowDescription;

        

        public StartingDisciplineSelectScreen()
        {
            selectedDiscipline = 0;
        }

        public override void InitScene()
        {
            arcaneLogo = new AnimatedImage(@"Assets\ArcaneSchool2.png", new IntVector2(16,16), 3);
            natureLogo = new AnimatedImage(@"Assets\NatureSchool.png", new IntVector2(16, 16), 7);
            shadowLogo = new AnimatedImage(@"Assets\ShadowSchoolFinal.png", new IntVector2(16, 16), 5);

            arcaneDescription = "The Discipline of the Arcane is concerned with the purest understanding of magic itself.  Arcane spells channel raw mystical energies, unleashing powerful effects and warping reality around the caster.  An Arcane wizard can not only wield deadly magical forces, but can dispel the magic of his foes as well.  Arcane wizards tend to struggle when they are faced by creatures resistant to magical energy.";
            natureDescription = "The Discipline of Nature uses magic to control plants, animals, and the earth.  Nature wizards can fight their foes using the rocks and plants around them, while shielding themselves in clouds of fog or behind magically summoned stone walls.  Since most Nature magic deals with the physical world, a Nature wizard may have trouble against incorporeal enemies like spirits or ghosts.";
            shadowDescription = "The Discipline of Shadow draws its power from Umbra, the Shadow Plane.  Shadow wizards can cripple enemies with powerful curses, cloak themselves in darkness to avoid enemies, and teleport short distances to avoid their foes.  Shadow magic provides very little protection in direct combat, making sneak attacks essential for Shadow wizards who want to survive against more powerful creatures.";
        
        }

        public override void UpdateScene()
        {
            if (Input.LastKeyPressed == "west")
            {
                //Left
                if (selectedDiscipline > 0)
                    selectedDiscipline--;
            }
            else if (Input.LastKeyPressed == "east")
            {
                //Right
                if (selectedDiscipline < 2)
                    selectedDiscipline++;
            }
            else if (Input.LastKeyPressed == "enter")
            {
                //Enter
                Game.GameInstance.GlobalStudentStats.ChosenDiscipline = selectedDiscipline;
                Game.GameInstance.StartPlayScene();
            }
            else if (Input.LastTCODKeyPressed.KeyCode == TCODKeyCode.F5)
            {
                Game.GameInstance.GlobalStudentStats.ChosenDiscipline = -1;
                Game.GameInstance.StartPlayScene();
            }

            arcaneLogo.AnimationStep(.3F);
            natureLogo.AnimationStep(.3F);
            shadowLogo.AnimationStep(.3F);
        }

        public override void DrawScene()
        {
            TCODConsole.root.setForegroundColor(TCODColor.white);
            Display.DrawString(new IntVector2(25, 2), "Select a magical Discipline for your character.");
            Display.DrawString(new IntVector2(23, 3), "Use the arrow keys and Enter to make your selection.");

            IntVector2 arcanePos = new IntVector2(15,8);
            IntVector2 naturePos = new IntVector2(40,8);
            IntVector2 shadowPos = new IntVector2(65, 8);


            TCODConsole.root.setForegroundColor(TCODColor.darkAzure);
            Display.DrawString(arcanePos + new IntVector2(5, 18), "Arcane");
            if (selectedDiscipline == 0)
                Display.DrawString(arcanePos + new IntVector2(5, 19), "------");

            TCODConsole.root.setForegroundColor(TCODColor.darkGreen);
            Display.DrawString(naturePos + new IntVector2(5, 18), "Nature");
            if (selectedDiscipline == 1)
                Display.DrawString(naturePos + new IntVector2(5, 19), "------");

            TCODConsole.root.setForegroundColor(TCODColor.darkPurple);
            Display.DrawString(shadowPos + new IntVector2(5, 18), "Shadow");
            if (selectedDiscipline == 2)
                Display.DrawString(shadowPos + new IntVector2(5, 19), "------");

            IntVector2 descriptionPos = new IntVector2(22, 32);

            TCODConsole.root.setForegroundColor(TCODColor.lighterGrey);
            if (selectedDiscipline == 0)
                TCODConsole.root.printRect(descriptionPos.X, descriptionPos.Y, 55, 10, arcaneDescription);
            if (selectedDiscipline == 1)
                TCODConsole.root.printRect(descriptionPos.X, descriptionPos.Y, 55, 10, natureDescription);
            if (selectedDiscipline == 2)
                TCODConsole.root.printRect(descriptionPos.X, descriptionPos.Y, 55, 10, shadowDescription);

            //Draw logos
            if (selectedDiscipline == 0)
                arcaneLogo.Draw(arcanePos);
            else
                arcaneLogo.Draw(arcanePos, 0);

            if (selectedDiscipline == 1)
                natureLogo.Draw(naturePos);
            else
                natureLogo.Draw(naturePos, 6);

            if (selectedDiscipline == 2)
                shadowLogo.Draw(shadowPos);
            else
                shadowLogo.Draw(shadowPos, 4);



        }

        
    }
}
