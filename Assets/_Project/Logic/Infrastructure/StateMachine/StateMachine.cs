using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Project.Logic.Infrastructure.StateMachine
{
    internal class StateMachine : IStateChanger
    {
        private readonly Dictionary<Type, IState> _states;
        
        private IState _activeState;

        public StateMachine(Dictionary<Type, IState> states) => 
            _states = states;

        public async UniTask Enter<TState>() where TState : IState
        {
            await Exit();

            IState state = _states[typeof(TState)];
            _activeState = state;

            await state.Enter();
        }

        public async UniTask Exit()
        {
            if (_activeState is not null)
                await _activeState.Exit();
        }
    }
}
