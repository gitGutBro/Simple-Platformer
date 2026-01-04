using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Project.Logic.InputSystem
{
    internal class GameplayNewInputSystem : IGameplayInputSystem, IInitializable
    {
        private readonly InputSystem_Actions _actions;
        
        public GameplayNewInputSystem()
        {
            _actions = new InputSystem_Actions();

            _movePerformed = callbackContext => MoveChanged?.Invoke(callbackContext.ReadValue<Vector2>().x);
            _moveCanceled = _ => MoveChanged?.Invoke(0f);

            _jumpPerformed = _ => JumpPressed?.Invoke();
            _jumpCanceled = _ => JumpReleased?.Invoke();

            _attackPerformed = _ => AttackPressed?.Invoke();
            _vampirePerformed = _ => VampirePressed?.Invoke();
        }
        
        public event Action<float> MoveChanged;
        public event Action JumpPressed;
        public event Action JumpReleased;
        public event Action AttackPressed;
        public event Action VampirePressed;
        
        private readonly Action<InputAction.CallbackContext> _movePerformed;
        private readonly Action<InputAction.CallbackContext> _moveCanceled;
        private readonly Action<InputAction.CallbackContext> _jumpPerformed;
        private readonly Action<InputAction.CallbackContext> _jumpCanceled;
        private readonly Action<InputAction.CallbackContext> _attackPerformed;
        private readonly Action<InputAction.CallbackContext> _vampirePerformed;

        public void Initialize()
        {
            _actions.Gameplay.Move.performed += _movePerformed;
            _actions.Gameplay.Move.canceled += _moveCanceled;

            _actions.Gameplay.Jump.performed += _jumpPerformed;
            _actions.Gameplay.Jump.canceled += _jumpCanceled;

            _actions.Gameplay.Attack.performed += _attackPerformed;
            _actions.Gameplay.VampireSkill.performed += _vampirePerformed;

            _actions.Gameplay.Enable();
        }
        
        public void Enable() => 
            _actions.Gameplay.Enable();

        public void Disable() => 
            _actions.Gameplay.Disable();

        public void Dispose()
        {
            try
            {
                _actions.Gameplay.Move.performed -= _movePerformed;
                _actions.Gameplay.Move.canceled -= _moveCanceled;

                _actions.Gameplay.Jump.performed -= _jumpPerformed;
                _actions.Gameplay.Jump.canceled -= _jumpCanceled;

                _actions.Gameplay.Attack.performed -= _attackPerformed;
                _actions.Gameplay.VampireSkill.performed -= _vampirePerformed;
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.LogException(ex);
#endif
            }

            try
            {
                _actions.Gameplay.Disable();
                _actions.Dispose();
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR
                Debug.LogException(ex);
#endif
            }
        }
    }
}