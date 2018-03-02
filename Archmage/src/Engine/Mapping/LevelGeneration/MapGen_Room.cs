using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.DataStructures;
using Archmage.GameData;

namespace Archmage.Mapping.LevelGeneration
{
    /// <summary>
    /// Represents a rectangular basic room in map generation
    /// This room can have Actors and Items in it
    /// </summary>
    class MapGen_Room
    {
        public IntVector2 Position { get; set; }
        public IntVector2 Size { get; set; }

        public string Type { get; set; }

        public List<ActorData> Actors { get; set; }
        public List<ItemData> Items { get; set; }

        public MapGen_Room(IntVector2 pos, IntVector2 size, string type)
        {
            Position = pos;
            Size = size;
            Type = type;

            Actors = new List<ActorData>();
            Items = new List<ItemData>();
        }
    }
}
