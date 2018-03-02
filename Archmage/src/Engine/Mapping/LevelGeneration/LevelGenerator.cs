using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.GameData;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Items;
using Archmage.Engine.Spells;

namespace Archmage.Mapping.LevelGeneration
{
    public class LevelGenerator
    {

        public static int WIDTH = 50;
        public static int HEIGHT = 30;

        Random rand;

        public LevelGenerator()
        {
            rand = new Random();
        }

        private List<MapGen_ActorEncounter> GetAllEncounterData()
        {
            MapGen_ActorEncounter e;
            List<MapGen_ActorEncounter> encounters = new List<MapGen_ActorEncounter>();

            e = new MapGen_ActorEncounter();
            e.MinLevel = 1;
            e.MaxLevel = 1;
            e.Points = 1;
            e.Actors.Add("enemy_dog");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 1;
            e.MaxLevel = 1;
            e.Points = 1;
            e.Actors.Add("enemy_wisp");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 1;
            e.MaxLevel = 2;
            e.Points = 3;
            e.Actors.Add("enemy_dog");
            e.Actors.Add("enemy_dog");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 1;
            e.MaxLevel = 2;
            e.Points = 3;
            e.Actors.Add("enemy_wisp");
            e.Actors.Add("enemy_wisp");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 1;
            e.MaxLevel = 2;
            e.Points = 3;
            e.Actors.Add("enemy_wisp");
            e.Actors.Add("enemy_wisp");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 2;
            e.MaxLevel = 2;
            e.Points = 3;
            e.Actors.Add("enemy_gopherkinScrub");
            e.Actors.Add("enemy_dog");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 2;
            e.MaxLevel = 3;
            e.Points = 4;
            e.Actors.Add("enemy_gopherkinScrub");
            e.Actors.Add("enemy_gopherkinScrub");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 2;
            e.MaxLevel = 2;
            e.Points = 3;
            e.Actors.Add("enemy_spectralPupil");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 2;
            e.MaxLevel = 2;
            e.Points = 3;
            e.Actors.Add("enemy_bluecap");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 2;
            e.MaxLevel = 2;
            e.Points = 4;
            e.Actors.Add("enemy_spectralPupil");
            e.Actors.Add("enemy_wisp");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 2;
            e.MaxLevel = 2;
            e.Points = 4;
            e.Actors.Add("enemy_bluecap");
            e.Actors.Add("enemy_wisp");
            e.Actors.Add("enemy_wisp");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 3;
            e.MaxLevel = 4;
            e.Points = 5;
            e.Actors.Add("enemy_gopherkinAdept");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 3;
            e.MaxLevel = 4;
            e.Points = 10;
            e.Actors.Add("enemy_orcBrute");
            e.Actors.Add("enemy_orcBrute");
            e.Actors.Add("enemy_gopherkinAdept");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 3;
            e.MaxLevel = 3;
            e.Points = 10;
            e.Actors.Add("enemy_orcBrute");
            e.Actors.Add("enemy_dog");
            e.Actors.Add("enemy_dog");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 3;
            e.MaxLevel = 4;
            e.Points = 10;
            e.Actors.Add("enemy_goblinTinker");
            e.Actors.Add("enemy_orcBrute");
            e.Actors.Add("enemy_orcBrute");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 4;
            e.MaxLevel = 4;
            e.Points = 13;
            e.Actors.Add("enemy_goblinTinker");
            e.Actors.Add("enemy_armoredWorg");
            e.Actors.Add("enemy_armoredWorg");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 4;
            e.MaxLevel = 5;
            e.Points = 12;
            e.Actors.Add("enemy_armoredWorg");
            e.Actors.Add("enemy_armoredWorg");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 5;
            e.MaxLevel = 5;
            e.Points = 13;
            e.Actors.Add("enemy_delver");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 5;
            e.MaxLevel = 6;
            e.Points = 13;
            e.Actors.Add("enemy_restlessSpirit");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 7;
            e.MaxLevel = 7;
            e.Points = 13;
            e.Actors.Add("enemy_minotaur");
            encounters.Add(e);

            e = new MapGen_ActorEncounter();
            e.MinLevel = 8;
            e.MaxLevel = 8;
            e.Points = 13;
            e.Actors.Add("enemy_zealot");
            encounters.Add(e);

            return encounters;
        }

