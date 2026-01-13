using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Logic.Characters;

namespace _Project.Logic.Items
{
    internal class CoinSpawner : MonoBehaviour
    {
        private const int SpawnCooldownInSeconds = 5;

        private readonly CancellationTokenSource _globalTokenSource = new();

        [SerializeField] private Coin _coinPrefab;
        [SerializeField] private Transform[] _spawnPoints;

        private Coin _currentCoin;

        private readonly System.Random _random = new();
        private readonly CooldownService _cooldownService = new(SpawnCooldownInSeconds);

        private Transform RandomSpawnPoint => _spawnPoints[_random.Next(_spawnPoints.Length)];

        private void Start()
        {
            _currentCoin = Instantiate(_coinPrefab, RandomSpawnPoint.position, Quaternion.identity, transform);
            _currentCoin.Consumed += OnCoinConsumed;
        }

        private void OnDestroy()
        {
            _globalTokenSource.Cancel();
            _globalTokenSource.Dispose();

            if (_currentCoin != null)
                _currentCoin.Consumed -= OnCoinConsumed;
        }

        private void OnCoinConsumed() => 
            HandleRespawn().Forget();

        private async UniTaskVoid HandleRespawn()
        {
            CancellationToken cancellationToken = _globalTokenSource.Token;

            try
            {
                await _cooldownService.WaitCooldown(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            _currentCoin.Respawn(RandomSpawnPoint);
        }
    }
}