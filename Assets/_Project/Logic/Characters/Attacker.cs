using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal class Attacker : DamageArea
    {
        [SerializeField] private int _damage;
        [SerializeField] private float _cooldownInMilliseconds;
        
        public bool OnCooldown { get; private set; }
        
        public async UniTask TryAttack()
        {
            if (OnCooldown)
                return;
            
            if (HaveTarget)
                TakeDamageToTarget(_damage);

            OnCooldown = true;
            await UniTask.Delay(TimeSpan.FromMilliseconds(_cooldownInMilliseconds));
            OnCooldown = false;
        }
    }
}