namespace _Project.Logic.Infrastructure.StateMachine
{
    internal interface IState
    {
        void Enter();
        void Exit();
    }
}