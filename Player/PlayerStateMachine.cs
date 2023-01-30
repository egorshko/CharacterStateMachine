using UnityEngine;

namespace Root 
{ 
    internal class PlayerStateMachine 
    {
        public PlayerState CurrentState { get; private set; }
        public void Init(PlayerState startState)
        {
            CurrentState = startState;
            startState.Enter();
        }

        public void ChangeState(PlayerState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            newState.Enter();
        }
    }
}
