using UnityEngine;

namespace _Project.Logic.Characters
{
    internal abstract class DamageArea : MonoBehaviour
    {
        private IDamagable _currentDamagable;
        
        protected bool HaveTarget => _currentDamagable is not null;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamagable damagable))
                _currentDamagable = damagable;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if  (other.TryGetComponent(out IDamagable _))
                _currentDamagable = null;
        }
        
        protected int TakeDamageToTarget(int damage) => 
            _currentDamagable.TakeDamage(damage);
    }
}