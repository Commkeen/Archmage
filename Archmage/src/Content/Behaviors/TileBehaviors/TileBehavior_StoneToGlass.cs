using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Behaviors;
using Archmage.Engine.DataStructures;

namespace Archmage.Content.Behaviors.TileBehaviors
{
    class TileBehavior_StoneToGlass:TileBehavior
    {
        public int SpreadStrength { get; set; }

        public TileBehavior_StoneToGlass(int instanceID, IntVector2 affectedTile)
            : base("stoneToGlass", instanceID, affectedTile)
        {
            SpreadStrength = 10;
            _energyPerTick = 500;
        }

        public override TurnResult TakeTurn()
        {
            if (SpreadStrength > 0)
            {
                //Search for other adjacent walls
                for (int x = AffectedTile.X - 1; x <= AffectedTile.X + 1; x++)
                {
                    for (int y = AffectedTile.Y - 1; y <= AffectedTile.Y + 1; y++)
                    {
                        if (x == AffectedTile.X - 1 && y == AffectedTile.Y - 1
                            || x == AffectedTile.X + 1 && y == AffectedTile.Y - 1
                            || x == AffectedTile.X + 1 && y == AffectedTile.Y + 1
                            || x == AffectedTile.X - 1 && y == AffectedTile.Y + 1)
                            continue;
                        IntVector2 tile = new IntVector2(x, y);
                        if (tile.X > 0 && tile.X < _scene.GetMap().Width && tile.Y > 0 && tile.Y < _scene.GetMap().Height)
                        {
                            if (_scene.GetMap().DoesTileHaveFeature(tile, Tiles.Tile_SimpleFeatureType.STONE_WALL) && _scene.GetMap().GetTileBehaviors(tile).Count == 0) //TODO: kind of hack-y
                            {
                                int newID = _scene.GetGameObjectPool().CreateTileBehavior(ObjectID, tile);
                                TileBehavior_StoneToGlass b = (TileBehavior_StoneToGlass)_scene.GetGameObjectPool().GetTileBehavior(newID);
                                b.SpreadStrength = SpreadStrength - 1;
                                _scene.GetMap().AddTileBehavior(newID);
                                _scene.GetTurnCounter().AddObjectToCounter(newID);
                            }
                        }
                    }
                }

                SpreadStrength = 0;
                
            }
            _scene.GetTurnCounter().RemoveObjectFromCounter(InstanceID);
            return base.TakeTurn();
        }

        public override Color GetSpriteColor()
        {
            return new Color(libtcod.TCODColor.lightestGrey);
        }

        public override Color GetBackColor()
        {
            return new Color(libtcod.TCODColor.lightBlue);
        }

        public override bool GetObstructsVision(bool prevResult)
        {
            return false;
        }
    }
}
