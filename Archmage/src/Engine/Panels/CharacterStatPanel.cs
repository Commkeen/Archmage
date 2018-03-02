using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;
using Archmage.Actors;
using Archmage.System;
using Archmage.Engine.DataStructures;

namespace Archmage.Panels
{
    class CharacterStatPanel:Panel
    {
        Student _student;
        PlayScene _scene;

        public CharacterStatPanel(Student student, PlayScene scene)
        {
            _student = student;
            _scene = scene;

            _windowPosition = new IntVector2();
            _windowSize = new IntVector2();

            _windowPosition.X = 71;
            _windowPosition.Y = 0;
            _windowSize.X = 29;
            _windowSize.Y = 30;
        }

        public override void DrawPanel()
        {
            Display.DrawWindowFrame(_windowPosition, _windowSize);

            DrawStringInPanel(2,1, _scene.GetLevelName());
            DrawStringInPanel(2, 2, "Depth: " + _scene.GetDepth());
            DrawStringInPanel(2, 6, "Health: " + _student.CurrentHealth + "\\" + _student.MaxHealth);
            DrawStringInPanel(2, 8, "Attention: " + _student.AttentionUsed + "\\" + _student.MaxAttention);
            DrawStringInPanel(2, 10, "Essence: " + _student.CurrentEssence);
            DrawStringInPanel(2, 12, "Evasion: " + _student.GetEvasionPoints());
            DrawStringInPanel(2, 14, "Shield: " + _student.GetShieldLayers() + " | " + _student.GetShieldStrength());
        }
    }
}
