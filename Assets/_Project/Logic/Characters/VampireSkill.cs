using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Project.Logiс.Configs.Data;

namespace _Project.Logic.Characters
{
    internal sealed class VampireSkill : DamageArea
    {
        private readonly CancellationTokenSource _globalTokenSource = new();
        
        [SerializeField] private Image _areaImage;
        [SerializeField] private CircleCollider2D _areaCollider;
        [SerializeField] private VampireSkillData _data;

        private bool _isActivated;
        private bool _isStealing;

        private CooldownService _cooldownService;
        private IHealable _healable;

        private bool CanActivate => _cooldownService.IsOnCooldown is false && _isActivated is false;
        
        private void Awake()
        {
            _areaImage.enabled = false;
            _areaCollider.enabled = false;
            
            _cooldownService = new CooldownService(_data.CooldownInSeconds);

            WaitStealing(_globalTokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _globalTokenSource?.Cancel();
            _globalTokenSource?.Dispose();
        }

        private async UniTaskVoid WaitStealing(CancellationToken globalToken)
        {
            Func<bool> cachedCanSteal = () => _isActivated && HaveTarget && _healable is not null;
            
            while (globalToken.IsCancellationRequested is false)
            {
                await UniTask.WaitUntil(cachedCanSteal, cancellationToken: globalToken);
                
                using CancellationTokenSource stealTokenSource =
                    CancellationTokenSource.CreateLinkedTokenSource(globalToken);
                
                await StealHealth(stealTokenSource.Token);
            }
        }
        
        public void Init(IHealable healable) =>
            _healable = healable;

        public void OnVampirePressed()
        {
            if (CanActivate is false)
                return;
            
            Activate().Forget();
        }
        
        private async UniTaskVoid Activate()
        {
            ChangeActivateStates(true);
            await UniTask.WaitForSeconds(_data.ActiveTimeInSeconds);
            ChangeActivateStates(false);
                
            _cooldownService.WaitCooldown().Forget();
        }

        private async UniTask StealHealth(CancellationToken cancellationToken)
        {
            const float HalfSecond = 0.5f;
            
            if (HaveTarget is false || _healable is null)
                return;
            
            if (_isStealing)
                return;
            
            _isStealing = true;
            
            int takenDamage = TakeDamageToAll(_data.DamageInHalfSecond);
            _healable.Heal(takenDamage);

            await UniTask.WaitForSeconds(HalfSecond, cancellationToken: cancellationToken);
            
            _isStealing = false;
        }

        private void ChangeActivateStates(bool state)
        {
            _areaImage.enabled = state;
            _areaCollider.enabled = state;
            _isActivated = state;
        }
    }
}