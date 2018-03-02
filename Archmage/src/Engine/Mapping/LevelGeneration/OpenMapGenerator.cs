using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archmage.GameData;
using Archmage.Engine.DataStructures;

namespace Archmage.Mapping.LevelGeneration
{
    class OpenMapGenerator
    {
        int width = 50;
        int height = 30;

        Random rand;

        LevelData level;

        public OpenMapGenerator()
        {

        }

        public void GenerateMapFrame(LevelData level)//generates floor surrounded by walls
        {
            width = level.width;
            height = level.height;
            int x = 0;
            int y = 0;

            for (x = 0; x < width; x++)
            {
                level.tiles[x, y] = new TileData(x, y, "wall");//first row gen

            }

            for (y = 1; y < height - 1; y++)
            {
                level.tiles[0, y] = new TileData(0, y, "wall");//side walls
                for (x = 1; x < width - 1; x++)
                {
                    level.tiles[x, y] = new TileData(x, y, "floor");

                }
                level.tiles[width - 1, y] = new TileData(width - 1, y, "wall");//side walls
            }
            y = height - 1;
            for (x = 0; x < width; x++)
            {
                level.tiles[x, y] = new TileData(x, y, "wall");//last row gen

            }
        }


        public List<MapGen_Room> GenerateMap(LevelData level)
        {
            rand = new Random();
            this.level = level;

            GenerateMapFrame(level);

            //Add stairs to map
            IntVector2 upStairsTile = new IntVector2(rand.Next(1, width - 1), rand.Next(1, height - 1));
            IntVector2 downStairsTile = new IntVector2(rand.Next(1, width - 1), rand.Next(1, height - 1));

            //level.tiles[upStairsTile.X, upStairsTile.Y] = level.upstairsTile = new TileData(upStairsTile, "upstairs");
            //level.tiles[downStairsTile.X, downStairsTile.Y] = level.downstairsTile = new TileData(downStairsTile, "downstairs");

            //Output rooms as MapGen_Room objects
            List<MapGen_Room> roomList = new List<MapGen_Room>();

            return roomList;
        }
    }
}
