using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.GameData
{
    public class LevelData
    {
        public int height;
        public int width;

        public int dungeonBranch;
        public int depth;

        public TileData[,] tiles;
        public List<ActorData> actors;
        public List<ItemData> items;
        public List<StairsData> stairs;

        public LevelData(int width, int height, int dungeonBranch, int depth)
        {
            this.height = height;
            this.width = width;
            this.dungeonBranch = dungeonBranch;
            this.depth = depth;

            tiles = new TileData[width, height];
            actors = new List<ActorData>();
            items = new List<ItemData>();
            stairs = new List<StairsData>();
        }
    }

    public struct StairsData
    {
        public int x;
        public int y;
        public int branch;
        public int depth;

        public StairsData(int x, int y, int branch, int depth)
        {
            this.x = x;
            this.y = y;
            this.branch = branch;
            this.depth = depth;
        }
    }
}
