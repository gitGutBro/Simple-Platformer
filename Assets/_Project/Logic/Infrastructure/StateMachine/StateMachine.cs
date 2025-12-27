using System;
using System.Collections.Generic;

namespace _Project.Logic.Infrastructure.StateMachine
{
    internal class StateMachine
    {
        private readonly Dictionary<Type, IState> _states;
        
        private IState _activeState;

        public StateMachine(Dictionary<Type, IState> states) => 
            _states = states;

        public void Enter<TState>() where TState : IState
        {
            _activeState?.Exit();
            IState state = _states[typeof(TState)];
            _activeState = state;
            state.Enter();
        }
    }
}
