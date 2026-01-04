using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal sealed class Attacker : DamageArea, IPlayerAttacker, IEnemyAttacker
    {
        private const float DelayAfterAnimationInSeconds = 0.5f;

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

        public async UniTask TryAttack(CancellationToken cancellationToken = default)
        {
            if (OnCooldown)
                return;

            OnCooldown = true;
            RunCooldownUncancellable().Forget();

            _animator.SetAttacking();

            await UniTask.Delay(TimeSpan.FromMilliseconds(_animator.AttackAnimationTimeInSeconds));
            await UniTask.WaitForSeconds(DelayAfterAnimationInSeconds);

            if (HaveTarget)
                TakeDamageToTarget(_damage);
        }

        private async UniTaskVoid RunCooldownUncancellable()
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(_cooldownInMilliseconds));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                OnCooldown = false;
            }
        }

        private async UniTaskVoid HandleAttack()
        {
            await TryAttack();
            _isAttacking = false;
        }
    }
}