        public LevelData GenerateLevel(int branch, int depth)
        {
            LevelData level = new LevelData(WIDTH, HEIGHT, branch, depth);

            BSPMapGenerator gen = new BSPMapGenerator();
            List<MapGen_Room> rooms = gen.GenerateMap(level);

            List<MapGen_ActorEncounter> encounterList = GetAllEncounterData();

            //Calculate points for the level based on depth
            int maxPoints = 25 + depth*7;
            int currentPoints = 0;

            

            //If this is the 8th floor, place the amulet
            if (depth == 6)
            {
                bool amuletPlaced = false;
                while (!amuletPlaced)
                {
                    int roomIndex = rand.Next(rooms.Count);
                    amuletPlaced = PlaceItemInRoom("amuletSealing", rooms[roomIndex]);
                }
            }

            //If this is the 10th floor, make the stairs a rift to the Plane of Fire
            /*
            if (depth == 8)
            {
                level.downstairsTile.tileType = "fireRift";
            }
             * */

            //If this is the 1st floor, take out the stairs and doors
            /*
            if (depth == 1)
            {
                level.upstairsTile.tileType = "exit";

                foreach (TileData t in level.tiles)
                {
                    if (t.tileType == "door")
                        t.tileType = "floor";
                }
            }
             * */

            //Populate the rooms with actors
            foreach (MapGen_Room r in rooms)
            {
                if (r.Type != "upstairsroom") //Don't put monsters in the first room
                {
                    if (currentPoints < maxPoints)
                    {
                        //Get a random encounter from the list
                        int index = 0;
                        do
                        {
                            index = rand.Next(encounterList.Count);
                        }
                        while (encounterList[index].MinLevel > depth || encounterList[index].MaxLevel < depth);

                        if (encounterList[index].Points <= maxPoints - currentPoints)
                        {
                            foreach (string actor in encounterList[index].Actors)
                            {
                                PlaceActorInRoom(actor, r);
                            }
                            currentPoints += encounterList[index].Points;
                        }
                    }
                }
            }

            //Populate the rooms with items
            foreach (MapGen_Room r in rooms)
            {
                string itemID;
                PlaceItemInRoom("itemSpellbook", GenerateRandomSpellbookForDepth(depth), r);
                //10% chance of potion
                int dieRoll = rand.Next(10);
                if (dieRoll < 2)
                {
                    itemID = Item.GetTonicIDs().OrderBy(a => rand.Next()).First();
                    PlaceItemInRoom(itemID, r);
                }
                else if (dieRoll < 3)
                {
                    PlaceItemInRoom("soul", r);
                }
            }

            //Put a spellbook in one room
            //MapGen_Room rm = rooms[rand.Next(rooms.Count)];
            //PlaceItemInRoom("itemSpellbook", GenerateRandomSpellbookForDepth(depth), rm);

            //Create a list of actors for this floor

            foreach (MapGen_Room r in rooms)
            {
                foreach (ActorData a in r.Actors)
                {
                    ActorData actor = new ActorData(a.x + r.Position.X, a.y + r.Position.Y, a.actorType);
                    level.actors.Add(actor);
                }
                foreach (ItemData i in r.Items)
                {
                    ItemData item = new ItemData(i.x + r.Position.X, i.y + r.Position.Y, i.itemName);
                    item.parameter = i.parameter;
                    level.items.Add(item);
                }
            }

            //ActorData dragon = new ActorData(level.downstairsTile.x, level.downstairsTile.y, "enemy_mechanodragon");
            //level.actors.Add(dragon);
            return level;
        }

        private bool PlaceActorInRoom(string actorType, MapGen_Room room)
        {
            int attempts = 0;
            bool actorPlaced = false;

            while (attempts < 50 && !actorPlaced)
            {
                int tryX = rand.Next(1, room.Size.X - 1);
                int tryY = rand.Next(1, room.Size.Y - 1);

                if (CheckIfLocationIsFreeOfActors(new IntVector2(tryX, tryY), room))
                {
                    room.Actors.Add(new ActorData(tryX, tryY, actorType));
                    actorPlaced = true;
                }
                attempts++;
            }

            return actorPlaced;
        }

        private bool CheckIfLocationIsFreeOfActors(IntVector2 location, MapGen_Room room)
        {
            bool locationFree = true;
            foreach (ActorData a in room.Actors)
            {
                if (a.x == location.X && a.y == location.Y)
                    locationFree = false;
            }

            return locationFree;
        }

        private bool PlaceItemInRoom(string itemType, MapGen_Room room)
        {
            return PlaceItemInRoom(itemType, "", room);
        }

        private bool PlaceItemInRoom(string itemType, string parameter, MapGen_Room room)
        {
            int attempts = 0;
            bool itemPlaced = false;

            while (attempts < 50 && !itemPlaced)
            {
                int tryX = rand.Next(1, room.Size.X - 1);
                int tryY = rand.Next(1, room.Size.Y - 1);

                if (CheckIfLocationIsFreeOfItems(new IntVector2(tryX, tryY), room))
                {
                    ItemData data = new ItemData(tryX, tryY, itemType);
                    data.parameter = parameter;
                    room.Items.Add(data);
                    itemPlaced = true;
                }
                attempts++;
            }

            return itemPlaced;
        }

        private string GenerateRandomSpellbookForDepth(int depth)
        {
            //TODO
            List<string> spells = Spell.GetAllSpellIDs().OrderBy(a => rand.Next()).ToList();
            return spells[0];
        }

        private bool CheckIfLocationIsFreeOfItems(IntVector2 location, MapGen_Room room)
        {
            bool locationFree = true;
            foreach (ItemData a in room.Items)
            {
                if (a.x == location.X && a.y == location.Y)
                    locationFree = false;
            }

            return locationFree;
        }
    }
}
