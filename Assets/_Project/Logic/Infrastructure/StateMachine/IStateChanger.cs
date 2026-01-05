using Cysharp.Threading.Tasks;

namespace _Project.Logic.Infrastructure.StateMachine
{
    internal interface IStateChanger
    {
        UniTask Enter<TState>() where TState : IState;
    }
}
