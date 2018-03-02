using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Engine.GlobalData;
using Archmage.System;

namespace Archmage.System
{
    class Game
    {

        private static Game _gameInstance;
        public static Game GameInstance
        {
            get
            {
                if (_gameInstance == null)
                    _gameInstance = new Game();
                return _gameInstance;
            }
        }

        Scene CurrentScene;

        public StudentStats GlobalStudentStats { get; protected set; }

        bool _exitProgram;       

        private Game()
        {
            GlobalStudentStats = new StudentStats();

            Display.InitDisplay();
            GotoMainMenu();
        }

        public void GameLoop()
        {
            _exitProgram = false;

            while (!_exitProgram)
            {
                Input.UpdateLastKeyPressed();
                CurrentScene.UpdateScene();
              
                Display.ClearScreen();
                CurrentScene.DrawScene();
                Display.FlushScreen();
            }
        }

        public void GotoMainMenu()
        {
            CurrentScene = new MainMenuScene();
            CurrentScene.InitScene();
        }

        public void GotoDisciplineSelection()
        {
            CurrentScene = new StartingDisciplineSelectScreen();
            CurrentScene.InitScene();
        }

        public void StartPlayScene()
        {
            CurrentScene = new PlayScene(this);
            CurrentScene.InitScene();
        }

        public void WinGame()
        {
            CurrentScene = new WinGameScene();
            CurrentScene.InitScene();
        }

        public void GameOver()
        {
            CurrentScene = new GameOverScene();
            CurrentScene.InitScene();
        }

        public void QuitGame()
        {
            _exitProgram = true;
        }

    }
}
