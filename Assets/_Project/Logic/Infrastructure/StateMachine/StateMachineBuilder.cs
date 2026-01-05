using System;
using System.Collections.Generic;

namespace _Project.Logic.Infrastructure.StateMachine
{
    internal class StateMachineBuilder
    {
        private readonly List<Func<IState>> _creators = new();

        public StateMachineBuilder AddState(Func<IState> factory)
        {
            _creators.Add(factory);
            return this;
        }

        public StateMachine Build()
        {
            Dictionary<Type, IState> states = new();

            foreach (Func<IState> create in _creators)
            {
                IState state = create();
                states[state.GetType()] = state;
            }

            StateMachine stateMachine = new(states);

            foreach (IState state in states.Values)
                state.SetStateChanger(stateMachine);

            return stateMachine;
        }
    }
}