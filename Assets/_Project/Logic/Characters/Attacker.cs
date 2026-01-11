using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal sealed class Attacker : DamageArea, IPlayerAttacker, IEnemyAttacker
    {
        private const float DelayAfterAnimationInSeconds = 0.5f;

        [SerializeField] private AttackData _data;

        private bool _isAttacking;
        private CooldownService _cooldownService;
        private IAttackAnimator _animator;

        public event Func<bool> Grounded;

        public bool OnCooldown => _cooldownService.IsOnCooldown;
        public bool CanAttack => _isAttacking is false && OnCooldown is false && Grounded.Invoke();

        private void Awake() => 
            _cooldownService = new CooldownService(_data.CooldownInSeconds);

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

            _cooldownService.WaitCooldown().Forget();

            _animator.SetAttacking();
            await UniTask.WaitForSeconds(_animator.AttackAnimationTimeInSeconds);

            TakeDamageToAll(_data.Damage);

            await UniTask.WaitForSeconds(DelayAfterAnimationInSeconds);
        }

        private async UniTaskVoid HandleAttack()
        {
            await TryAttack();
            _isAttacking = false;
        }
    }
}