using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Tiles;
using Archmage.Behaviors;
using Archmage.GameData;
using Archmage.Engine.Scenes;
using Archmage.Engine.DataStructures;

namespace Archmage.Mapping
{
    class Map
    {
        public Tile[,] tiles;
        public List<int> tileBehaviors;

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        protected PlayScene _scene;

        public Map(PlayScene scene)
        {
            _scene = scene;

            Width = 40;
            Height = 40;
            tiles = new Tile[Width, Height];
            tileBehaviors = new List<int>();
        }

        #region Load/Save

        
        public void LoadMapFromData(int width, int height, TileData[,] tileData)
        {
            Width = width;
            Height = height;

            tiles = new Tile[Width, Height];
            tileBehaviors = new List<int>();

            foreach (TileData t in tileData)
            {
                tiles[t.x, t.y] = new Tile(new IntVector2(t.x, t.y));
                tiles[t.x, t.y].Explored = t.explored;
                for (int i = 0; i < t.layers.Length; i++)
                {
                    tiles[t.x, t.y].SetFeature((Tile_SimpleFeatureType)t.layers[i]);
                }

                if (t.layers[(int)TileLayer.SURFACE] == (int)Tile_SimpleFeatureType.DOOR)
                {
                    int behavior = _scene.GetGameObjectPool().CreateTileBehavior("door", new IntVector2(t.x, t.y));
                    tileBehaviors.Add(behavior);
                }
            }
        }
        

        
        public TileData[,] SaveMapToData()
        {
            TileData[,] tileData = new TileData[Width, Height];

            foreach (Tile t in tiles)
            {
                tileData[t.Position.X, t.Position.Y] = new TileData(t.Position, Array.ConvertAll(t.SimpleLayers, value => (int) value));
                tileData[t.Position.X, t.Position.Y].explored = t.Explored;
            }

            return tileData;
        }
        

