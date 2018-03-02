using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libtcod;
using Archmage.Engine.DataStructures;
using Archmage.System;

namespace Archmage.Command
{
    class TargetingCursor
    {
        public IntVector2 Source { get; set; }
        public IntVector2 Destination { get; set; }
        public int Range { get; set; }
        public bool InRange
        {
            get
            {
                if (Range == 1)
                {
                    return !(Math.Abs(Destination.X - Source.X)>1 || Math.Abs(Destination.Y - Source.Y)>1);
                }
                return ((Destination - Source).Magnitude() <= Range);
            }
        }

        public TargetingCursor(IntVector2 source)
        {
            Source = source;
            Destination = new IntVector2(source.X, source.Y);
            Range = 1;
        }

        public void Draw(IntVector2 windowPosition)
        {
            TCODConsole.root.setForegroundColor(TCODColor.green);

            List<IntVector2> line = IntVector2.LineBetweenPoints(Source, Destination);

            for (int i = 1; i < line.Count; i++)
            {
                if ((line[i] - Source).Magnitude() <= Range)
                {
                    TCODConsole.root.setForegroundColor(TCODColor.green);
                }
                else
                {
                    TCODConsole.root.setForegroundColor(TCODColor.red);
                }
                Display.DrawCharacter(line[i] + windowPosition, '*');
            }
            Display.DrawCharacter(Destination + windowPosition, 'X');

            return;
        }
    }
}
