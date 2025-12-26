using UnityEngine;

namespace _Project.Logic.Characters
{
    internal class AnimationsCharacterSwitcher
    {
        private static readonly int IsGrounded = Animator.StringToHash(nameof(IsGrounded));
        private static readonly int Speed = Animator.StringToHash(nameof(Speed));
        private static readonly int Attack = Animator.StringToHash(nameof(Attack));
        
        private readonly Animator _animator;

        public AnimationsCharacterSwitcher(Animator animator) =>
            _animator = animator;

        public void SetSpeed(float speed) =>
            _animator.SetFloat(Speed, Mathf.Abs(speed));

        public void SetGrounded(bool state) =>
            _animator.SetBool(IsGrounded, state);

        public void SetAttacking() =>
            _animator.SetTrigger(Attack);
    }
}