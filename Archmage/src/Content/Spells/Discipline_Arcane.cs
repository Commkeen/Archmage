using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Spells;


namespace Archmage.Content.Spells
{
    class Discipline_Arcane:Discipline
    {

        public Discipline_Arcane()
            : base()
        {
            _spells.Add("magicDart");
            _tierNum.Add(0);

            _spells.Add("arcaneShield");
            _tierNum.Add(0);

            _spells.Add("magicMissile");
            _tierNum.Add(1);

            _spells.Add("photonSpike");
            _tierNum.Add(1);

            _spells.Add("astralConduit");
            _tierNum.Add(2);

            _spells.Add("energize");
            _tierNum.Add(2);

            _spells.Add("dispel");
            _tierNum.Add(3);

            _spells.Add("quickshot");
            _tierNum.Add(3);

            _spells.Add("arcaneBlast");
            _tierNum.Add(4);

            _spells.Add("boundSpirit");
            _tierNum.Add(4);
        }

    }
}
