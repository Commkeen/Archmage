using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.System;
using Archmage.Engine.DataStructures;
using Archmage.Engine.Scenes;

namespace Archmage.Panels
{
    class MessagePanel : Panel
    {
        Queue<string> _messageQueue;

        public MessagePanel(Queue<string> messages)
        {
            _windowPosition = new IntVector2();
            _windowSize = new IntVector2();

            _windowPosition.X = 50;
            _windowPosition.Y = 30;
            _windowSize.X = 50;
            _windowSize.Y = 20;
            _messageQueue = messages;
        }

        public override void DrawPanel()
        {
            Display.DrawWindowFrame(_windowPosition, _windowSize);
            int messagesToWrite = Math.Min(_messageQueue.Count, _windowSize.Y - 2);
            for (int i = 0; i < messagesToWrite; i++)
            {
                DrawStringInPanel(1, _windowSize.Y - i - 2, _messageQueue.ElementAt(_messageQueue.Count - 1 - i));
            }
        }
    }
}
