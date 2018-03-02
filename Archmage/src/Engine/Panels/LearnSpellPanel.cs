using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;
using Archmage.Engine.Spells;
using Archmage.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archmage.Panels
{
    class LearnSpellPanel : Panel
    {

        PlayScene _scene;

        LearnSpellInterface _interface;

        public LearnSpellPanel(PlayScene scene, LearnSpellInterface learnSpellInterface)
        {
            _scene = scene;
            _interface = learnSpellInterface;
            _windowPosition = new IntVector2();
            _windowSize = new IntVector2();
            _windowPosition.X = 21;
            _windowPosition.Y = 0;
            _windowSize.X = 50;
            _windowSize.Y = 30;
        }

        public override void DrawPanel()
        {
            DrawStringInPanel(2, 3, "The spellbook contains: " + _interface.spell.Name);
            DrawStringInPanel(2, 5, _interface.spell.Description);

            if (_interface.displayConfirmation)
                DrawStringInPanel(2, 8, "Are you sure you want to put this spell in slot " + _interface.slotSelected + "? Y/N");
            else
                DrawStringInPanel(2, 8, "Select a slot to place this spell, or press Escape to cancel");
        }
    }
}
