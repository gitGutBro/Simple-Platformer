using UnityEngine;

namespace _Project.Logic.Characters
{
    internal abstract class DamageArea : MonoBehaviour
    {
        protected IDamagable CurrentDamagable;
        
        protected bool HaveTarget => CurrentDamagable is not null;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamagable damagable))
                CurrentDamagable = damagable;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if  (other.TryGetComponent(out IDamagable _))
                CurrentDamagable = null;
        }
    }
}