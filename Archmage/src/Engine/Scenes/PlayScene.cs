using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Panels;
using Archmage.Engine.DataStructures;
using Archmage.GameData;
using Archmage.Actors;
using Archmage.Command;
using Archmage.Mapping;
using Archmage.Mapping.LevelGeneration;
using Archmage.SpecialEffects;
using Archmage.Engine.Spells;
using Archmage.Content.Spells;
using Archmage.Behaviors;
using Archmage.Engine.Items;
using Archmage.Engine.Spells.Effects;
using Archmage.Content.Items;
using Archmage.Tiles;
using Archmage.Engine.Mapping.LevelGeneration;
using Archmage.Engine.Mapping;

namespace Archmage.Engine.Scenes
{
    class PlayScene:Scene
    {
        Game _game; //Parent

        //List<LevelData> _levelDatas; //All the levels that have been generated so far, so we can go back to them
        Dungeon _dungeon; //Object for holding levels we have been to
        LevelData _currentLevel;

        NewLevelGenerator _gen;

        Map _map;
        Dungeon_Branches _currentDungeonBranch;
        int _currentDepth;
        int _maxDepth; //The farthest the player has gone this game

        int _studentID;

        GameObjectPool _gameObjectPool;

        SpellFactory _spellFactory;

        TurnCounter _turnCounter;
        List<int> _specialEffects;

        List<Panel> _panels; //Elements of the display

        Queue<string> _messageQueue; //Messages to the player
        
        //Variables for pausing the game to offer the level up/spell select interface
        bool _upgradeScreenActive = false;
        UpgradeInterface _upgradeInterface;
        UpgradePanel _upgradePanel;

        bool _learnSpellScreenActive = false;
        LearnSpellInterface _learnSpellInterface;
        LearnSpellPanel _learnSpellPanel;

        bool _spellbookScreenActive = false;
        SpellbookInterface _spellbookInterface;
        SpellbookPanel _spellbookPanel;

        bool _gameOver = false; //Pauses the game on death

        public PlayScene(Game game)
        {
            _game = game;

            //Give systems a static PlayScene reference
            GameObject.InitSceneReference(this);
            Ability.InitSceneReference(this);
            Effect.InitSceneReference(this);
            Item.InitItemIdentifications();

            _dungeon = new Dungeon();
            _gen = new NewLevelGenerator();
            _gameObjectPool = new GameObjectPool(this);
            _map = new Map(this);
            _turnCounter = new TurnCounter(this);
            _specialEffects = new List<int>();

            _currentDepth = 0;
            _maxDepth = 1;

            _messageQueue = new Queue<string>();

            _panels = new List<Panel>();

            _spellFactory = new SpellFactory();

            
        }

        #region Core scene methods

        public override void InitScene()
        {
            _studentID = _gameObjectPool.CreateActor("playerStudent");
            

            Discipline startingDiscipline = new Discipline_Arcane();

            if (Game.GameInstance.GlobalStudentStats.ChosenDiscipline == 0)
            {
                startingDiscipline = new Discipline_Arcane();
            }
            else if (Game.GameInstance.GlobalStudentStats.ChosenDiscipline == 1)
            {
                startingDiscipline = new Discipline_Nature();
            }
            else if (Game.GameInstance.GlobalStudentStats.ChosenDiscipline == 2)
            {
                startingDiscipline = new Discipline_Shadow();
            }
            else if (Game.GameInstance.GlobalStudentStats.ChosenDiscipline == -1)
            {
                startingDiscipline = new Discipline_Debug();
            }

            
            foreach (string s in startingDiscipline.GetSpells(0))
            {
                GetStudent().AddSpell(CreateSpell(s, _studentID));
            }

            GotoLevel(Dungeon_Branches.CELLAR, 1);

            _panels.Add(new MapPanel(this, _map, _gameObjectPool, GetStudent().Command));
            _panels.Add(new SpellHotbarPanel(GetStudent().Spells));
            _panels.Add(new MessagePanel(_messageQueue));
            _panels.Add(new CharacterStatPanel(GetStudent(), this));
            _panels.Add(new ItemsPanel(GetStudent().Items));
        }

