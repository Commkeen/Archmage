using Archmage.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Engine.Spells.Effects
{
    class Effect_WriteText:Effect
    {
        public string Text { get; set; }
        public Color TextColor { get; set; }

        public Effect_WriteText()
        {
            TextColor = new Color(libtcod.TCODColor.white);
            Text = "";
        }

        public Effect_WriteText(string text)
        {
            TextColor = new Color(libtcod.TCODColor.white);
            Text = text;
        }

        public override void Activate()
        {
            _scene.WriteMessage(Text);
            EndEffect();
        }
    }
}
