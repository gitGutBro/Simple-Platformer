using System;
using UnityEngine;

namespace _Project.Logic.Characters
{
    [Serializable]
    internal class AnimationsCharacterSwitcher : IAttackAnimator
    {
        private static readonly int IsGrounded = Animator.StringToHash(nameof(IsGrounded));
        private static readonly int Speed = Animator.StringToHash(nameof(Speed));
        private static readonly int Attack = Animator.StringToHash(nameof(Attack));
        
        [SerializeField] private Animator _animator;

        [field: SerializeField] public float AttackAnimationTimeInMilliseconds { get; private set; }

        public void SetSpeed(float speed) =>
            _animator.SetFloat(Speed, Mathf.Abs(speed));

        public void SetGrounded(bool state) =>
            _animator.SetBool(IsGrounded, state);

        public void SetAttacking() =>
            _animator.SetTrigger(Attack);
    }
}