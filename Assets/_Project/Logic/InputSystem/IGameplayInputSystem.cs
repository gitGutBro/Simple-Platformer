using System;

namespace _Project.Logic.InputSystem
{
    internal interface IGameplayInputSystem : IInputSystem
    {
        public event Action<float> MoveChanged;
        public event Action JumpPressed;
        public event Action JumpReleased;
        public event Action AttackPressed;
        public event Action VampirePressed;
    }
}