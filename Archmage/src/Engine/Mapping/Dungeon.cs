using Archmage.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Mapping
{
    public enum Dungeon_Branches
    {
        CELLAR,
        CATACOMBS,
        LIBRARY,
        RESEARCH_VAULTS,
        FUNGUS_CAVES,

        PLANE_OF_ABYSS,
        PLANE_OF_WILD,
        PLANE_OF_STORM,
        PLANE_OF_FIRE
    }

    class Dungeon
    {
        List<LevelData> _levels;
        List<Tuple<Dungeon_Branches, int>> _locations;

        public Dungeon()
        {
            _levels = new List<LevelData>();
            _locations = new List<Tuple<Dungeon_Branches, int>>();
        }

        public LevelData LoadLevel(Dungeon_Branches branch, int depth)
        {
            //TODO: This should really load a COPY, not the actual reference cause that might cause weird behavior?
            LevelData lvl = null;
            int indexOfLevel = GetIndexOfLocation(branch, depth);
            if (indexOfLevel >= 0)
                lvl = _levels[indexOfLevel];
            return lvl;
        }

        public void SaveLevel(Dungeon_Branches branch, int depth, LevelData level)
        {
            int indexOfLevel = GetIndexOfLocation(branch, depth);
            if (indexOfLevel >= 0)
            {
                _levels[indexOfLevel] = level;
            }
            else
            {
                _levels.Add(level);
                _locations.Add(new Tuple<Dungeon_Branches, int>(branch, depth));
            }
        }

        private int GetIndexOfLocation(Dungeon_Branches branch, int depth)
        {
            for (int i = 0; i < _locations.Count; i++)
            {
                if (_locations[i].Item1 == branch && _locations[i].Item2 == depth)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
