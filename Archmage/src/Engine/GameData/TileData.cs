using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;

namespace Archmage.GameData
{
    public class TileData
    {
        public int x;
        public int y;
        public int[] layers;
        public bool explored;

        public TileData(int x, int y, int[] layers)
        {
            this.x = x;
            this.y = y;
            this.layers = layers;
            explored = false;
        }

        public TileData(IntVector2 pos, int[] layers)
            : this(pos.X, pos.Y, layers)
        {

        }

        public TileData(int x, int y, string type)
        {

        }

        public TileData(IntVector2 pos, string type)
        {

        }
    }
}
