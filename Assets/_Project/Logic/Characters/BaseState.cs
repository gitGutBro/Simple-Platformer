using System.Threading;
using Cysharp.Threading.Tasks;
using _Project.Logic.Infrastructure.StateMachine;

namespace _Project.Logic.Characters
{
    internal abstract class BaseState : IState
    {
        protected CancellationTokenSource CancellationTokenSource;

        protected StateMachine StateMachine { get; private set; }

        public void SetStateMachine(StateMachine stateMachine) =>
            StateMachine = stateMachine;

        public abstract UniTask Enter();
        public abstract UniTask Exit();

        protected void DisposeToken()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }
    }
}