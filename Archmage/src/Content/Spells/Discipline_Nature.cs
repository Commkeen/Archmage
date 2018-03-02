using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Spells;


namespace Archmage.Content.Spells
{
    class Discipline_Nature:Discipline
    {

        public Discipline_Nature()
            : base()
        {
            _spells.Add("scaledSkin");
            _tierNum.Add(0);

            _spells.Add("gravelShot");
            _tierNum.Add(0);

            _spells.Add("hurlSpines");
            _tierNum.Add(1);

            _spells.Add("slicingWind");
            _tierNum.Add(2);

            _spells.Add("lightningBolt");
            _tierNum.Add(3);

            _spells.Add("meteor");
            _tierNum.Add(4);
        }

    }
}
