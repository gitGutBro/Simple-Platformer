using System.Collections.Generic;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal abstract class DamageArea : MonoBehaviour
    {
        private readonly HashSet<IDamagable> _overlappingTargets = new();

        protected bool HaveTarget => _overlappingTargets.Count > 0;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamagable damagable))
                _overlappingTargets.Add(damagable);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if  (other.TryGetComponent(out IDamagable damagable))
                _overlappingTargets.Remove(damagable);
        }

        private void OnDestroy() =>
            _overlappingTargets.Clear();

        protected int TakeDamageToAll(int damage)
        {
            if (_overlappingTargets.Count is 0)
                return 0;

            IDamagable[] copy = new IDamagable[_overlappingTargets.Count];
            _overlappingTargets.CopyTo(copy);

            int totalApplied = 0;

            foreach (IDamagable damagable in copy)
            {
                if (damagable is null)
                    continue;

                totalApplied += damagable.TakeDamage(damage);
            }

            return totalApplied;
        }
    }
}