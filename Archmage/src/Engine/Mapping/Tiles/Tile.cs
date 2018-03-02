using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;

namespace Archmage.Tiles
{
    public class Tile
    {
        public Tile_SimpleFeatureType[] SimpleLayers { get; set; }
        private Color[,] RandomizedColors;

        public IntVector2 Position { get; protected set; } //Tiles are referenced by their position on the map

        public bool Explored { get; set; }

        public Tile(IntVector2 position)
        {
            SimpleLayers = new Tile_SimpleFeatureType[(int)TileLayer.NUM_OF_LAYERS];
            RandomizedColors = new Color[(int)TileLayer.NUM_OF_LAYERS,2];

            Position = position;
            Explored = false;

            //InitTile();
        }

        public Tile Copy()
        {
            Tile copy = new Tile(new IntVector2(Position));
            copy.Explored = Explored;
            for (int i = 0; i < (int)TileLayer.NUM_OF_LAYERS; i++)
            {
                Tile_SimpleFeatureType featureType = GetLayer((TileLayer)i);
                copy.SetFeature(featureType);
            }
            return copy;
        }

        /*
        private void InitTile()
        {
            if (ID == "floor")
            {
                Name = "floor";
                Sprite = '.';
                SpriteColor = new Color();
                Passable = true;
                SeeThrough = true;
                ShootThrough = true;
            }

            else if (ID == "wall")
            {
                Name = "wall";
                Sprite = '#';
                SpriteColor = new Color();
                BackColor = new Color(libtcod.TCODColor.darkerGrey);
                Passable = false;
                SeeThrough = false;
                ShootThrough = false;
            }

            else if (ID == "downstairs")
            {
                Name = "downstairs";
                Sprite = '>';
                SpriteColor = new Color();
                Passable = true;
                SeeThrough = true;
                ShootThrough = true;
            }

            else if (ID == "upstairs")
            {
                Name = "upstairs";
                Sprite = '<';
                SpriteColor = new Color();
                Passable = true;
                SeeThrough = true;
                ShootThrough = true;
            }

            else if (ID == "exit")
            {
                Name = "floor";
                Sprite = '.';
                SpriteColor = new Color();
                Passable = true;
                SeeThrough = true;
                ShootThrough = true;
            }

            else if (ID == "door")
            {
                Name = "door";
                Sprite = '.';
                SpriteColor = new Color();
                Passable = true;
                SeeThrough = true;
                ShootThrough = true;
            }

            else if (ID == "fireRift")
            {
                Name = "fireRift";
                Sprite = '>';
                SpriteColor = new Color(libtcod.TCODColor.red);
                Passable = true;
                SeeThrough = true;
                ShootThrough = true;
            }
        }
         * */

        #region Get/Set layer
        public Tile_SimpleFeatureType GetLayer(TileLayer layer)
        {
            return SimpleLayers[(int)layer];
        }

        public void SetFeature(Tile_SimpleFeatureType feature)
        {
            if (feature == Tile_SimpleFeatureType.NONE)
                return;
            SimpleLayers[(int)GetFeature(feature).layer] = feature;
            RandomizedColors[(int)GetFeature(feature).layer, 0] = GetFeature(feature).displayCharColor;
            if (GetFeature(feature).displayCharColor != null && GetFeature(feature).displayCharColorRandomFactor != null)
                RandomizedColors[(int)GetFeature(feature).layer, 0] = RandomizedColors[(int)GetFeature(feature).layer, 0].Randomize(GetFeature(feature).displayCharColorRandomFactor);
            RandomizedColors[(int)GetFeature(feature).layer, 1] = GetFeature(feature).displayBackColor;
            if (GetFeature(feature).displayBackColor != null && GetFeature(feature).displayBackColorRandomFactor != null)
                RandomizedColors[(int)GetFeature(feature).layer, 0] = RandomizedColors[(int)GetFeature(feature).layer, 1].Randomize(GetFeature(feature).displayBackColorRandomFactor);
        }