        public override void UpdateScene()
        {
            if (_upgradeScreenActive)
            {
                _upgradeInterface.Update();
            }

            else if (_learnSpellScreenActive)
            {
                _learnSpellInterface.Update();
            }

            else if (_spellbookScreenActive)
            {
                _spellbookInterface.Update();
            }

            else if (_gameOver)
            {
                if (Input.LastTCODKeyPressed.KeyCode == libtcod.TCODKeyCode.Enter)
                    _game.GameOver();
            }

            else
            {
                bool fxPlaying = false;
                //Update special effects
                for (int i = 0; i < _specialEffects.Count; i++)
                {
                    SpecialEffect effect = _gameObjectPool.GetSpecialEffect(_specialEffects[i]);

                    if (effect == null || !effect.Alive)
                    {
                        _specialEffects.RemoveAt(i);
                        i--;
                    }
                    else if (effect.Awake)
                    {
                        fxPlaying = true;
                        effect.Update();
                    }
                }
                if (!fxPlaying)
                {
                    //If there are no special effects playing, update the turn counter
                    _turnCounter.Update();
                }
            }
        }

        public override void DrawScene()
        {
            if (_upgradeScreenActive)
            {
                _upgradePanel.DrawPanel();
            }
            else if (_learnSpellScreenActive)
            {
                _learnSpellPanel.DrawPanel();
            }
            else if (_spellbookScreenActive)
            {
                _spellbookPanel.DrawPanel();
            }
            else
            {
                foreach (Panel p in _panels)
                    p.DrawPanel();
            }
        }

        #endregion

        #region Data Accessors

        public GameObjectPool GetGameObjectPool()
        {
            return _gameObjectPool;
        }

        public TurnCounter GetTurnCounter()
        {
            return _turnCounter;
        }

        /// <summary>
        /// A way to get the Student easily.
        /// </summary>
        /// <returns></returns>
        public Student GetStudent()
        {
            return (Student)_gameObjectPool.GetActor(_studentID);
        }

        public Spell CreateSpell(string spellID, int ownerID)
        {
            return _spellFactory.CreateSpell(spellID, this, ownerID);
        }

        #region Map information

        public string GetLevelName()
        {
            string name = "Dungeon";
            if (_currentDepth == 1)
            {
                name = "Covus Woods";
            }
            if (_currentDepth == 2)
            {
                name = "Upper Caverns";
            }
            if (_currentDepth == 3)
            {
                name = "Goblin Delves";
            }
            if (_currentDepth == 4)
            {
                name = "Goblin Mines";
            }
            if (_currentDepth == 5)
            {
                name = "Ruby Caverns";
            }
            if (_currentDepth == 6)
            {
                name = "Lost Enclaves";
            }
            if (_currentDepth == 7)
            {
                name = "Labyrinth of Madness";
            }
            if (_currentDepth == 8)
            {
                name = "The Phlegian Gate";
            }
            if (_currentDepth == 9)
            {
                name = "Pyrius, the Plane of Fire";
            }

            return name;
        }

        public int GetDepth()
        {
            return _currentDepth;
        }

        public List<int> GetItemTokensAtPosition(IntVector2 position)
        {
            List<int> list = new List<int>();
            foreach (ItemToken a in _gameObjectPool.GetAllAliveItemTokens())
            {
                if (a.Position.Equals(position))
                {
                    list.Add(a.InstanceID);
                }
            }
            return list;
        }

        public List<int> GetActorsAtPosition(IntVector2 position)
        {
            List<int> list = new List<int>();
            foreach (Actor a in _gameObjectPool.GetAllAliveActors())
            {
                if (a.Position.Equals(position))
                {
                    list.Add(a.InstanceID);
                }
            }
            return list;
        }

