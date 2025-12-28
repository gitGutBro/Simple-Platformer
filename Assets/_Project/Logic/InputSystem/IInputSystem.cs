using System;

namespace _Project.Logic.InputSystem
{
    internal interface IInputSystem : IDisposable
    {
        void Enable();
        void Disable();
    }
}