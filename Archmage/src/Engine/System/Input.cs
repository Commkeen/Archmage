using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
using Archmage.Engine.DataStructures;

namespace Archmage.System
{
    public static class Input
    {

        public static string LastKeyPressed { get; set; }
        public static TCODKey LastTCODKeyPressed { get; set; }

        public static void Init()
        {
            LastKeyPressed = "";
            LastTCODKeyPressed = TCODConsole.checkForKeypress();
        }

        public static void UpdateLastKeyPressed()
        {
            LastTCODKeyPressed = TCODConsole.checkForKeypress();

            switch (LastTCODKeyPressed.KeyCode)
            {
                case TCODKeyCode.KeypadFour:
                case TCODKeyCode.Left:
                    LastKeyPressed = "west";
                    break;
                case TCODKeyCode.KeypadEight:
                case TCODKeyCode.Up:
                    LastKeyPressed = "north";
                    break;
                case TCODKeyCode.KeypadSix:
                case TCODKeyCode.Right:
                    LastKeyPressed = "east";
                    break;
                case TCODKeyCode.KeypadTwo:
                case TCODKeyCode.Down:
                    LastKeyPressed = "south";
                    break;
                case TCODKeyCode.KeypadSeven:
                    LastKeyPressed = "northwest";
                    break;
                case TCODKeyCode.KeypadNine:
                    LastKeyPressed = "northeast";
                    break;
                case TCODKeyCode.KeypadOne:
                    LastKeyPressed = "southwest";
                    break;
                case TCODKeyCode.KeypadThree:
                    LastKeyPressed = "southeast";
                    break;
                case TCODKeyCode.One:
                    LastKeyPressed = "1";
                    break;
                case TCODKeyCode.Two:
                    LastKeyPressed = "2";
                    break;
                case TCODKeyCode.Three:
                    LastKeyPressed = "3";
                    break;
                case TCODKeyCode.Four:
                    LastKeyPressed = "4";
                    break;
                case TCODKeyCode.Enter:
                    LastKeyPressed = "enter";
                    break;
                case TCODKeyCode.Escape:
                    LastKeyPressed = "escape";
                    break;
                default:
                    LastKeyPressed = "";
                    break;
            }
        }

        public static IntVector2 GetMovementInput()
        {
            switch (LastKeyPressed)
            {
                case "north":
                    return new IntVector2(0, -1);
                case "south":
                    return new IntVector2(0, 1);
                case "west":
                    return new IntVector2(-1, 0);
                case "east":
                    return new IntVector2(1, 0);
                case "northeast":
                    return new IntVector2(1, -1);
                case "southeast":
                    return new IntVector2(1, 1);
                case "northwest":
                    return new IntVector2(-1, -1);
                case "southwest":
                    return new IntVector2(-1, 1);
                default:
                    return null;
            }
        }

        public static char GetLetterInput()
        {
            return LastTCODKeyPressed.Character;
        }

        public static int GetLetterAsIndex()
        {
            int index = -1;
            char inputChar = LastTCODKeyPressed.Character;
            if (inputChar != null && inputChar >= 'a' && inputChar <= 'z')
            {
                index = (int)inputChar - (int)'a';
            }
            return index;
        }

        public static int GetNumberAsIndex()
        {
            int index = -1;
            if (IsKeypad(LastTCODKeyPressed))
                return -1;
            switch (LastTCODKeyPressed.Character)
            {
                case '1':
                    index = 1;
                    break;
                case '2':
                    index = 2;
                    break;
                case '3':
                    index = 3;
                    break;
                case '4':
                    index = 4;
                    break;
                case '5':
                    index = 5;
                    break;
                case '6':
                    index = 6;
                    break;
                case '7':
                    index = 7;
                    break;
                case '8':
                    index = 8;
                    break;
                case '9':
                    index = 9;
                    break;
                default:
                    break;
            }
            return index;
        }

        public static bool IsKeypad(TCODKey key)
        {
            bool result = false;
            result |= key.KeyCode == TCODKeyCode.KeypadOne;
            result |= key.KeyCode == TCODKeyCode.KeypadTwo;
            result |= key.KeyCode == TCODKeyCode.KeypadThree;
            result |= key.KeyCode == TCODKeyCode.KeypadFour;
            result |= key.KeyCode == TCODKeyCode.KeypadFive;
            result |= key.KeyCode == TCODKeyCode.KeypadSix;
            result |= key.KeyCode == TCODKeyCode.KeypadSeven;
            result |= key.KeyCode == TCODKeyCode.KeypadEight;
            result |= key.KeyCode == TCODKeyCode.KeypadNine;

            return result;
        }
    }
}