        public void ColorTiles(int depth)
        {
            Random r = new Random();

            Color wallColor1 = new Color();
            Color wallColor2 = new Color();
            Color floorColor1 = new Color();
            Color floorColor2 = new Color();

            if (depth == 1)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0x29, 0x8A, 0x08));
                wallColor2 = new Color(new libtcod.TCODColor(0x0B, 0x61, 0x0B));
                floorColor1 = new Color(new libtcod.TCODColor(0x61, 0x38, 0x0B));
            }
            else if (depth == 2)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0x0B, 0x0B, 0x61));
                wallColor2 = new Color(new libtcod.TCODColor(0xFF, 0xFF, 0xFF));
                floorColor1 = new Color(new libtcod.TCODColor(0x2A, 0x1B, 0x0A));
            }
            else if (depth == 3)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0x84, 0x84, 0x84));
                wallColor2 = new Color(new libtcod.TCODColor(0x42, 0x42, 0x42));
                floorColor1 = new Color(new libtcod.TCODColor(0x3A, 0x2F, 0x0B));
            }
            else if (depth == 4)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0x3B, 0x24, 0x0B));
                wallColor2 = new Color(new libtcod.TCODColor(0xA4, 0xA4, 0xA4));
                floorColor1 = new Color(new libtcod.TCODColor(0x6E, 0x6E, 0x6E));
            }
            else if (depth == 5)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0x3B, 0x17, 0x0B));
                wallColor2 = new Color(new libtcod.TCODColor(0xDF, 0x01, 0x01));
                floorColor1 = new Color(new libtcod.TCODColor(0x8A, 0x4B, 0x08));
            }
            else if (depth == 6)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0xFA, 0xFA, 0xFA));
                wallColor2 = new Color(new libtcod.TCODColor(0xBD, 0xBD, 0xBD));
                floorColor1 = new Color(new libtcod.TCODColor(0xA9, 0xBC, 0xF5));
            }
            else if (depth == 7)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0x80, 0x00, 0xFF));
                wallColor2 = new Color(new libtcod.TCODColor(0xDF, 0x01, 0x3A));
                floorColor1 = new Color(new libtcod.TCODColor(0xDF, 0x01, 0x01));
            }
            else if (depth == 8)
            {
                wallColor1 = new Color(new libtcod.TCODColor(0xFF, 0x40, 0x00));
                wallColor2 = new Color(new libtcod.TCODColor(0xB4, 0x04, 0x04));
                floorColor1 = new Color(new libtcod.TCODColor(0xDF, 0x74, 0x01));
            }


            /*
            foreach (Tile t in tiles)
            {
                if (t.ID == "wall")
                {
                    if (r.Next(20) < 18)
                    {
                        t.SpriteColor = wallColor1;
                    }
                    else
                    {
                        t.SpriteColor = wallColor2;
                    }
                }
                if (t.ID == "floor")
                {
                    if (r.Next(20) < 20)
                    {
                        t.SpriteColor = floorColor1;
                    }
                    else
                    {
                        t.SpriteColor = floorColor2;
                    }
                }
            }
             * */
        }

        #endregion


        #region Get map info

        public IntVector2 GetFirstTileWithFeature(Tile_SimpleFeatureType featureType)
        {
            IntVector2 tile = null;
            foreach (Tile t in tiles)
            {
                if (t.DoesTileHaveFeature(featureType))
                    tile = t.Position;
            }

            return tile;
        }

        #endregion


        #region Get tile data

        public bool DoesTileHaveFeature(IntVector2 tilePos, Tile_SimpleFeatureType feature)
        {
            return tiles[tilePos.X, tilePos.Y].DoesTileHaveFeature(feature);
        }

        public string GetTileDescription(IntVector2 tilePos)
        {
            return tiles[tilePos.X, tilePos.Y].GetDescription();
        }

        public char GetTileSprite(IntVector2 tilePos)
        {
            List<int> behaviors = GetTileBehaviors(tilePos);
            char result = tiles[tilePos.X, tilePos.Y].GetSprite();
            int behaviorPriority = int.MaxValue;
            foreach (int id in behaviors)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                if (b.GetPriority() < behaviorPriority && b.GetSprite() != ' ')
                {
                    behaviorPriority = b.GetPriority();
                    result = b.GetSprite();
                }
            }
            return result;
        }

        public Color GetTileSpriteColor(IntVector2 tilePos)
        {
            List<int> behaviors = GetTileBehaviors(tilePos);
            Color result = tiles[tilePos.X, tilePos.Y].GetSpriteColor();
            int behaviorPriority = int.MaxValue;
            foreach (int id in behaviors)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                if (b.GetPriority() < behaviorPriority && b.GetSpriteColor() != null)
                {
                    behaviorPriority = b.GetPriority();
                    result = b.GetSpriteColor();
                }
            }
            return result;
        }

        public Color GetTileBackColor(IntVector2 tilePos)
        {
            List<int> behaviors = GetTileBehaviors(tilePos);
            Color result = tiles[tilePos.X, tilePos.Y].GetSpriteBackColor();
            int behaviorPriority = int.MaxValue;
            foreach (int id in behaviors)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                if (b.GetPriority() < behaviorPriority && b.GetBackColor() != null)
                {
                    behaviorPriority = b.GetPriority();
                    result = b.GetBackColor();
                }
            }
            return result;
        }

        public bool DoesTileObstructActors(IntVector2 tilePos)
        {
            List<int> behaviors = GetTileBehaviors(tilePos);
            bool result = tiles[tilePos.X, tilePos.Y].ObstructsActors();
            foreach (int id in behaviors)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                result = (result || b.GetObstructsActors());
            }
            return result;
        }

        public bool IsTileExplored(IntVector2 tilePos)
        {
            List<int> behaviors = GetTileBehaviors(tilePos);
            bool result = tiles[tilePos.X, tilePos.Y].Explored;
            return result;
        }

        public bool DoesTileObstructVision(IntVector2 tilePos)
        {
            List<int> behaviors = GetTileBehaviors(tilePos);
            bool result = tiles[tilePos.X, tilePos.Y].ObstructsVision();
            foreach (int id in behaviors)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                result = (b.GetObstructsVision(result));
            }
            return result;
        }

        public bool DoesTileObstructProjectiles(IntVector2 tilePos)
        {
            List<int> behaviors = GetTileBehaviors(tilePos);
            bool result = tiles[tilePos.X, tilePos.Y].ObstructsProjectiles();
            foreach (int id in behaviors)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                result = (result || b.GetObstructsProjectiles());
            }
            return result;
        }

        public void AddTileBehavior(int behaviorID)
        {
            tileBehaviors.Add(behaviorID);
        }

        public void RemoveTileBehavior(int behaviorID)
        {
            tileBehaviors.Remove(behaviorID);
        }

        public List<int> GetTileBehaviors(IntVector2 tile)
        {
            List<int> behaviors = new List<int>();
            foreach (int id in tileBehaviors)
            {
                TileBehavior b = _scene.GetGameObjectPool().GetTileBehavior(id);
                if (b != null && b.AffectedTile.Equals(tile))
                {
                    behaviors.Add(id);
                }
            }

            return behaviors;
        }

        #endregion

        #region Set tile data

        public void SetTileExplored(IntVector2 tilePos, bool value)
        {
            tiles[tilePos.X, tilePos.Y].Explored = value;
        }

        #endregion

        #region Tile hooks

        public void TileHook_OnDamage(DamageData dmg)
        {

        }

        public void TileHook_OnActorEnterTile(int actorID)
        {

        }

        #endregion

    }
}