        public List<int> GetActorsWithinDistance(IntVector2 position, int distance)
        {
            List<int> actors = new List<int>();

            foreach (Actor a in _gameObjectPool.GetAllAliveActors())
            {
                if ((a.Position - position).Magnitude() <= distance)
                {
                    actors.Add(a.InstanceID);
                }
            }

            return actors;
        }

        public List<IntVector2> GetTilesWithinDistance(IntVector2 position, int distance, bool outline)
        {
            List<IntVector2> tiles = new List<IntVector2>();

            for (int i = 0; i <= distance * 2; i++)
            {
                for (int k = 0; k <= distance * 2; k++)
                {
                    IntVector2 tile = new IntVector2(i + position.X - distance, k + position.Y - distance);
                    if (tile.X >= 0 && tile.Y >= 0 && tile.X < GetMap().Width && tile.Y < GetMap().Height)
                    {
                        int magnitude = (position - tile).Magnitude();
                        if (magnitude == distance)
                        {
                            tiles.Add(tile);
                        }
                        else if (!outline && magnitude <= distance)
                        {
                            tiles.Add(tile);
                        }
                    }
                }
            }

            return tiles;
        }

        public bool IsPositionFree(IntVector2 position)
        {
            bool positionFree = true;
            if (_map.DoesTileObstructActors(position))
                positionFree = false;

            if (GetActorsAtPosition(position).Count != 0)
                positionFree = false;

            return positionFree;
        }

        public bool IsPositionFree(IntVector2 position, List<int> ignoreTheseActors)
        {
            bool positionFree = true;
            if (_map.DoesTileObstructActors(position))
                positionFree = false;

            List<int> actorsAtPos = GetActorsAtPosition(position);
            foreach (int a in ignoreTheseActors)
                actorsAtPos.Remove(a);
            if (actorsAtPos.Count != 0)
                positionFree = false;

            return positionFree;
        }

        public List<IntVector2> GetBestRouteBetweenPositions(IntVector2 source, IntVector2 destination)
        {
            //TODO: Put some pathfinding here!
            List<IntVector2> list = new List<IntVector2>();

            list = IntVector2.LineBetweenPoints(source, destination);

            return list;
        }

        public bool IsTileShootableFromLocation(IntVector2 source, IntVector2 destination, int range)
        {
            if ((destination - source).Magnitude() > range)
                return false;

            List<IntVector2> path = IntVector2.LineBetweenPoints(source, destination);
            for (int i = 1; i < path.Count - 1; i++) //We don't care if the source or destination is shootthrough
            {
                if (_map.DoesTileObstructProjectiles(path[i]))
                    return false;
            }

            return true;
        }

        public bool IsTileVisibleFromLocation(IntVector2 source, IntVector2 destination, int sightRange)
        {
            if ((destination - source).Magnitude() > sightRange)
                return false;

            List<IntVector2> sightPath = IntVector2.LineBetweenPoints(source, destination);
            for (int i = 1; i < sightPath.Count - 1; i++) //We don't care if the source or destination is seethrough
            {
                if (_map.DoesTileObstructVision(sightPath[i]))
                    return false;
            }

            return true;
        }

        public Map GetMap()
        {
            return _map;
        }

        public List<IntVector2> GetPathModifiedForCollisions(IntVector2 startingPos, IntVector2 targetPos)
        {
            List<IntVector2> path = IntVector2.LineBetweenPoints(startingPos, targetPos);
            int currentIndex = 1;
            while (currentIndex < path.Count - 1)
            {
                if (!IsPositionFree(path[currentIndex]))
                {
                    targetPos = path[currentIndex];
                    path = IntVector2.LineBetweenPoints(startingPos, targetPos);
                    currentIndex = 1;
                }
                else
                {
                    currentIndex++;
                }
            }
            return path;
        }

