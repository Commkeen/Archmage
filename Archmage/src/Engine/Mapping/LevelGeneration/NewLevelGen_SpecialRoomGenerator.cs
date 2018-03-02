using Archmage.Engine.DataStructures;
using Archmage.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Mapping.LevelGeneration
{
    class NewLevelGen_SpecialRoomGenerator
    {


        public static bool TreasureRoomAGenerator(NewLevelGen_Room room, Tile[,] roomContents)
        {
            bool success = true;

            if (room.Size.X > 10 || room.Size.Y > 10)
                return false;

            while (room.Exits.Count > 1)
            {
                roomContents[room.Exits[0].X - room.TopLeftCorner.X, room.Exits[0].Y - room.TopLeftCorner.Y].SetFeature(Tile_SimpleFeatureType.STONE_WALL);
                room.Exits.RemoveAt(0);
            }

            for (int i = 1; i < room.Size.X - 1; i++)
            {
                for (int k = 1; k < room.Size.Y - 1; k++)
                {
                    roomContents[i, k].SetFeature(Tile_SimpleFeatureType.FLOOR);
                }
            }

            for (int i = 2; i < room.Size.X - 2; i++)
            {
                roomContents[i, 2].SetFeature(Tile_SimpleFeatureType.DEEP_WATER);
                roomContents[i, room.Size.Y - 3].SetFeature(Tile_SimpleFeatureType.DEEP_WATER);
            }

            for (int i = 2; i < room.Size.Y - 2; i++)
            {
                roomContents[2, i].SetFeature(Tile_SimpleFeatureType.DEEP_WATER);
                roomContents[room.Size.X - 3, i].SetFeature(Tile_SimpleFeatureType.DEEP_WATER);
            }

            roomContents[room.Size.X / 2, room.Size.Y / 2].SetFeature(Tile_SimpleFeatureType.PLAIN_CHEST);

            return success;
        }

        public static bool CatacombsEntranceGenerator(NewLevelGen_Room room, Tile[,] roomContents, out IntVector2 stairsLoc)
        {
            stairsLoc = null;
            bool success = true;

            if (room.Size.X > 10 || room.Size.Y > 10)
                return false;

            while (room.Exits.Count > 1)
            {
                roomContents[room.Exits[0].X - room.TopLeftCorner.X, room.Exits[0].Y - room.TopLeftCorner.Y].SetFeature(Tile_SimpleFeatureType.STONE_WALL);
                room.Exits.RemoveAt(0);
            }

            for (int i = 1; i < room.Size.X - 1; i++)
            {
                for (int k = 1; k < room.Size.Y - 1; k++)
                {
                    roomContents[i, k].SetFeature(Tile_SimpleFeatureType.FLOOR);
                }
            }
            stairsLoc = roomContents[room.Size.X / 2, room.Size.Y / 2].Position;
            roomContents[room.Size.X / 2, room.Size.Y / 2].SetFeature(Tile_SimpleFeatureType.STAIRS_SPECIAL);

            return success;
        }

    }
}
