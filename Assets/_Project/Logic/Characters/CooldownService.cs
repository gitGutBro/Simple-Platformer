using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Logic.Characters
{
    internal class CooldownService
    {
        private const int MinValueInSeconds = 1;

        private readonly float _cooldownInSeconds;

        public CooldownService(float cooldownInSeconds) =>
            _cooldownInSeconds = cooldownInSeconds;

        public event Action<float, float> Tick;

        public bool IsOnCooldown { get; private set; }

        public async UniTask WaitCooldown(CancellationToken cancellationToken = default)
        {
            IsOnCooldown = true;

            try
            {
                float remaining = _cooldownInSeconds;

                while (remaining > 0f && cancellationToken.IsCancellationRequested is false)
                {
                    float normalized = Mathf.Clamp01(remaining / _cooldownInSeconds);
                    Tick?.Invoke(normalized, remaining);

                    float wait = Mathf.Min(MinValueInSeconds, remaining);
                    await UniTask.WaitForSeconds(wait, cancellationToken: cancellationToken);
                    remaining -= wait;
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
# if UNITY_EDITOR
                Debug.LogError($"CooldownService WaitCooldown error: {ex}");
#endif
            }
            finally
            {
                IsOnCooldown = false;
                Tick?.Invoke(0f, 0f);
            }
        }
    }
}
