using Cysharp.Threading.Tasks;

namespace _Project.Logic.Infrastructure.StateMachine
{
    internal interface IState
    {
        UniTask Enter();
        UniTask Exit();
        void SetStateChanger(IStateChanger stateChanger);
    }
}