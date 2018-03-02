using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Spells;


namespace Archmage.Content.Spells
{
    class Discipline_Debug:Discipline
    {

        public Discipline_Debug()
            : base()
        {
            _spells.Add("xrayVision");
            _tierNum.Add(0);

            _spells.Add("invincibility");
            _tierNum.Add(0);
        }

    }
}
