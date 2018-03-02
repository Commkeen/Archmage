using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archmage
{
    public enum TurnResult { IGNORE_TURN, GOTO_STARTTURN, GOTO_TAKETURN, GOTO_ENDTURN, TURN_OVER };

    interface ITakesTurns
    {
        

        int GetPriority();
        bool IsReadyForTurn();
        bool IsCurrentlyTakingTurn();

        void OnTick();

        TurnResult StartTurn();
        TurnResult TakeTurn();
        TurnResult EndTurn();

    }
}
