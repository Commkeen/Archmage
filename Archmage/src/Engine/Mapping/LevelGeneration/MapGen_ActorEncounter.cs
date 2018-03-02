using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.Mapping.LevelGeneration
{
    class MapGen_ActorEncounter
    {
        public List<string> Actors { get; set; }
        public int Points { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }

        public MapGen_ActorEncounter()
        {
            Actors = new List<string>();
            Points = 0;
            MinLevel = 1;
            MaxLevel = 10;
        }
    }
}
