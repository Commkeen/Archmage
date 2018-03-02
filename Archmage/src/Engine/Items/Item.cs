using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.Engine.DataStructures;

namespace Archmage.Engine.Items
{
    class Item:GameObject
    {

        public enum ItemType { POTION, SCROLL, ARTIFACT, SOUL };
        public ItemType Type { get; protected set; }

        public string Name 
        {
            get
            {
                if (!IsIdentified(ObjectID))
                {
                    return GetUnidentifiedName(ObjectID);
                }

                return RealName;
            }
        }
        public string RealName { get; protected set; }
        public string Description { get; protected set; }
        public char Sprite { get; protected set; }
        public Color SpriteColor
        {
            get
            {
                if (PermanentColor == null)
                    return GetColor(ObjectID);
                return PermanentColor;
            }
        }
        protected Color PermanentColor { get; set; }

        public Item(string objID, int instanceID)
            : base(objID, instanceID)
        {
            Type = ItemType.ARTIFACT;
            RealName = "Generic Item";
            Description = "A very undefined item.";
            Sprite = '?';
            PermanentColor = null;
        }

        public virtual bool OnUse(Actor a)
        {
            _scene.WriteMessage("This item is far to generic to use in any meaningful way.");
            return false;
        }


        #region Identification system

        static List<string> itemIDs;
        static List<string> unidentifiedNames;
        static List<Color> colors;
        static List<bool> identificationStatus;


        #region Initialization
        public static void InitItemIdentifications()
        {
            itemIDs = new List<string>();
            unidentifiedNames = new List<string>();
            identificationStatus = new List<bool>();
            colors = new List<Color>();

            List<string> tonicIDs = GetTonicIDs();
            List<string> tonicNames = GetRandomTonicUnidentifiedNames();
            itemIDs.AddRange(tonicIDs);
            unidentifiedNames.AddRange(tonicNames.Take(tonicIDs.Count));

            List<string> liquorIDs = GetLiquorIDs();
            List<string> liquorNames = GetRandomLiquorUnidentifiedNames();
            itemIDs.AddRange(liquorIDs);
            unidentifiedNames.AddRange(liquorNames.Take(liquorIDs.Count));

            colors.AddRange(GetRandomPotionColors());

            for (int i = 0; i < itemIDs.Count; i++)
            {
                identificationStatus.Add(false);
            }
        }
        #endregion

        #region Accessors

        public static bool IsIdentified(string itemID)
        {
            for (int i = 0; i < itemIDs.Count; i++)
            {
                if (itemIDs[i] == itemID)
                    return identificationStatus[i];
            }

            return true;
        }

        public static string GetUnidentifiedName(string itemID)
        {
            for (int i = 0; i < itemIDs.Count; i++)
            {
                if (itemIDs[i] == itemID)
                    return unidentifiedNames[i];
            }

            return "";
        }

        public static Color GetColor(string itemID)
        {
            for (int i = 0; i < itemIDs.Count; i++)
            {
                if (itemIDs[i] == itemID)
                    return colors[i];
            }

            return new Color(libtcod.TCODColor.white);
        }

        public static void Identify(string itemID)
        {
            for (int i = 0; i < itemIDs.Count; i++)
            {
                if (itemIDs[i] == itemID)
                    identificationStatus[i] = true;
            }
        }



        #endregion

        #endregion

        #region Lists of items
        //This should all be replaced with a more data driven approach obviously

        public static List<string> GetTonicIDs()
        {
            List<string> ids = new List<string>();
            ids.Add("tonic_healthSmall");
            ids.Add("tonic_essenceSmall");
            //ids.Add("tonic_speedSmall");
            //ids.Add("tonic_focusSmall");
            return ids;
        }

        public static List<string> GetLiquorIDs()
        {
            List<string> ids = new List<string>();
            ids.Add("liquor_healthMedium");
            ids.Add("liquor_essenceMedium");
            ids.Add("liquor_displacement");
            ids.Add("liquor_forgetMap");
            ids.Add("liquor_booze");
            ids.Add("liquor_xray");
            return ids;
        }

        public static List<string> GetRandomTonicUnidentifiedNames()
        {
            List<string> names = new List<string>();
            names.Add("strawberry sarsaparilla");
            names.Add("original flavor sarsaparilla");
            names.Add("apple sarsaparilla");
            names.Add("cherry sarsaparilla");
            names.Add("snozzberry sarsaparilla");
            names.Add("sarsaparilla lite");
            names.Add("new and improved sarsaparilla");
            names.Add("water-flavored sarsaparilla");
            Random r = new Random();
            names = names.OrderBy(a => r.Next()).ToList();
            return names;
        }

        public static List<string> GetRandomLiquorUnidentifiedNames()
        {
            List<string> names = new List<string>();
            names.Add("moonshine");
            names.Add("spiced rum");
            names.Add("nasty-looking grog");
            names.Add("rubbing alcohol");
            names.Add("cheap wine");
            names.Add("expensive-looking champagne");
            names.Add("gin");
            names.Add("absinthe");
            Random r = new Random();
            names = names.OrderBy(a => r.Next()).ToList();
            return names;
        }

        public static List<Color> GetRandomPotionColors()
        {
            List<Color> colors = new List<Color>();
            colors.Add(new Color(libtcod.TCODColor.red));
            colors.Add(new Color(libtcod.TCODColor.blue));
            colors.Add(new Color(libtcod.TCODColor.orange));
            colors.Add(new Color(libtcod.TCODColor.peach));
            colors.Add(new Color(libtcod.TCODColor.lime));
            colors.Add(new Color(libtcod.TCODColor.pink));
            colors.Add(new Color(libtcod.TCODColor.purple));
            colors.Add(new Color(libtcod.TCODColor.sea));
            colors.Add(new Color(libtcod.TCODColor.turquoise));
            colors.Add(new Color(libtcod.TCODColor.celadon));
            colors.Add(new Color(libtcod.TCODColor.copper));
            Random r = new Random();
            colors = colors.OrderBy(a => r.Next()).ToList();
            return colors;
        }

        #endregion

    }
}
