using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.GameData
{
    public class ItemData
    {
        public int x;
        public int y;

        public string itemName;
        public string parameter; //Right now this is only for spellbook spells

        public ItemData(int x, int y, string itemType)
        {
            this.x = x;
            this.y = y;
            this.itemName = itemType;
            parameter = "";
        }
    }
}
