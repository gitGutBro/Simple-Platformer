using System.Threading;
using Cysharp.Threading.Tasks;
using _Project.Logic.Infrastructure.StateMachine;

namespace _Project.Logic.Characters
{
    internal abstract class BaseState : IState
    {
        private CancellationTokenSource _cancellationTokenSource;

        protected IStateChanger StateChanger { get; private set; }

        public void SetStateChanger(IStateChanger stateChanger) =>
            StateChanger = stateChanger;

        public UniTask Enter()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return EnterCore(_cancellationTokenSource.Token);
        }

        public async UniTask Exit()
        {
            DisposeToken();
            await UniTask.CompletedTask;
        }

        protected void DisposeToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        protected abstract UniTask EnterCore(CancellationToken cancellationToken);
    }
}