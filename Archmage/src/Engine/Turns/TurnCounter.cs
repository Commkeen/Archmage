using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archmage.Engine.Scenes;

namespace Archmage
{
    class TurnCounter
    {
        PlayScene _scene;

        int _tickCounter;

        List<int> _objectsTakingTurns;

        public TurnCounter(PlayScene scene)
        {
            _scene = scene;
            _tickCounter = 0;
            _objectsTakingTurns = new List<int>();
        }

        public void ClearTurnCounter()
        {
            _tickCounter = 0;
            _objectsTakingTurns = new List<int>();
        }

        public void AddObjectToCounter(int objID)
        {
            if (!_objectsTakingTurns.Contains(objID))
                _objectsTakingTurns.Add(objID);
        }

        public void RemoveObjectFromCounter(int objID)
        {
            _objectsTakingTurns.Remove(objID);
        }

        public void Update()
        {
            GameObjectPool pool = _scene.GetGameObjectPool();
            bool redraw = false;

            //Keep going through this loop until something takes a proper turn and we want to redraw
            while (!redraw)
            {
                Queue<int> objectsActingThisTick = GetObjectsActingThisTick();

                //If there is nothing that needs to act, perform ticks until something needs to act
                while (objectsActingThisTick.Count == 0)
                {
                    //Perform a time tick
                    _tickCounter++;
                    int i = 0;
                    for (i = 0; i < _objectsTakingTurns.Count; i++)
                    {
                        ITakesTurns obj = pool.GetITakesTurns(_objectsTakingTurns[i]);
                        obj.OnTick();
                    }

                    objectsActingThisTick = GetObjectsActingThisTick();
                }

                TurnResult result = TurnResult.GOTO_STARTTURN;
                //Get the item at the top of the queue (TODO: get highest priority item?)
                ITakesTurns activeObject = pool.GetITakesTurns(objectsActingThisTick.Peek());
                if (!activeObject.IsCurrentlyTakingTurn())
                {
                    result = activeObject.StartTurn();
                }
                else
                {
                    result = activeObject.TakeTurn();
                }

                if (result != TurnResult.IGNORE_TURN)
                {
                    redraw = true;
                }
                if (result == TurnResult.GOTO_ENDTURN)
                {
                    result = activeObject.EndTurn();
                }

                if (!(activeObject is Archmage.Actors.Student))
                {
                    redraw = false;
                }
            }
        }

        public Queue<int> GetObjectsActingThisTick()
        {
            GameObjectPool pool = _scene.GetGameObjectPool();

            Queue<int> objectsActingThisTurn = new Queue<int>();

            List<int> deadObjects = new List<int>();

            foreach (int id in _objectsTakingTurns)
            {
                ITakesTurns obj = pool.GetITakesTurns(id);
                if (obj == null)
                {
                    deadObjects.Add(id);
                }
                else if (obj.IsReadyForTurn())
                {
                    objectsActingThisTurn.Enqueue(id);
                }
            }

            foreach (int id in deadObjects)
            {
                RemoveObjectFromCounter(id);
            }

            //TODO: Sort by priority!
            return objectsActingThisTurn;
        }
    }
}
