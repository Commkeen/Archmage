using Archmage.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Mapping.LevelGeneration
{
    struct NewLevelGen_TileType
    {
        public string name;
        public char displayChar;
        public Color displayCharColor;

        public NewLevelGen_TileType(string name, char displayChar, Color displayCharColor)
        {
            this.name = name;
            this.displayChar = displayChar;
            this.displayCharColor = displayCharColor;
        }
    }

    enum NewLevelGen_TileTypes
    {
        STONE_WALL,
        UNBREAKABLE_WALL,
        FLOOR,
        GRASS,
        PUDDLE_WATER,
        DEEP_WATER,
        PUDDLE_ICE,
        DEEP_ICE,
        CHASM,
        CHASM_EDGE,
        LAVA,
        DOOR,
        LOCKED_DOOR,
        SECRET_DOOR,
        STAIRS_UP,
        STAIRS_DOWN,
        PLAIN_CHEST,
        LOCKED_CHEST,
        ICE_CHEST,
        GHOST_CHEST
    }

    class NewLevelGen_Tile
    {
        public static NewLevelGen_TileType[] TILE_TYPE_CATALOG = 
        {
            new NewLevelGen_TileType("stone wall", '#', new Color(libtcod.TCODColor.lightGrey)),
            new NewLevelGen_TileType("unbreakable wall", '#', new Color(libtcod.TCODColor.lightestGrey)),
            new NewLevelGen_TileType("floor", '.', new Color(libtcod.TCODColor.darkGrey)),
            new NewLevelGen_TileType("grass", '"', new Color(libtcod.TCODColor.darkGreen)),
            new NewLevelGen_TileType("a puddle", '~', new Color(libtcod.TCODColor.blue)),
            new NewLevelGen_TileType("deep water", '=', new Color(libtcod.TCODColor.blue)),
            new NewLevelGen_TileType("sheet ice", '~', new Color(libtcod.TCODColor.lightestBlue)),
            new NewLevelGen_TileType("frozen lake", '~', new Color(libtcod.TCODColor.lightBlue)),
            new NewLevelGen_TileType("a chasm", '`', new Color(libtcod.TCODColor.darkestGrey)),
            new NewLevelGen_TileType("chasm edge", '.', new Color(libtcod.TCODColor.lightGrey)),
            new NewLevelGen_TileType("lava", '=', new Color(libtcod.TCODColor.red)),
            new NewLevelGen_TileType("a door", '+', new Color(libtcod.TCODColor.sepia)),
            new NewLevelGen_TileType("a locked door", '+', new Color(libtcod.TCODColor.copper)),
            new NewLevelGen_TileType("a secret door", '+', new Color(libtcod.TCODColor.lightGrey)),
            new NewLevelGen_TileType("stairs up", '<', new Color(libtcod.TCODColor.white)),
            new NewLevelGen_TileType("stairs down", '>', new Color(libtcod.TCODColor.white)),
            new NewLevelGen_TileType("a treasure chest", '&', new Color(libtcod.TCODColor.sepia)),
            new NewLevelGen_TileType("a locked chest", '&', new Color(libtcod.TCODColor.copper)),
            new NewLevelGen_TileType("a chest trapped in ice", '&', new Color(libtcod.TCODColor.lightBlue)),
            new NewLevelGen_TileType("a ghostly chest", '&', new Color(libtcod.TCODColor.lightestPurple))
        };

        public IntVector2 Position { get; set; }

        public NewLevelGen_Tile(IntVector2 position)
        {
            Position = new IntVector2(position);
        }
    }

    
}
