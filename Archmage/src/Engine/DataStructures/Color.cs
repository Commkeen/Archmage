using Archmage.Engine.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage.Engine.DataStructures
{
    public class Color
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public Color()
        {
            R = 255;
            G = 255;
            B = 255;
        }

        public Color(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;

            R = Math.Min(R, 255);
            R = Math.Max(R, 0);
            G = Math.Min(G, 255);
            G = Math.Max(G, 0);
            B = Math.Min(B, 255);
            B = Math.Max(B, 0);
        }

        public Color(libtcod.TCODColor libColor)
        {
            R = libColor.Red;
            G = libColor.Green;
            B = libColor.Blue;
        }

        public Color Tint(Color tintColor, float weightOfOriginalColor)
        {
            if (weightOfOriginalColor > 1.0f)
                weightOfOriginalColor = 1.0f;
            if (weightOfOriginalColor < 0f)
                weightOfOriginalColor = 0f;

            float rValue = ((float)this.R * weightOfOriginalColor) + ((float)tintColor.R * (1.0f - weightOfOriginalColor));
            float gValue = ((float)this.G * weightOfOriginalColor) + ((float)tintColor.G * (1.0f - weightOfOriginalColor));
            float bValue = ((float)this.B * weightOfOriginalColor) + ((float)tintColor.B * (1.0f - weightOfOriginalColor));

            Color result = new Color((int)rValue, (int)gValue, (int)bValue);

            return result;
        }

        public Color Randomize(Color randomColor)
        {
            return Randomize(randomColor.R, randomColor.G, randomColor.B);
        }

        public Color Randomize(int rAmount, int gAmount, int bAmount)
        {
            int rValue = this.R - (rAmount / 2) + Utility.random.Next(rAmount);
            int gValue = this.G - (gAmount / 2) + Utility.random.Next(gAmount);
            int bValue = this.B - (bAmount / 2) + Utility.random.Next(bAmount);

            Color result = new Color((int)rValue, (int)gValue, (int)bValue);

            return result;
        }

        public libtcod.TCODColor ToTCODColor()
        {
            return new libtcod.TCODColor(this.R, this.G, this.B);
        }
    }

    public static class ColorCatalog
    {
        public static Color normalFloorColor = new Color(libtcod.TCODColor.sepia);
        public static Color normalWallColor = new Color(libtcod.TCODColor.darkGrey);
        public static Color lakeColor = new Color(libtcod.TCODColor.blue);
        public static Color specialStairsColor = new Color(libtcod.TCODColor.red);
        public static Color grassColor = new Color(libtcod.TCODColor.darkestGreen);

        public static Color basicRandomColor = new Color(20, 20, 20);
        public static Color randomGrassColor = new Color(0, 100, 0);
    }
}
