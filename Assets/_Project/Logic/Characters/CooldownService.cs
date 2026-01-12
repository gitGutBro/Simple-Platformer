using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal class CooldownService
    {
        private readonly float _cooldownInSeconds;

        public CooldownService(float cooldownInSeconds) =>
            _cooldownInSeconds = cooldownInSeconds;

        public bool IsOnCooldown { get; private set; }

        public async UniTask WaitCooldown()
        {
            IsOnCooldown = true;

            try
            {
                await UniTask.WaitForSeconds(_cooldownInSeconds);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                IsOnCooldown = false;
            }
        }
    }
}
