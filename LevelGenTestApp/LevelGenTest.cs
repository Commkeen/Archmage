using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.System;
using Archmage.Engine.Mapping.LevelGeneration;
using Archmage.Engine.DataStructures;
using Archmage.GameData;


namespace LevelGenTestApp
{
    class LevelGenTest
    {
        NewLevelGenerator _levelGenerator;

        bool _drawRooms;
        bool _drawExits;
        bool _visualizeGeneration;
        Color _bgColor;

        public static IntVector2 DRAW_OFFSET = new IntVector2(0,0);

        public LevelGenTest()
        {
            Display.InitDisplay();
            Display.displayConsole.setForegroundColor(libtcod.TCODColor.lightGrey);
            _bgColor = new Color(libtcod.TCODColor.black);
            Display.displayConsole.setBackgroundColor(_bgColor.ToTCODColor());
            _levelGenerator = new NewLevelGenerator();
            _drawRooms = true;
            _drawExits = true;
            _visualizeGeneration = true;
        }

        public void GameLoop()
        {
            while (true)
            {
                Input.UpdateLastKeyPressed();

                if (Input.LastTCODKeyPressed.KeyCode == libtcod.TCODKeyCode.Space)
                {
                    _levelGenerator.ResetGenerator();
                }

                if (Input.LastTCODKeyPressed.Character == 'r')
                {
                    _drawRooms = !_drawRooms;
                }
                if (Input.LastTCODKeyPressed.Character == 'e')
                {
                    _drawExits = !_drawExits;
                }
                if (Input.LastTCODKeyPressed.Character == 'v')
                {
                    _visualizeGeneration = !_visualizeGeneration;
                }

                
                _levelGenerator.ExecuteNextStep();
                if (!_visualizeGeneration)
                {
                    while (!_levelGenerator._finished)
                        _levelGenerator.ExecuteNextStep();
                }

                Display.ClearScreen();
                DrawMapTiles();
                if (_drawRooms)
                    DrawRoomOutlines();
                if (_drawExits)
                    DrawExits();
                DrawMonsters();
                Display.FlushScreen();
            }
        }

        private void DrawMapTiles()
        {
            for (int i = 0; i < NewLevelGenerator.WIDTH; i++)
            {
                for (int k = 0; k < NewLevelGenerator.HEIGHT; k++)
                {
                    IntVector2 pos = new IntVector2(i,k);
                    Display.SetBackColorOfPos(pos + DRAW_OFFSET, _bgColor);
                    Display.displayConsole.setForegroundColor(_levelGenerator.GetDisplayCharColorOfTile(pos).ToTCODColor());
                    Display.DrawCharacter(pos+DRAW_OFFSET, _levelGenerator.GetDisplayCharOfTile(pos));
                }
            }

            Display.displayConsole.setForegroundColor(libtcod.TCODColor.white);
        }

        private void DrawRoomOutlines()
        {
            List<NewLevelGen_Room> rooms = _levelGenerator.GetRooms();
            Color normalColor = new Color(171, 95, 15);
            Color startColor = new Color(255, 0, 0);
            Color endColor = new Color(34, 67, 89);
            Color critColor = new Color(55, 155, 129);
            
            foreach (NewLevelGen_Room rm in rooms)
            {
                Color drawColor = normalColor;
                if (_levelGenerator._roomsToBeUsed.Contains(rm))
                    drawColor = critColor;
                if (rm.Equals(_levelGenerator._startRoom))
                    drawColor = startColor;
                if (rm.Equals(_levelGenerator._endRoom))
                    drawColor = endColor;

                DrawRoomOutline(rm, normalColor);
            }

            foreach (NewLevelGen_Room room in _levelGenerator._roomsToBeUsed)
            {
                DrawRoomOutline(room, critColor);
            }
            if (_levelGenerator._startRoom != null)
                DrawRoomOutline(_levelGenerator._startRoom, startColor);
            if (_levelGenerator._endRoom != null)
                DrawRoomOutline(_levelGenerator._endRoom, endColor);

            
        }

        private void DrawExits()
        {
            Color exitColor = new Color(150, 55, 129);
            foreach (NewLevelGen_Room room in _levelGenerator.GetRooms())
            {
                foreach (IntVector2 exit in room.Exits)
                {
                    Display.SetBackColorOfPos(exit + DRAW_OFFSET, exitColor);
                }
            }
        }

        private void DrawRoomOutline(NewLevelGen_Room room, Color color)
        {
            foreach (IntVector2 pos in room.GetEdges())
            {
                Display.SetBackColorOfPos(pos + DRAW_OFFSET, color);
            }
        }

        private void DrawMonsters()
        {
            foreach (ActorData a in _levelGenerator.GetMonsters())
            {
                Display.DrawCharacter(new IntVector2(a.x, a.y) + DRAW_OFFSET, 'M');
            }
        }
    }
}