        public List<IntVector2> GetPathModifiedForCollisions(IntVector2 startingPos, IntVector2 targetPos, List<int> ignoreTheseActors)
        {
            List<IntVector2> path = IntVector2.LineBetweenPoints(startingPos, targetPos);
            int currentIndex = 1;
            while (currentIndex < path.Count - 1)
            {
                if (!IsPositionFree(path[currentIndex], ignoreTheseActors))
                {
                    targetPos = path[currentIndex];
                    path = IntVector2.LineBetweenPoints(startingPos, targetPos);
                    currentIndex = 1;
                }
                else
                {
                    currentIndex++;
                }
            }
            return path;
        }

        //TODO: Maybe throw this whole thing out cause its stupid
        public string GetAttackDescription(DamageData data)
        {
            string attackDescription = "";

            Actor src = _gameObjectPool.GetActor(data.SourceID);
            Actor tgt = _gameObjectPool.GetActor(data.TargetID);

            if (src.ObjectID == "playerStudent" && tgt.ObjectID != "playerStudent")
            {
                attackDescription = "You hit the " + tgt.Name + "!";
            }

            if (tgt.ObjectID == "playerStudent" && src.ObjectID != "playerStudent")
            {
                attackDescription = "The " + src.Name + " hits you!";
            }

            return attackDescription;
        }

        

        #endregion

        #endregion

        #region Game functionality

        public void AddSpecialEffect(int id)
        {
            _specialEffects.Add(id);
        }

        public void WriteMessage(string message)
        {
            _messageQueue.Enqueue(message);
        }

        public void UseStairs(IntVector2 position)
        {
            Dungeon_Branches destinationBranch = Dungeon_Branches.CELLAR;
            int destinationDepth = -1;
            foreach (StairsData s in _currentLevel.stairs)
            {
                if (s.x == position.X && s.y == position.Y)
                {
                    destinationBranch = (Dungeon_Branches)s.branch;
                    destinationDepth = s.depth;
                }
            }
            if (destinationDepth == 0)
            {
                _gameOver = true;
            }
            else if (destinationDepth != -1)
            {
                GotoLevel(destinationBranch, destinationDepth);
            }
        }

        public void GotoLevel(Dungeon_Branches branch, int depth)
        {
            //TODO: Any spells affecting monsters on the previous level need to be shut off!

            //Save the player
            Student s = (Student)_gameObjectPool.GetActor(_studentID);

            //Save the player's ActorBehaviors
            List<ActorBehavior> studentBehaviors = new List<ActorBehavior>();
            foreach (int bID in s.GetBehaviors())
            {
                studentBehaviors.Add(_gameObjectPool.GetActorBehavior(bID));
            }

            //Save the current level
            if (_currentDepth != 0)
                _dungeon.SaveLevel(_currentDungeonBranch, _currentDepth, SaveLevelToData());

            //Reset the game object pool and turn counter objects
            _gameObjectPool.ResetGameObjectPool();
            _turnCounter.ClearTurnCounter();

            //Move all the student stuff into the new game object pool
            //TODO: Do this instead: Save the student as a data object properly, with its behaviors
            //And then load it into the level normally, as if the player loaded a saved game
            _gameObjectPool.AddObjectToPool(s, _studentID); //TODO: This is hack-y and should be changed somehow
            _turnCounter.AddObjectToCounter(_studentID);
            foreach (ActorBehavior b in studentBehaviors)
            {
                _gameObjectPool.AddObjectToPool(b, b.InstanceID);
            }

            //Build level
            _currentLevel = _dungeon.LoadLevel(branch, depth);
            if (_currentLevel == null)
            {
                _currentLevel = _gen.GenerateLevel(branch, depth);
            }
            LoadLevelFromData(_currentLevel);

            IntVector2 playerStart = new IntVector2(5, 5);
            //Place the player at the proper location
            if (_currentDepth > depth)
            {
                //Player climbed up, put the player on the down stairs
                IntVector2 tile = _map.GetFirstTileWithFeature(Tile_SimpleFeatureType.STAIRS_DOWN);
                if (tile != null)
                {
                    playerStart = tile;
                }
            }
            else
            {
                //Player went down, put the player on the up stairs
                IntVector2 tile = _map.GetFirstTileWithFeature(Tile_SimpleFeatureType.STAIRS_UP);
                if (tile != null)
                {
                    playerStart = tile;
                }
            }

            

            _gameObjectPool.GetActor(_studentID).WarpToPosition(playerStart);
            
            _currentDepth = depth;
            if (_currentDepth > _maxDepth)
                _maxDepth = _currentDepth;

            //A hacky way to make the levels look unique
            _map.ColorTiles(_currentDepth);
        }

