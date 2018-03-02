using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Spells;


namespace Archmage.Content.Spells
{
    class Discipline_Shadow:Discipline
    {

        public Discipline_Shadow()
            : base()
        {
            _spells.Add("infect");
            _tierNum.Add(0);

            _spells.Add("blink");
            _tierNum.Add(1);

            _spells.Add("darkArrow");
            _tierNum.Add(1);

            _spells.Add("sleep");
            _tierNum.Add(2);

            _spells.Add("stoneToGlass");
            _tierNum.Add(2);

            _spells.Add("pain");
            _tierNum.Add(3);

            _spells.Add("touchOfDeath");
            _tierNum.Add(4);
        }

    }
}
