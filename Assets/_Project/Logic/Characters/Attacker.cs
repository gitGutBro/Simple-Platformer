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
        private Func<bool> _grounded;

        public bool OnCooldown => _cooldownService.IsOnCooldown;
        public bool CanAttack => _isAttacking is false && OnCooldown is false && _grounded.Invoke();

        private void Awake() => 
            _cooldownService = new CooldownService(_data.CooldownInSeconds);

        public void Init(IAttackAnimator animator, Func<bool> grounded = null)
        {
            _animator = animator;
            _grounded = grounded;
        }

        public void OnAttackPressed()
        {
            if (CanAttack is false)
                return;

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
            _isAttacking = true;
            await TryAttack();
            _isAttacking = false;
        }
    }
}