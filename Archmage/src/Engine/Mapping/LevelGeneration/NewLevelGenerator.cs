using Archmage.Engine.DataStructures;
using Archmage.Engine.Spells;
using Archmage.Engine.System;
using Archmage.GameData;
using Archmage.System;
using Archmage.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Mapping.LevelGeneration
{
    public class NewLevelGenerator
    {

        public static int WIDTH = 50;
        public static int HEIGHT = 30;

        Random rand;

        private Dungeon_Branches _branch;
        private int _depth;

        private Tile[,] _map;
        private List<ActorData> _monsters;
        private List<ItemData> _items;
        private List<NewLevelGen_Room> _rooms;
        private List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>> _roomAdjacencies; //RoomA, RoomB, random "distance" weight for making random paths between rooms

        IntVector2 _roomSize = new IntVector2(8, 8);

        int[] _pathWeights = { 1, 5, 12 };

        private int _mergeRoomsIterationsMax = 40;
        private int _mergeRoomsIterations;

        public NewLevelGen_Room _startRoom;
        public NewLevelGen_Room _endRoom;
        private IntVector2 _upStairs;
        private IntVector2 _downStairs;
        private IntVector2 _specialStairs;

        public List<NewLevelGen_Room> _roomsToBeUsed;
        public List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>> _adjacenciesToBeUsed;

        private int _branchRoomsIterationsMax = 20;
        private int _branchRoomsIterations;

        public List<IntVector2> _floods;

        private int _digCavesIterationsMax = 3000;
        private int _digCavesIterationsBetweenDraws = 100;
        private int _digCavesIterations;

        private int _makeLakesIterationsMax = 15;
        private int _makeLakesIterations;

        private int _monsterPointsForLevel;
        private int _monsterPointsUsed;

        private int _currentStep;

        public bool _finished;

        public NewLevelGenerator()
        {
            rand = new Random();

            SetDepth(1);
            _branch = Dungeon_Branches.CATACOMBS;

            ResetGenerator();
        }

        public void ResetGenerator()
        {
            _map = new Tile[WIDTH, HEIGHT];
            _monsters = new List<ActorData>();
            _items = new List<ItemData>();
            _startRoom = null;
            _endRoom = null;
            _upStairs = null;
            _downStairs = null;
            _rooms = new List<NewLevelGen_Room>();
            _roomsToBeUsed = new List<NewLevelGen_Room>();
            _roomAdjacencies = new List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>>();
            _adjacenciesToBeUsed = new List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>>();
            _floods = new List<IntVector2>();
            InitMapWithTileType(Tile_SimpleFeatureType.STONE_WALL);
            CreateRooms();

            _mergeRoomsIterations = 0;
            _branchRoomsIterations = 0;
            _digCavesIterations = 0;
            _makeLakesIterations = 0;

            _monsterPointsForLevel = _depth * 15;
            _monsterPointsUsed = 0;

            _currentStep = 0;
            _finished = false;
        }

        public void SetDepth(int depth)
        {
            _depth = depth;
        }

        public LevelData GenerateLevel(Dungeon_Branches branch, int depth)
        {
            _branch = branch;
            _depth = depth;
            ResetGenerator();

            while (!_finished)
                ExecuteNextStep();

            return GetLevelData();
        }

        public LevelData GetLevelData()
        {
            LevelData lvl = new LevelData(WIDTH, HEIGHT, (int)_branch, _depth);
            lvl.tiles = new TileData[WIDTH, HEIGHT];
            for (int i = 0; i < WIDTH; i++)
            {
                for (int k = 0; k < HEIGHT; k++)
                {
                    lvl.tiles[i, k] = new TileData(_map[i, k].Position, Array.ConvertAll(_map[i, k].SimpleLayers, value => (int)value));
                }
            }

            lvl.actors = new List<ActorData>(_monsters);
            lvl.items = new List<ItemData>(_items);

            lvl.stairs.Add(new StairsData(_upStairs.X, _upStairs.Y, (int)_branch, _depth - 1));
            lvl.stairs.Add(new StairsData(_downStairs.X, _downStairs.Y, (int)_branch, _depth + 1));
            if (_specialStairs != null)
            {
                lvl.stairs.Add(new StairsData(_specialStairs.X, _specialStairs.Y, 1, 1));
            }

            return lvl;
        }

        public Tile[,] GetMap()
        {
            return _map;
        }

        

        public void ExecuteNextStep()
        {
            switch (_currentStep)
            {
                case 0:
                    MergeRoomsIteration();
                    _mergeRoomsIterations++;
                    if (_mergeRoomsIterations >= _mergeRoomsIterationsMax)
                    {
                        _currentStep++;
                    }
                    break;
                case 1:
                    ChooseStartAndEndRooms();
                    _currentStep++;
                    break;
                case 2:
                    GetCriticalPathBetweenStartAndEndRooms();
                    if (_roomsToBeUsed.Count < 4)
                    {
                        _currentStep = 1;
                    }
                    else
                    {
                        _currentStep++;
                    }
                    break;
                case 3:
                    AddRandomRoomToPathIteration();
                    _branchRoomsIterations++;
                    if (_branchRoomsIterations >= _branchRoomsIterationsMax)
                    {
                        _currentStep++;
                    }
                    break;
                case 4:
                    ClearOutUnusedRooms();
                    _currentStep++;
                    break;
                case 5:
                    DigOutChambers();
                    //DigOutCavesStart();
                    _currentStep++;
                    break;
                case 6:
                    /*
                    for (int i = 0; i < _digCavesIterationsBetweenDraws; i++)
                    {
                        DigOutCavesIteration();
                        _digCavesIterations++;
                    }
                    if (_digCavesIterations >= _digCavesIterationsMax)
                        _currentStep++;
                     * */
                    DigOutCorridors();
                    _currentStep++;
                    break;
                case 7:
                    if (!AddStairs())
                    {
                        ResetGenerator();
                    }
                    else
                    {
                        _currentStep++;
                    }
                    break;
                case 8:
                    //Make grass
                    MakeLake(5,15);
                    _makeLakesIterations++;
                    if (_makeLakesIterations >= _makeLakesIterationsMax)
                    {
                        _currentStep++;
                    }
                    break;
                case 9:
                    SpecialRoomStep();
                    break;
                case 10:
                    if (!PlaceMonster())
                        _currentStep++;
                    break;
                case 11:
                    PlaceItem();
                    _currentStep++;
                    break;
                case 12:
                    _finished = true;
                    break;
                default:
                    break;
            }
        }

        #region Generator Steps

        private void CreateRooms()
        {
            
            int currentRoomListIndex = 0;
            for (int i = 0; i < (WIDTH-1) / (_roomSize.X-1); i++)
            {
                for (int k = 0; k < (HEIGHT-1) / (_roomSize.Y-1); k++)
                {
                    NewLevelGen_Room rm = new NewLevelGen_Room();
                    rm.TopLeftCorner = new IntVector2(i * (_roomSize.X-1), k * (_roomSize.Y-1));
                    rm.Size = new IntVector2(_roomSize);
                    _rooms.Add(rm);

                    //Add adjacencies
                    if (k > 0)
                    {
                        AddAdjacencyBetweenRooms(_rooms[currentRoomListIndex - 1], rm);
                    }
                    if (i > 0)
                    {
                        AddAdjacencyBetweenRooms(_rooms[currentRoomListIndex - HEIGHT/(_roomSize.X-1)], rm);
                    }

                    currentRoomListIndex++;
                }
            }
        }



        public void MergeRoomsIteration()
        {
            //Pick a random room
            int candidateIndex = Utility.random.Next(_rooms.Count);
            NewLevelGen_Room candidateRoom = _rooms[candidateIndex];

            //Get adjacent 
            List<NewLevelGen_Room> adjRooms = GetAdjacentRooms(candidateRoom);
            if (adjRooms.Count == 0)
                return;

            //Pick a random adjacent room
            bool mergeCandidateFound = false;
            int mergeCandidateIndex = Utility.random.Next(adjRooms.Count);
            NewLevelGen_Room mergeCandidate = adjRooms[mergeCandidateIndex];
            //A room needs to be A. the same dimensions and B. on the same X or Y axis to be considered for a merge
            if (candidateRoom.TopLeftCorner.X == mergeCandidate.TopLeftCorner.X || candidateRoom.TopLeftCorner.Y == mergeCandidate.TopLeftCorner.Y)
            {
                if (candidateRoom.Size.Equals(mergeCandidate.Size))
                    mergeCandidateFound = true;
            }

            if (mergeCandidateFound)
            {
                NewLevelGen_Room expandedRoom = candidateRoom;
                NewLevelGen_Room absorbedRoom = mergeCandidate;
                //Make sure we are keeping the room whose top left corner position will remain unchanged
                if (mergeCandidate.TopLeftCorner.X < candidateRoom.TopLeftCorner.X || mergeCandidate.TopLeftCorner.Y < candidateRoom.TopLeftCorner.Y)
                {
                    expandedRoom = mergeCandidate;
                    absorbedRoom = candidateRoom;
                }

                bool expandToTheRight = true; //Expand down if false
                if (expandedRoom.TopLeftCorner.X == absorbedRoom.TopLeftCorner.X)
                {
                    expandToTheRight = false;
                }

                if (expandToTheRight)
                {
                    expandedRoom.Size.X += absorbedRoom.Size.X-1;
                }
                else
                {
                    expandedRoom.Size.Y += absorbedRoom.Size.Y-1;
                }

                //Get adjacent rooms to absorbedRoom, they are adjacent to us now
                List<NewLevelGen_Room> absorbedAdjacencies = GetAdjacentRooms(absorbedRoom);
                foreach (NewLevelGen_Room rm in absorbedAdjacencies)
                {
                    if (rm != expandedRoom)
                    {
                        AddAdjacencyBetweenRooms(expandedRoom, rm);
                    }
                }

                RemoveRoomAndAdjacencies(absorbedRoom);
            }

        }

        private void ChooseStartAndEndRooms()
        {
            _startRoom = _rooms[Utility.random.Next(_rooms.Count)];
            _endRoom = _rooms[Utility.random.Next(_rooms.Count)];
            if (_startRoom.Equals(_endRoom))
                ChooseStartAndEndRooms();
            if (GetAdjacentRooms(_startRoom).Contains(_endRoom))
                ChooseStartAndEndRooms();
        }

        private void GetCriticalPathBetweenStartAndEndRooms()
        {
            _roomsToBeUsed = GetLeastWeightPathBetweenRooms(_startRoom, _endRoom);
            for (int i = 0; i < _roomsToBeUsed.Count - 1; i++)
            {
                _adjacenciesToBeUsed.Add(GetAdjacencyBetweenRooms(_roomsToBeUsed[i], _roomsToBeUsed[i + 1]));
                AddExitLocationsToRoomsUsingAdjacency(_adjacenciesToBeUsed.Last());
            }
        }

        private void AddRandomRoomToPathIteration()
        {
            //Get a random room thats already in our path
            NewLevelGen_Room roomToBranchFrom = _roomsToBeUsed[Utility.random.Next(_roomsToBeUsed.Count)];
            List<NewLevelGen_Room> neighbors = GetAdjacentRooms(roomToBranchFrom);
            NewLevelGen_Room roomToAdd = neighbors[Utility.random.Next(neighbors.Count)];

            if (!_roomsToBeUsed.Contains(roomToAdd))
            {
                _roomsToBeUsed.Add(roomToAdd);
                _adjacenciesToBeUsed.Add(GetAdjacencyBetweenRooms(roomToBranchFrom, roomToAdd));
                AddExitLocationsToRoomsUsingAdjacency(_adjacenciesToBeUsed.Last());
            }
            else if (!_adjacenciesToBeUsed.Contains(GetAdjacencyBetweenRooms(roomToBranchFrom, roomToAdd)))
            {
                _adjacenciesToBeUsed.Add(GetAdjacencyBetweenRooms(roomToBranchFrom, roomToAdd));
                AddExitLocationsToRoomsUsingAdjacency(_adjacenciesToBeUsed.Last());
            }
        }

        private void ClearOutUnusedRooms()
        {
            _rooms = _roomsToBeUsed;
            _roomAdjacencies = _adjacenciesToBeUsed;
        }

        private void DigOutCavesStart()
        {
            _floods = new List<IntVector2>();
            foreach (NewLevelGen_Room room in _rooms)
            {
                IntVector2 digTile = new IntVector2(room.TopLeftCorner);
                digTile.X += Utility.random.Next(room.Size.X/2);
                digTile.X += Utility.random.Next(room.Size.X / 2);
                digTile.Y += Utility.random.Next(room.Size.Y / 2);
                digTile.Y += Utility.random.Next(room.Size.Y / 2);

                _map[digTile.X, digTile.Y].SetFeature(Tile_SimpleFeatureType.FLOOR);
                _floods.Add(digTile);
            }
        }

        private void DigOutCavesIteration()
        {
            int index = Utility.random.Next(_floods.Count);
            IntVector2 floodTile = _floods[index];

            IntVector2 tileToCheck = new IntVector2(0,0);
            //0 = up, 1 = right, 2 = down, 3 = left
            int randomDirection = Utility.random.Next(4);
            if (randomDirection == 0)
                tileToCheck = new IntVector2(floodTile.X, floodTile.Y-1);
            else if (randomDirection == 1)
                tileToCheck = new IntVector2(floodTile.X+1, floodTile.Y);
            else if (randomDirection == 2)
                tileToCheck = new IntVector2(floodTile.X, floodTile.Y+1);
            else if (randomDirection == 3)
                tileToCheck = new IntVector2(floodTile.X-1, floodTile.Y);

            //Make sure tile is within bounds
            if (tileToCheck.X < 0 || tileToCheck.X >= WIDTH
                || tileToCheck.Y < 0 || tileToCheck.Y >= HEIGHT)
            {
                return;
            }

            if (_map[tileToCheck.X, tileToCheck.Y].GetLayer(TileLayer.BASE) == Tile_SimpleFeatureType.STONE_WALL)
            {
                _map[tileToCheck.X, tileToCheck.Y].SetFeature(Tile_SimpleFeatureType.FLOOR);
                _floods.Add(tileToCheck);
            }
        }

        private void DigOutChambers()
        {
            int minChamberDim = 4;
            int digsInEachRoom = 2;

            foreach (NewLevelGen_Room room in _rooms)
            {
                for (int d = 0; d < digsInEachRoom; d++)
                {
                    IntVector2 newChamberDims = new IntVector2();
                    newChamberDims.X = Utility.random.Next(minChamberDim, (room.Size.X - 1));
                    newChamberDims.X += Utility.random.Next(minChamberDim, (room.Size.X - 1));
                    newChamberDims.X /= 2;
                    newChamberDims.Y = Utility.random.Next(minChamberDim, (room.Size.Y - 1));
                    newChamberDims.Y += Utility.random.Next(minChamberDim, (room.Size.Y - 1));
                    newChamberDims.Y /= 2;

                    IntVector2 newChamberPos = new IntVector2(room.TopLeftCorner);
                    newChamberPos.X += Utility.random.Next(1, room.Size.X - newChamberDims.X);
                    newChamberPos.Y += Utility.random.Next(1, room.Size.Y - newChamberDims.Y);

                    for (int i = newChamberPos.X; i < newChamberPos.X + newChamberDims.X; i++)
                    {
                        for (int k = newChamberPos.Y; k < newChamberPos.Y + newChamberDims.Y; k++)
                        {
                            _map[i, k].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                    }
                }
            }
        }

        private void DigOutCorridors()
        {

            //We need to make sure each room's exits are connected within the room
            foreach (NewLevelGen_Room room in _rooms)
            {
                IntVector2 roomCenter = new IntVector2(room.TopLeftCorner);
                roomCenter += new IntVector2(room.Size.X / 2, room.Size.Y / 2);
                for (int i = 0; i < room.Exits.Count; i++)
                {
                    bool xFirst = true;
                    int xPos = room.Exits[i].X;
                    int yPos = room.Exits[i].Y;
                    _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);

                    if (yPos == room.TopLeftCorner.Y
                        || yPos == room.TopLeftCorner.Y + room.Size.Y - 1)
                        xFirst = false;

                    if (xFirst)
                    {
                        while (xPos > roomCenter.X)
                        {
                            xPos--;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                        while (xPos < roomCenter.X)
                        {
                            xPos++;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                        while (yPos > roomCenter.Y)
                        {
                            yPos--;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                        while (yPos < roomCenter.Y)
                        {
                            yPos++;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                    }
                    else
                    {
                        while (yPos > roomCenter.Y)
                        {
                            yPos--;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                        while (yPos < roomCenter.Y)
                        {
                            yPos++;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                        while (xPos > roomCenter.X)
                        {
                            xPos--;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                        while (xPos < roomCenter.X)
                        {
                            xPos++;
                            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.FLOOR);
                        }
                        
                    }
                }
            }
        }

        private bool PlaceMonster()
        {
            bool couldPlaceMonster = false;

            List<MonsterGenInfo> viableMonsters = GetViableMonsters(_depth, _monsterPointsForLevel - _monsterPointsUsed);
            if (viableMonsters.Count == 0)
                return false;

            int tries = 0;
            while (!couldPlaceMonster && tries < 10)
            {
                //Select a random room
                int roomToTryIndex = Utility.random.Next(_rooms.Count);
                NewLevelGen_Room roomToTry = _rooms[roomToTryIndex];

                //Get a random position in the room
                int xPos = roomToTry.TopLeftCorner.X;
                xPos += Utility.random.Next(1, roomToTry.Size.X / 2);
                xPos += Utility.random.Next(1, roomToTry.Size.X / 2);
                int yPos = roomToTry.TopLeftCorner.Y;
                yPos += Utility.random.Next(1, roomToTry.Size.Y / 2);
                yPos += Utility.random.Next(1, roomToTry.Size.Y / 2);

                if (!_map[xPos, yPos].ObstructsActors())
                {
                    //Get a random actor
                    int monsterIndex = Utility.random.Next(viableMonsters.Count());
                    ActorData a = new ActorData(xPos, yPos, viableMonsters[monsterIndex].monsterID);
                    _monsterPointsUsed += viableMonsters[monsterIndex].pointCost;
                    _monsters.Add(a);
                    couldPlaceMonster = true;
                }

                tries++;
            }



            return couldPlaceMonster;
        }

        public bool PlaceItem()
        {
            bool couldPlaceItem = false;

            string spellForSpellbook = GenerateRandomSpellbookForDepth(_depth);

            int tries = 0;
            while (!couldPlaceItem && tries < 10)
            {
                //Select a random room
                int roomToTryIndex = Utility.random.Next(_rooms.Count);
                NewLevelGen_Room roomToTry = _rooms[roomToTryIndex];

                //Get a random position in the room
                int xPos = roomToTry.TopLeftCorner.X;
                xPos += Utility.random.Next(1, roomToTry.Size.X / 2);
                xPos += Utility.random.Next(1, roomToTry.Size.X / 2);
                int yPos = roomToTry.TopLeftCorner.Y;
                yPos += Utility.random.Next(1, roomToTry.Size.Y / 2);
                yPos += Utility.random.Next(1, roomToTry.Size.Y / 2);

                if (!_map[xPos, yPos].ObstructsActors())
                {
                    //Place spellbook
                    ItemData i = new ItemData(xPos, yPos, "itemSpellbook");
                    i.parameter = spellForSpellbook;
                    _items.Add(i);
                    couldPlaceItem = true;
                }

                tries++;
            }

            return couldPlaceItem;
        }

        public bool AddStairs()
        {
            int tries = 0;
            int xPos;
            int yPos;
            do
            {
                xPos = _startRoom.TopLeftCorner.X;
                xPos += Utility.random.Next(1, _startRoom.Size.X / 2);
                xPos += Utility.random.Next(1, _startRoom.Size.X / 2);
                yPos = _startRoom.TopLeftCorner.Y;
                yPos += Utility.random.Next(1, _startRoom.Size.Y / 2);
                yPos += Utility.random.Next(1, _startRoom.Size.Y / 2);

                tries++;
            }
            while (_map[xPos, yPos].ObstructsActors() && tries < 10);

            if (tries == 10)
            {
                return false;
            }
            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.STAIRS_UP);
            _upStairs = new IntVector2(xPos, yPos);

            tries = 0;
            do
            {
                xPos = _endRoom.TopLeftCorner.X;
                xPos += Utility.random.Next(1, _endRoom.Size.X / 2);
                xPos += Utility.random.Next(1, _endRoom.Size.X / 2);
                yPos = _endRoom.TopLeftCorner.Y;
                yPos += Utility.random.Next(1, _endRoom.Size.Y / 2);
                yPos += Utility.random.Next(1, _endRoom.Size.Y / 2);

                tries++;
            }
            while (_map[xPos, yPos].ObstructsActors() && tries < 10);
            if (tries == 10)
            {
                return false;
            }
            _map[xPos, yPos].SetFeature(Tile_SimpleFeatureType.STAIRS_DOWN);
            _downStairs = new IntVector2(xPos, yPos);

            return true;
        }

        private void MakeLake(int sizeMin, int sizeMax)
        {
            bool success = false;

            int numOfTries = 0;

            while (!success && numOfTries < 10)
            {
                //Make a copy of the map to work from
                Tile[,] mapCopy = new Tile[WIDTH,HEIGHT];
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int k = 0; k < HEIGHT; k++)
                    {
                        mapCopy[i, k] = _map[i, k].Copy();
                    }
                }

                List<IntVector2> lakeTiles = new List<IntVector2>();

                //Pick a random spot
                int xStart = Utility.random.Next(WIDTH);
                int yStart = Utility.random.Next(HEIGHT);
                lakeTiles.Add(new IntVector2(xStart, yStart));

                int lakeSize = Utility.random.Next(sizeMin, sizeMax);

                for (int i = 0; i < lakeSize; i++)
                {
                    IntVector2 tileToBranch;
                    List<IntVector2> neighbors = new List<IntVector2>();
                    int branchTries = 0;
                    while (neighbors.Count == 0 && branchTries < 10000)
                    {
                        tileToBranch = lakeTiles[Utility.random.Next(lakeTiles.Count)];
                        neighbors = GetTileNeighbors(tileToBranch);
                        for (int k = 0; k < neighbors.Count; k++ )
                        {
                            IntVector2 n = neighbors[k];
                            if (lakeTiles.Contains(n))
                            {
                                neighbors.Remove(n);
                                k--;
                            }
                        }
                        branchTries++;
                    }
                    IntVector2 nextTile = neighbors[Utility.random.Next(neighbors.Count)];
                    lakeTiles.Add(nextTile);
                }

                //Now bake these tiles into the copy of the map
                foreach (IntVector2 pos in lakeTiles)
                {
                    if (pos.X > 0 && pos.X < WIDTH - 1 && pos.Y > 0 && pos.Y < HEIGHT - 1)
                    {
                        if (!mapCopy[pos.X, pos.Y].DoesTileHaveFeature(Tile_SimpleFeatureType.STAIRS_DOWN)
                            && !mapCopy[pos.X, pos.Y].DoesTileHaveFeature(Tile_SimpleFeatureType.STAIRS_UP))
                        {
                            mapCopy[pos.X, pos.Y].SetFeature(Tile_SimpleFeatureType.FLOOR);
                            mapCopy[pos.X, pos.Y].SetFeature(Tile_SimpleFeatureType.GRASS);
                            mapCopy[pos.X, pos.Y].RemoveFeatureFromLayer(TileLayer.LIQUID);
                        }
                    }
                }

                //Check for connectability
                List<IntVector2> connectivityResult = GetPathBetweenTiles(mapCopy, _upStairs, _downStairs);

                if (connectivityResult != null)
                {
                    success = true;
                    _map = mapCopy;
                }

                numOfTries++;
            }
        }

        private void SpecialRoomStep()
        {
            if (_branch == Dungeon_Branches.CELLAR && _depth == 2)
            {
                //We must make a stairway to the catacombs
                if (MakeCatacombsStairsRoom())
                {
                    _currentStep++;
                }
                else
                {
                    ResetGenerator();
                }
            }
            else
            {
                MakeSpecialRoom();
                _currentStep++;
            }

        }

        private bool MakeSpecialRoom()
        {

            bool success = false;
            int numOfTries = 0;

            while (!success && numOfTries < 10)
            {
                //Choose a room
                NewLevelGen_Room candidateRoom = _rooms[Utility.random.Next(_rooms.Count)];

                //Copy the map
                Tile[,] mapCopy = new Tile[WIDTH, HEIGHT];
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int k = 0; k < HEIGHT; k++)
                    {
                        mapCopy[i, k] = _map[i, k].Copy();
                    }
                }

                //Reference this map in a smaller map of the room, which will be passed to the generator
                Tile[,] roomMap = new Tile[candidateRoom.Size.X, candidateRoom.Size.Y];
                for (int i = 0; i < candidateRoom.Size.X; i++)
                {
                    for (int k = 0; k < candidateRoom.Size.Y; k++)
                    {
                        roomMap[i, k] = mapCopy[i + candidateRoom.TopLeftCorner.X, k + candidateRoom.TopLeftCorner.Y];
                    }
                }

                //Give this to a random generator
                //TODO: pick a generator in some sort of logical way
                bool genSuccess = NewLevelGen_SpecialRoomGenerator.TreasureRoomAGenerator(candidateRoom, roomMap);


                //Check for connectability
                List<IntVector2> connectivityResult = GetPathBetweenTiles(mapCopy, _upStairs, _downStairs);

                if (genSuccess && connectivityResult != null)
                {
                    success = true;
                    _map = mapCopy;
                }

                numOfTries++;
            }

            return success;
        }

        private bool MakeCatacombsStairsRoom()
        {

            bool success = false;
            int numOfTries = 0;

            while (!success && numOfTries < 10)
            {
                //Choose a room
                NewLevelGen_Room candidateRoom = _rooms[Utility.random.Next(_rooms.Count)];

                //Copy the map
                Tile[,] mapCopy = new Tile[WIDTH, HEIGHT];
                for (int i = 0; i < WIDTH; i++)
                {
                    for (int k = 0; k < HEIGHT; k++)
                    {
                        mapCopy[i, k] = _map[i, k].Copy();
                    }
                }

                //Reference this map in a smaller map of the room, which will be passed to the generator
                Tile[,] roomMap = new Tile[candidateRoom.Size.X, candidateRoom.Size.Y];
                for (int i = 0; i < candidateRoom.Size.X; i++)
                {
                    for (int k = 0; k < candidateRoom.Size.Y; k++)
                    {
                        roomMap[i, k] = mapCopy[i + candidateRoom.TopLeftCorner.X, k + candidateRoom.TopLeftCorner.Y];
                    }
                }

                //Give this to a random generator
                //TODO: pick a generator in some sort of logical way
                IntVector2 stairsLoc;
                bool genSuccess = NewLevelGen_SpecialRoomGenerator.CatacombsEntranceGenerator(candidateRoom, roomMap, out stairsLoc);


                //Check for connectability
                List<IntVector2> connectivityResult = GetPathBetweenTiles(mapCopy, _upStairs, _downStairs);
                if (genSuccess && connectivityResult != null)
                {
                    connectivityResult = GetPathBetweenTiles(mapCopy, _upStairs, stairsLoc);
                }

                if (genSuccess && connectivityResult != null)
                {
                    success = true;
                    _map = mapCopy;
                    _specialStairs = stairsLoc;
                }

                numOfTries++;
            }

            return success;
        }

        #endregion

        private void InitMapWithTileType(Tile_SimpleFeatureType tileType)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                for (int k = 0; k < HEIGHT; k++)
                {
                    _map[i, k] = new Tile(new IntVector2(i, k));
                    _map[i, k].SetFeature(Tile_SimpleFeatureType.STONE_WALL);
                    _map[i, k].SetFeature(tileType);
                }
            }
        }

        #region Room Helper Methods

        private void AddAdjacencyBetweenRooms(NewLevelGen_Room a, NewLevelGen_Room b)
        {
            int weight = _pathWeights[Utility.random.Next(_pathWeights.Length)];
            _roomAdjacencies.Add(new Tuple<NewLevelGen_Room,NewLevelGen_Room,int>(a,b,weight));
        }

        private List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>> GetAdjacenciesForRoom(NewLevelGen_Room room)
        {
            List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>> items = new List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>>();

            foreach (Tuple<NewLevelGen_Room, NewLevelGen_Room, int> adj in _roomAdjacencies)
            {
                if (adj.Item1.Equals(room) || adj.Item2.Equals(room))
                {
                    items.Add(adj);
                }
            }

            return items;
        }

        private Tuple<NewLevelGen_Room, NewLevelGen_Room, int> GetAdjacencyBetweenRooms(NewLevelGen_Room roomA, NewLevelGen_Room roomB)
        {
            Tuple<NewLevelGen_Room, NewLevelGen_Room, int> result = null;
            foreach (Tuple<NewLevelGen_Room, NewLevelGen_Room, int> adj in _roomAdjacencies)
            {
                if (adj.Item1 == roomA && adj.Item2 == roomB
                    || adj.Item1 == roomB && adj.Item2 == roomA)
                    result = adj;
            }

            return result;
        }

        private List<NewLevelGen_Room> GetAdjacentRooms(NewLevelGen_Room room)
        {
            List<NewLevelGen_Room> results = new List<NewLevelGen_Room>();
            List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>> adjs = GetAdjacenciesForRoom(room);

            foreach (Tuple<NewLevelGen_Room, NewLevelGen_Room, int> adj in adjs)
            {
                if (adj.Item1.Equals(room))
                {
                    results.Add(adj.Item2);
                }
                else
                {
                    results.Add(adj.Item1);
                }
            }

            return results;
        }

        private void RemoveRoomAndAdjacencies(NewLevelGen_Room room)
        {
            List<Tuple<NewLevelGen_Room, NewLevelGen_Room, int>> adjs = GetAdjacenciesForRoom(room);
            foreach (Tuple<NewLevelGen_Room, NewLevelGen_Room, int> adj in adjs)
            {
                _roomAdjacencies.Remove(adj);
            }

            _rooms.Remove(room);
        }

        private List<NewLevelGen_Room> GetLeastWeightPathBetweenRooms(NewLevelGen_Room source, NewLevelGen_Room target)
        {
            //Djikstra
            Dictionary<NewLevelGen_Room, int> distancesFromSource = new Dictionary<NewLevelGen_Room, int>(); //List of distance from the source to room X
            Dictionary<NewLevelGen_Room, NewLevelGen_Room> previousNodeFromSource = new Dictionary<NewLevelGen_Room, NewLevelGen_Room>(); //List of previous node in optimal path from source
            List<NewLevelGen_Room> unoptimizedRooms = new List<NewLevelGen_Room>(_rooms);

            foreach (NewLevelGen_Room room in _rooms)
            {
                distancesFromSource.Add(room, int.MaxValue);
                previousNodeFromSource.Add(room, null);
            }

            distancesFromSource[source] = 0; //Distance from source to itself is 0

            while (unoptimizedRooms.Count > 0)
            {
                //Get unoptimizedRooms entry with shortest distance in distancesFromSource
                int shortest = int.MaxValue;
                NewLevelGen_Room nextRoom = unoptimizedRooms[0];
                foreach (NewLevelGen_Room room in unoptimizedRooms)
                {
                    if (distancesFromSource[room] < shortest)
                    {
                        shortest = distancesFromSource[room];
                        nextRoom = room;
                    }
                }
                unoptimizedRooms.Remove(nextRoom);
                if (shortest == int.MaxValue) //If we haven't found a linked node
                {
                    return null; //No path exists!
                }

                foreach (Tuple<NewLevelGen_Room, NewLevelGen_Room, int> adj in GetAdjacenciesForRoom(nextRoom))
                {
                    NewLevelGen_Room neighbor = adj.Item1;
                    if (neighbor.Equals(nextRoom))
                        neighbor = adj.Item2;

                    int newDistance = distancesFromSource[nextRoom] + adj.Item3;
                    if (newDistance < distancesFromSource[neighbor])
                    {
                        distancesFromSource[neighbor] = newDistance;
                        previousNodeFromSource[neighbor] = nextRoom;
                    }
                }
            }

            List<NewLevelGen_Room> roomPath = new List<NewLevelGen_Room>();
            NewLevelGen_Room nextRoomInFinalPath = target;
            while (previousNodeFromSource[nextRoomInFinalPath] != null)
            {
                roomPath.Add(nextRoomInFinalPath);
                nextRoomInFinalPath = previousNodeFromSource[nextRoomInFinalPath];
            }
            roomPath.Add(source);
            roomPath.Reverse();
            return roomPath;
        }

        

        private void AddExitLocationsToRoomsUsingAdjacency(Tuple<NewLevelGen_Room, NewLevelGen_Room, int> adj)
        {
            List<IntVector2> boundary = GetBoundaryBetweenRooms(adj.Item1, adj.Item2);
            boundary.Sort((item, item2) => { return item.X.CompareTo(item2.X); });
            boundary.Sort((item, item2) => { return item.Y.CompareTo(item2.Y); });

            int index = Utility.random.Next(1, boundary.Count/2);
            index += Utility.random.Next(1, boundary.Count/2);

            adj.Item1.Exits.Add(boundary[index]);
            adj.Item2.Exits.Add(boundary[index]);

        }

        private List<IntVector2> GetBoundaryBetweenRooms(NewLevelGen_Room roomA, NewLevelGen_Room roomB)
        {
            List<IntVector2> results = new List<IntVector2>();
            List<IntVector2> roomAEdges = roomA.GetEdges();
            foreach (IntVector2 position in roomAEdges)
            {
                if (DoesRoomContainPosition(roomB, position))
                {
                    results.Add(position);
                }
            }

            return results;
        }

        private bool DoesRoomContainPosition(NewLevelGen_Room room, IntVector2 position)
        {
            bool result = true;
            if (position.X < room.TopLeftCorner.X || position.X >= room.TopLeftCorner.X + room.Size.X)
                result = false;
            if (position.Y < room.TopLeftCorner.Y || position.Y >= room.TopLeftCorner.Y + room.Size.Y)
                result = false;

            return result;
        }

        #endregion

        #region Tile helper methods

        private static List<IntVector2> GetTileNeighbors(IntVector2 tile)
        {
            List<IntVector2> result = new List<IntVector2>();
            if (tile.X > 0)
                result.Add(new IntVector2(tile) + new IntVector2(-1, 0));
            if (tile.X < WIDTH-1)
                result.Add(new IntVector2(tile) + new IntVector2(1, 0));
            if (tile.Y > 0)
                result.Add(new IntVector2(tile) + new IntVector2(0, -1));
            if (tile.Y < HEIGHT-1)
                result.Add(new IntVector2(tile) + new IntVector2(0, 1));

            return result;
        }

        private static List<IntVector2> GetPathBetweenTiles(Tile[,] map, IntVector2 source, IntVector2 target)
        {
            //Djikstra
            Dictionary<IntVector2, int> distancesFromSource = new Dictionary<IntVector2, int>(); //List of distance from the source to tile X
            Dictionary<IntVector2, IntVector2> previousNodeFromSource = new Dictionary<IntVector2, IntVector2>(); //List of previous node in optimal path from source
            List<IntVector2> unoptimizedTiles = new List<IntVector2>();
            for (int i = 0; i < WIDTH; i++)
                for (int k = 0; k < HEIGHT; k++)
                    unoptimizedTiles.Add(new IntVector2(i, k));

            foreach (IntVector2 pos in unoptimizedTiles)
            {
                distancesFromSource.Add(pos, int.MaxValue);
                previousNodeFromSource.Add(pos, null);
                if (pos.Equals(source))
                    distancesFromSource[pos] = 0; //Distance from source to itself is 0
            }

            bool pathFound = false;
            while (unoptimizedTiles.Count > 0 && !pathFound)
            {
                //Get unoptimizedTiles entry with shortest distance in distancesFromSource
                int shortest = int.MaxValue;
                IntVector2 nextTile = unoptimizedTiles[0];
                foreach (IntVector2 tile in unoptimizedTiles)
                {
                    if (distancesFromSource[tile] < shortest)
                    {
                        shortest = distancesFromSource[tile];
                        nextTile = tile;
                    }
                }
                
                if (shortest == int.MaxValue) //If we haven't found a linked node
                {
                    return null; //No path exists!
                }
                unoptimizedTiles.Remove(nextTile);
                //Display.SetBackColorOfPos(nextTile, ColorCatalog.lakeColor);
                //Display.FlushScreen();
                if (nextTile.Equals(target))
                {
                    pathFound = true;
                }

                foreach (IntVector2 n in GetTileNeighbors(nextTile))
                {
                    IntVector2 neighbor = n;
                    bool tileFound = false;
                    foreach (IntVector2 t in unoptimizedTiles)
                    {
                        if (t.Equals(neighbor))
                        {
                            tileFound = true;
                            neighbor = t;
                            break;
                        }
                    }
                    if (tileFound)
                    {
                        if (map[neighbor.X, neighbor.Y].ObstructsActors())
                        {
                            unoptimizedTiles.Remove(neighbor);
                        }
                        else
                        {
                            int newDistance = distancesFromSource[nextTile] + 1;
                            if (newDistance < distancesFromSource[neighbor])
                            {
                                distancesFromSource[neighbor] = newDistance;
                                previousNodeFromSource[neighbor] = nextTile;
                            }
                        }
                    }
                }
            }

            List<IntVector2> tilePath = new List<IntVector2>();
            IntVector2 nextTileInFinalPath = target;
            foreach (IntVector2 pos in previousNodeFromSource.Keys)
            {
                if (nextTileInFinalPath.Equals(pos))
                    nextTileInFinalPath = pos;
            }
            while (previousNodeFromSource[nextTileInFinalPath] != null)
            {
                tilePath.Add(nextTileInFinalPath);
                nextTileInFinalPath = previousNodeFromSource[nextTileInFinalPath];
            }
            tilePath.Add(source);
            tilePath.Reverse();
            return tilePath;
        }

        #endregion

        public List<NewLevelGen_Room> GetRooms()
        {
            return _rooms;
        }

        public List<ActorData> GetMonsters()
        {
            return _monsters;
        }

        #region Tile data accessors

        public char GetDisplayCharOfTile(IntVector2 position)
        {
            return _map[position.X, position.Y].GetSprite();
        }

        public Color GetDisplayCharColorOfTile(IntVector2 position)
        {
            return _map[position.X, position.Y].GetSpriteColor();
        }

        #endregion

        #region Monster generation info

        struct MonsterGenInfo
        {
            public string monsterID;
            public int minDepth;
            public int maxDepth;
            public int spawnChance;
            public int pointCost;

            public MonsterGenInfo(string monsterID, int minDepth, int maxDepth, int spawnChance, int pointCost)
            {
                this.monsterID = monsterID;
                this.minDepth = minDepth;
                this.maxDepth = maxDepth;
                this.spawnChance = spawnChance;
                this.pointCost = pointCost;
            }
        }

        static MonsterGenInfo[] MonsterGenCatalog = 
        {
                                //ID                    minLevel, maxLevel, spawnChance, points
            new MonsterGenInfo("enemy_dog",             1, 2, 10, 4),
            new MonsterGenInfo("enemy_wisp",            1, 2, 10, 4),
            new MonsterGenInfo("enemy_gopherkinScrub",  2, 4, 10, 6),
            new MonsterGenInfo("enemy_spectralPupil",   2, 4, 10, 6),
            new MonsterGenInfo("enemy_bluecap",         2, 4, 10, 6),
            new MonsterGenInfo("enemy_gopherkinAdept",  3, 5, 10, 8),
            new MonsterGenInfo("enemy_quazaxRocketeer", 3, 7, 10, 8)
        };

        private List<MonsterGenInfo> GetViableMonsters(int depth, int pointsLeft)
        {
            List<MonsterGenInfo> viableMonsters = new List<MonsterGenInfo>();

            foreach (MonsterGenInfo m in MonsterGenCatalog)
            {
                if (m.minDepth <= depth && m.maxDepth >= depth && m.pointCost <= pointsLeft)
                    viableMonsters.Add(m);
            }

            return viableMonsters;
        }

        #endregion

        #region Item generation info

        private string GenerateRandomSpellbookForDepth(int depth)
        {
            //TODO
            List<string> spells = Spell.GetAllSpellIDs().OrderBy(a => rand.Next()).ToList();
            return spells[0];
        }

        #endregion

    }
}
