using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.Engine.Spells
{
    class Discipline
    {
        protected List<string> _spells;
        protected List<int> _tierNum;

        public Discipline()
        {
            _spells = new List<string>();
            _tierNum = new List<int>();
        }

        public List<string> GetSpells(int tier)
        {
            List<string> spellList = new List<string>();
            for (int i = 0; i < _spells.Count; i++)
            {
                if (_tierNum[i] == tier)
                    spellList.Add(_spells[i]);
            }

            return spellList;
        }
    }
}