        public void RemoveFeatureFromLayer(TileLayer layer)
        {
            SimpleLayers[(int)layer] = Tile_SimpleFeatureType.NONE;
            RandomizedColors[(int)layer, 0] = null;
            RandomizedColors[(int)layer, 1] = null;
        }
        #endregion

        static Tile_SimpleFeature GetFeature(Tile_SimpleFeatureType featureType)
        {
            return SimpleFeatureCatalog[(int)featureType];
        }

        
        

        public bool DoesTileHaveFeature(Tile_SimpleFeatureType featureType)
        {
            return (   SimpleLayers[0] == featureType
                    || SimpleLayers[1] == featureType
                    || SimpleLayers[2] == featureType
                    || SimpleLayers[3] == featureType);
        }

        #region Get/Set attributes

        public string GetDescription()
        {
            //TODO
            return "";
        }

        public char GetSprite()
        {
            char sprite = ' ';
            int currentPriority = int.MaxValue;
            for (int i = 0; i < (int)TileLayer.NUM_OF_LAYERS; i++)
            {
                Tile_SimpleFeature feature = GetFeature(GetLayer((TileLayer)i));
                if (feature.priority < currentPriority && feature.displayChar != null)
                {
                    sprite = feature.displayChar;
                    currentPriority = feature.priority;
                }
            }
            return sprite;
        }

        public Color GetSpriteColor()
        {
            Color c = null;
            int currentPriority = int.MaxValue;
            for (int i = 0; i < (int)TileLayer.NUM_OF_LAYERS; i++)
            {
                Tile_SimpleFeature feature = GetFeature(GetLayer((TileLayer)i));
                if (feature.priority < currentPriority && RandomizedColors[i,0] != null)
                {
                    c = RandomizedColors[i,0];
                    currentPriority = feature.priority;
                }
            }
            return c;
        }

        public Color GetSpriteBackColor()
        {
            Color c = null;
            int currentPriority = int.MaxValue;
            for (int i = 0; i < (int)TileLayer.NUM_OF_LAYERS; i++)
            {
                Tile_SimpleFeature feature = GetFeature(GetLayer((TileLayer)i));
                if (feature.priority < currentPriority && RandomizedColors[i, 1] != null)
                {
                    c = RandomizedColors[i, 1];
                    currentPriority = feature.priority;
                }
            }
            return c;
        }

        public bool ObstructsActors()
        {
            bool obstructsActors = false;
            for (int i = 0; i < (int)TileLayer.NUM_OF_LAYERS; i++)
            {
                obstructsActors |= (GetFeature(GetLayer((TileLayer)i)).flags & Tile_SimpleFeatureFlag.OBSTRUCTS_ACTORS) == Tile_SimpleFeatureFlag.OBSTRUCTS_ACTORS;
            }

            return obstructsActors;
        }

        public bool ObstructsVision()
        {
            return (GetFeature(GetLayer(TileLayer.BASE)).flags & Tile_SimpleFeatureFlag.OBSTRUCTS_VISION) == Tile_SimpleFeatureFlag.OBSTRUCTS_VISION;
        }

        public bool ObstructsProjectiles()
        {
            return (GetFeature(GetLayer(TileLayer.BASE)).flags & Tile_SimpleFeatureFlag.OBSTRUCTS_PROJECTILES) == Tile_SimpleFeatureFlag.OBSTRUCTS_PROJECTILES;
        }

        #endregion

