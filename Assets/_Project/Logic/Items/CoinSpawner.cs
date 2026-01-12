using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Logic.Characters;

namespace _Project.Logic.Items
{
    internal class CoinSpawner : MonoBehaviour
    {
        private const int SpawnCooldownInSeconds = 5;

        [SerializeField] private Coin _coin;
        [SerializeField] private Transform[] _spawnPoints;

        private readonly System.Random _random = new();
        private readonly CooldownService _cooldownService = new(SpawnCooldownInSeconds);

        private Transform RandomSpawnPoint => _spawnPoints[_random.Next(_spawnPoints.Length)];


        private void Start()
        {
            _coin.Respawn(RandomSpawnPoint);

            Spawning().Forget();
        }

        private async UniTaskVoid Spawning()
        {
            while (enabled)
            {
                await UniTask.WaitUntil(() => _coin.gameObject.activeSelf is false);

                await _cooldownService.WaitCooldown();

                _coin.Respawn(RandomSpawnPoint);
            }
        }
    }
}