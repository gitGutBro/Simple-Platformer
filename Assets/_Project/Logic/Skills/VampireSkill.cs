using _Project.Logic.Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Logic.Skills
{
    internal class VampireSkill : MonoBehaviour
    {
        [SerializeField] private int _damageInHalfSecond;
        [SerializeField] private float _cooldownInSeconds;
        [SerializeField] private float _activeTimeInSeconds;
        [SerializeField] private Image _areaImage;
        [SerializeField] private CircleCollider2D _areaCollider;

        private bool _isOnCooldown;
        private bool _isActivated;
        private bool _isStealing;
        private IDamagable _damagable;
        private IHealable _healable;
        
        private void Awake()
        {
            _areaImage.enabled = false;
            _areaCollider.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
                Active().Forget();
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

        private async UniTaskVoid Active()
        {
            if (_isOnCooldown || _isActivated)
                return;
            
            _areaImage.enabled = true;
            _areaCollider.enabled = true;
            _isActivated = true;
            
            await UniTask.WaitForSeconds(_activeTimeInSeconds);
            
            _areaImage.enabled = false;
            _areaCollider.enabled = false;
            _isActivated = false;

            WaitCooldown().Forget();
        }

        private async UniTaskVoid WaitCooldown()
        {
            if (_isOnCooldown || _isActivated)
                return;
            
            _isOnCooldown = true;
            await UniTask.WaitForSeconds(_cooldownInSeconds);
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
            
            damagable.TakeDamage(_damageInHalfSecond);
            healable.Heal(_damageInHalfSecond);

            await UniTask.WaitForSeconds(HalfSecond);
            
            _isStealing = false;
        }
    }
}