        static Tile_SimpleFeature[] SimpleFeatureCatalog = 
        {
            new Tile_SimpleFeature("", TileLayer.BASE, ' ', null, null, null, null, 10000, Tile_SimpleFeatureFlag.NONE, null, null),
            new Tile_SimpleFeature("a stone wall", TileLayer.BASE, '#', ColorCatalog.normalWallColor, ColorCatalog.basicRandomColor, null, null, 0, (Tile_SimpleFeatureFlag.OBSTRUCTS_ALL), null, null),
            new Tile_SimpleFeature("a stone wall", TileLayer.BASE, '#', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.OBSTRUCTS_ALL), null, null),
            new Tile_SimpleFeature("the floor", TileLayer.BASE, '.', ColorCatalog.normalFloorColor, null, null, null, 100, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a patch of grass", TileLayer.SURFACE, '"', ColorCatalog.grassColor, ColorCatalog.randomGrassColor, null, null, 50, Tile_SimpleFeatureFlag.NONE, null, null),
            new Tile_SimpleFeature("a puddle", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a lake", TileLayer.LIQUID, '=', ColorCatalog.lakeColor, null, null, null, 50, (Tile_SimpleFeatureFlag.OBSTRUCTS_ACTORS), null, null),
            new Tile_SimpleFeature("sheet ice", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a frozen lake", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a deep chasm", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("chasm edge", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("lava", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a door", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a locked door", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a secret door", TileLayer.BASE, '.', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("stairs up", TileLayer.BASE, '<', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("stairs leading down", TileLayer.BASE, '>', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("stairs leading somewhere strange", TileLayer.BASE, '>', ColorCatalog.specialStairsColor, null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a portal out of the dungeon", TileLayer.BASE, '<', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null),
            new Tile_SimpleFeature("a treasure chest", TileLayer.SURFACE, '&', new Color(), null, null, null, 0, (Tile_SimpleFeatureFlag.NONE), null, null)
        };

    }

    public enum TileLayer
    {
        BASE = 0,
        LIQUID,
        SURFACE,
        AIR,
        NUM_OF_LAYERS
    }

    public enum Tile_SimpleFeatureType
    {
        NONE = 0,
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
        STAIRS_SPECIAL,
        STAIRS_DUNGEONEXIT,
        PLAIN_CHEST,
        LOCKED_CHEST,
        ICE_CHEST,
        GHOST_CHEST,
        NUM_SIMPLEFEATURETYPES
    }

    [Flags]
    enum Tile_SimpleFeatureFlag
    {
        NONE                = 1 << 0,
        OBSTRUCTS_ACTORS    = 1 << 1,
        OBSTRUCTS_VISION    = 1 << 2,
        OBSTRUCTS_PROJECTILES = 1 << 3,
        OBSTRUCTS_BLASTS = 1 << 4,
        OBSTRUCTS_ALL = (OBSTRUCTS_ACTORS | OBSTRUCTS_VISION | OBSTRUCTS_PROJECTILES | OBSTRUCTS_BLASTS)

    }

    struct Tile_SimpleFeature
    {
        public string description;

        public TileLayer layer;

        public char displayChar;
        public Color displayCharColor;
        public Color displayCharColorRandomFactor;
        public Color displayBackColor;
        public Color displayBackColorRandomFactor;

        public int priority;

        public Tile_SimpleFeatureFlag flags;

        public SimpleFeature_OnDamage OnDamage;
        public SimpleFeature_OnActorEntersTile OnActorEntersTile;

        public Tile_SimpleFeature(  string description, TileLayer layer, char displayChar, Color displayCharColor,
                                    Color displayCharColorRandomFactor, Color displayBackColor, Color displayBackColorRandomFactor,
                                    int priority, Tile_SimpleFeatureFlag flags,
                                    SimpleFeature_OnDamage onDamage, SimpleFeature_OnActorEntersTile onActorEntersTile)
        {
            this.description = description;
            this.layer = layer;
            this.displayChar = displayChar;
            this.displayCharColor = displayCharColor;
            this.displayCharColorRandomFactor = displayCharColorRandomFactor;
            this.displayBackColor = displayBackColor;
            this.displayBackColorRandomFactor = displayBackColorRandomFactor;
            this.priority = priority;
            this.flags = flags;
            this.OnDamage = onDamage;
            this.OnActorEntersTile = onActorEntersTile;
        }
    }

    public delegate void SimpleFeature_OnDamage(DamageData dmg);
    public delegate void SimpleFeature_OnActorEntersTile();

}
