using Archmage.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Mapping.LevelGeneration
{
    public class NewLevelGen_Room
    {
        public IntVector2 TopLeftCorner { get; set; }
        public IntVector2 Size { get; set; }

        public List<IntVector2> Exits { get; set; }

        public NewLevelGen_Room()
        {
            TopLeftCorner = new IntVector2(0, 0);
            Size = new IntVector2(0, 0);
            Exits = new List<IntVector2>();
        }

        public List<IntVector2> GetEdges()
        {
            List<IntVector2> results = new List<IntVector2>();

            for (int i = 0; i < Size.X; i++)
            {
                results.Add(new IntVector2(TopLeftCorner.X + i, TopLeftCorner.Y));
                results.Add(new IntVector2(TopLeftCorner.X + i, TopLeftCorner.Y + Size.Y - 1));
            }
            for (int i = 1; i < Size.Y - 1; i++)
            {
                results.Add(new IntVector2(TopLeftCorner.X, TopLeftCorner.Y + i));
                results.Add(new IntVector2(TopLeftCorner.X + Size.X - 1, TopLeftCorner.Y + i));
            }

            return results;
        }
    }
}
