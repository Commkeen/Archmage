using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Actors;
using Archmage.Engine.Items;
using Archmage.Command;
using Archmage.Mapping;
using Archmage.Engine.DataStructures;
using Archmage.SpecialEffects;
using Archmage.Engine.Scenes;
using libtcod;

namespace Archmage.Panels
{
    class MapPanel:Panel
    {
        Map _map; //A pointer to the map
        GameObjectPool _gameObjectPool; //The game object pool
        CommandInterface _command;
        PlayScene _scene;

        public MapPanel(PlayScene scene, Map map, GameObjectPool gameObjectPool, CommandInterface commandInterface)
        {
            _scene = scene;
            _map = map;
            _gameObjectPool = gameObjectPool;
            _command = commandInterface;

            _windowPosition = new IntVector2();
            _windowSize = new IntVector2();

            _windowPosition.X = 21;
            _windowPosition.Y = 0;
            _windowSize.X = 50;
            _windowSize.Y = 30;
        }

        public override void DrawPanel()
        {
            TCODConsole.root.setForegroundColor(TCODColor.white);
            //Draw the map
            for (int i = 0; i < _map.Height; i++)
            {
                for (int k = 0; k < _map.Width; k++)
                {
                    IntVector2 tile = new IntVector2(k,i);
                    if (_scene.GetStudent().HasAttribute("XRAY")
                        || _scene.IsTileVisibleFromLocation(_scene.GetStudent().Position, tile, _scene.GetStudent().SightRange))
                    {
                        RenderTile(tile, false);
                        _map.SetTileExplored(tile, true);
                    }
                    else if (_map.IsTileExplored(tile))
                    {
                        RenderTile(tile, true);
                    }
                }
            }

            //Draw items
            List<ItemToken> items = _gameObjectPool.GetAllAliveItemTokens();

            foreach (ItemToken i in items)
            {
                Item item = _gameObjectPool.GetItem(i.ItemID);
                if (_scene.GetStudent().HasAttribute("XRAY")
                    || _scene.IsTileVisibleFromLocation(_scene.GetStudent().Position, i.Position, _scene.GetStudent().SightRange))
                {
                    TCODConsole.root.setForegroundColor(new TCODColor(item.SpriteColor.R, item.SpriteColor.G, item.SpriteColor.B)); //TODO: Get color properly
                    DrawCharacterInPanel(i.Position, item.Sprite);
                }
                else if (i.PositionKnown)
                {
                    TCODConsole.root.setForegroundColor(TCODColor.darkGrey);
                    DrawCharacterInPanel(i.Position, item.Sprite);
                }
            }


            //Draw actors
            List<Actor> actors = _gameObjectPool.GetAllAliveActors();

            foreach (Actor a in actors)
            {
                if (_scene.GetStudent().HasAttribute("XRAY")
                    || _scene.IsTileVisibleFromLocation(_scene.GetStudent().Position, a.Position, _scene.GetStudent().SightRange))
                {
                    Color spriteColor = a.GetSpriteColor();
                    TCODConsole.root.setForegroundColor(new TCODColor(spriteColor.R, spriteColor.G, spriteColor.B)); //TODO: Get color properly
                    DrawCharacterInPanel(a.Position, a.Sprite);
                }
            }

            //Draw special effects
            List<SpecialEffect> effects = _gameObjectPool.GetAllAliveSpecialEffects();

            foreach (SpecialEffect fx in effects)
            {
                List<Tuple<IntVector2, char, Color>> fxSprites = fx.Draw();
                foreach (Tuple<IntVector2, char, Color> sprite in fxSprites)
                {
                    TCODConsole.root.setForegroundColor(new TCODColor(sprite.Item3.R, sprite.Item3.G, sprite.Item3.B));
                    DrawCharacterInPanel(sprite.Item1, sprite.Item2);
                }
            }

            //Draw command interface (targeting, etc)
            _command.Draw(_windowPosition);
        }


        void RenderTile(IntVector2 tile, bool darken)
        {
            Color tileColor = _map.GetTileSpriteColor(tile);
            Color backColor = _map.GetTileBackColor(tile);
            if (backColor == null)
                backColor = new Color(TCODColor.black);
            Color tintColor = new Color(TCODColor.darkestBlue).Tint(new Color(TCODColor.darkestGrey), 0.6f);
            tintColor = tintColor.Randomize(10, 10, 10);
            float tintAmount = 0.1f;
            if (darken)
            {
                TCODConsole.root.setForegroundColor(tileColor.Tint(tintColor, tintAmount).ToTCODColor());
                SetCharacterBackgroundInPanel(tile, backColor.Tint(tintColor, tintAmount));
            }
            else
            {
                TCODConsole.root.setForegroundColor(tileColor.ToTCODColor());
                SetCharacterBackgroundInPanel(tile, backColor);
            }
            DrawCharacterInPanel(tile, _map.GetTileSprite(tile));
            
            
            TCODConsole.root.setBackgroundColor(TCODColor.black);
        }

    }

    


}
