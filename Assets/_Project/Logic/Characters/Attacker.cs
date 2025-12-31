using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal sealed class Attacker : DamageArea, IPlayerAttacker, IEnemyAttacker
    {
        [SerializeField] private int _damage;
        [SerializeField] private float _cooldownInMilliseconds;

        private bool _isAttacking;
        private IAttackAnimator _animator;

        public event Func<bool> Grounded;

        public bool OnCooldown { get; private set; }
        public bool CanAttack => _isAttacking is false && OnCooldown is false && Grounded.Invoke();

        public void Init(IAttackAnimator animator) =>
            _animator = animator;

        public void OnAttackPressed()
        {
            if (CanAttack is false)
                return;

            _isAttacking = true;
            HandleAttack().Forget();
        }

        public async UniTask TryAttack()
        {
            if (OnCooldown)
                return;
            
            if (HaveTarget)
                TakeDamageToTarget(_damage);

            _animator.SetAttacking();

            OnCooldown = true;
            await UniTask.Delay(TimeSpan.FromMilliseconds(_cooldownInMilliseconds));
            OnCooldown = false;
        }

        private async UniTaskVoid HandleAttack()
        {
            await TryAttack();
            _isAttacking = false;
        }
    }
}