        public void OpenUpgradePanel()
        {
            _upgradeScreenActive = true;
            _upgradeInterface = new UpgradeInterface(this);
            _upgradePanel = new UpgradePanel(this, _upgradeInterface);
        }

        public void CloseUpgradePanel()
        {
            _upgradeScreenActive = false;
            _upgradeInterface = null;
            _upgradePanel = null;
        }

        public void OpenLearnSpellPanel(Item_Spellbook s)
        {
            _learnSpellScreenActive = true;
            _learnSpellInterface = new LearnSpellInterface(this, s);
            _learnSpellPanel = new LearnSpellPanel(this, _learnSpellInterface);
        }

        public void CloseLearnSpellPanel()
        {
            _learnSpellScreenActive = false;
            _learnSpellInterface = null;
            _learnSpellPanel = null;
        }

        public void OpenSpellbookPanel()
        {
            _spellbookScreenActive = true;
            _spellbookInterface = new SpellbookInterface(this);
            _spellbookPanel = new SpellbookPanel(this, _spellbookInterface);
        }

        public void OpenSpellbookPanel(Item_Spellbook book)
        {
            _spellbookScreenActive = true;
            _spellbookInterface = new SpellbookInterface(this);
            _spellbookPanel = new SpellbookPanel(this, _spellbookInterface);
            _spellbookInterface.ReadSpellbook(book);
        }

        public void CloseSpellbookPanel()
        {
            _spellbookScreenActive = false;
            _spellbookInterface = null;
            _spellbookPanel = null;
        }

        public void WinGame()
        {
            _game.WinGame();
        }

        public void LoseGame()
        {
            WriteMessage("You have died.  Press Enter...");

            _gameOver = true;
        }

        #endregion

        #region Helper functions

        private LevelData SaveLevelToData()
        {
            //TODO: Save behaviors too!
            LevelData lvl = _currentLevel;//new LevelData(_map.Width, _map.Height, (int)_currentDungeonBranch, _currentDepth);
            lvl.tiles = _map.SaveMapToData();
            lvl.actors = new List<ActorData>();
            foreach (Actor a in _gameObjectPool.GetAllAliveActors())
            {
                if (a.ObjectID != "playerStudent")
                    lvl.actors.Add(new ActorData(a.Position.X, a.Position.Y, a.ObjectID));
            }

            //TODO: items

            return lvl;
        }

        private void LoadLevelFromData(LevelData data)
        {
            _map.LoadMapFromData(data.width, data.height, data.tiles);

            foreach (ActorData a in data.actors)
            {
                int actorID = _gameObjectPool.CreateActor(a.actorType);
                _gameObjectPool.GetActor(actorID).WarpToPosition(new IntVector2(a.x, a.y));
                _turnCounter.AddObjectToCounter(actorID);
            }

            foreach (ItemData i in data.items)
            {
                int itemID = _gameObjectPool.CreateItem(i.itemName);
                if (i.itemName == "itemSpellbook")
                {
                    (_gameObjectPool.GetItem(itemID) as Item_Spellbook).InitSpellbook(i.parameter);
                }
                int itemTokenID = _gameObjectPool.CreateItemToken(itemID);
                _gameObjectPool.GetItemToken(itemTokenID).Position = new IntVector2(i.x, i.y);
            }
        }

        #endregion

    }
}
