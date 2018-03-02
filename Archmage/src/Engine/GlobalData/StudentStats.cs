using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.Engine.GlobalData
{
    class StudentStats
    {
        //0=arcane, 1=nature, 2=shadow
        //TODO: Make an enum or something I guess
        public int ChosenDiscipline { get; set; }

        public StudentStats()
        {
            ChosenDiscipline = 0;
        }
    }
}
