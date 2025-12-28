using _Project.Logic.Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Logic.Skills
{
    internal class VampireSkill : MonoBehaviour
    {
        [SerializeField] private Image _areaImage;
        [SerializeField] private CircleCollider2D _areaCollider;
        [SerializeField] private VampireSkillData _data;

        private bool _isOnCooldown;
        private bool _isActivated;
        private bool _isStealing;
        
        private IDamagable _damagable;
        private IHealable _healable;

        private bool CanActivate => _isOnCooldown is false && _isActivated is false;
        
        private void Awake()
        {
            _areaImage.enabled = false;
            _areaCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamagable damagable))
                _damagable = damagable;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isActivated is false)
                return;
                
            StealHealth(_damagable, _healable).Forget();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if  (other.TryGetComponent(out IDamagable _))
                _damagable = null;
        }

        public void Init(IHealable healable) =>
            _healable = healable;

        public void TryActivate()
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
                
            WaitCooldown().Forget();
        }

        private async UniTaskVoid WaitCooldown()
        {
            _isOnCooldown = true;
            await UniTask.WaitForSeconds(_data.CooldownInSeconds);
            _isOnCooldown = false;
        }

        private async UniTaskVoid StealHealth(IDamagable damagable, IHealable healable)
        {
            const float HalfSecond = 0.5f;
            
            if (damagable is null || healable is null)
                return;
            
            if (_isStealing)
                return;
            
            _isStealing = true;
            
            damagable.TakeDamage(_data.DamageInHalfSecond);
            healable.Heal(_data.DamageInHalfSecond);

            await UniTask.WaitForSeconds(HalfSecond);
            
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