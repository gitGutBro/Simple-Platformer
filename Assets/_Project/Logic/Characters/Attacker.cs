using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal class Attacker : MonoBehaviour
    {
        [SerializeField] private int  _damage;
        [SerializeField] private float _cooldownInMiliseconds;
        
        private IDamagable _currentDamagable;

        public bool OnCooldown { get; private set; }
        private bool HaveEnemy => _currentDamagable != null;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out IDamagable damagable) == false) 
                return;
            
            _currentDamagable = damagable;
        }

        private void OnTriggerExit2D(Collider2D other) => 
            _currentDamagable = null;

        public async UniTask TryAttack()
        {
            if (HaveEnemy == false || OnCooldown)
                return;
            
            _currentDamagable.TakeDamage(_damage);

            OnCooldown = true;
            await UniTask.Delay(TimeSpan.FromMilliseconds(_cooldownInMiliseconds));
            OnCooldown = false;
        }
    }
}