using System;
using System.Threading;
using _Project.Logic.Common;
using _Project.Logic.Health;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Logic.Characters
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    internal class Enemy : MonoBehaviour, IDamagable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly RaycastHit2D[] _playerHit = new RaycastHit2D[1];
        
        [SerializeField] private float _nearDistance;
        [SerializeField] private float _speed;
        [SerializeField] private byte _sightDistance;
        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private Transform _transform;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Attacker _attacker;
        [SerializeField] private Transform[] _wayPoints;
        
        [field: SerializeField] public HealthModel Health { get; private set; }

        private bool _isCleaned;
        private int _wayPointIndex = 0;
        private AnimationsCharacterSwitcher _animationsSwitcher;
        
        private float NearDistanceSqr => _nearDistance * _nearDistance;
        
        private void Awake()
        {
            _animationsSwitcher = new AnimationsCharacterSwitcher(_animator);

            Health.Died += OnDie;
        }

        private void Start() => 
            StartStatesBehaviour(_cancellationTokenSource.Token).Forget();

        private void OnValidate() => 
            Health.OnValidate();

        private void OnDestroy()
        {
            Health.Died -= OnDie;
            
            Clean();
        }
        
        public void TakeDamage(int amount) => 
            Health.Decrease(amount);
        
        private async UniTaskVoid StartStatesBehaviour(CancellationToken globalToken)
        {
            DetectingPlayer(globalToken).Forget();

            Func<bool> cachedPlayerHit = OnPlayerHit;
            Func<bool> cachedPlayerLost = OnPlayerLost;
            Func<bool> cachedPlayerNear = OnPlayerNear;

            UniTask[] whenAnyTasks = new UniTask[2];

            try
            {
                while (globalToken.IsCancellationRequested is false)
                {
                    if (cachedPlayerLost.Invoke())
                    {
                        using CancellationTokenSource patrolTokenSource =
                            CancellationTokenSource.CreateLinkedTokenSource(globalToken);

                        Patrolling(patrolTokenSource.Token).Forget();

                        await UniTask.WaitUntil(cachedPlayerHit, cancellationToken: globalToken);
                        patrolTokenSource.Cancel();
                    }
                    else if (cachedPlayerNear.Invoke())
                    {
                        while (cachedPlayerNear.Invoke() && globalToken.IsCancellationRequested is false)
                        {
                            _rigidbody2D.linearVelocityX = 0;
                            _animationsSwitcher.SetSpeed(0);
                            _animationsSwitcher.SetAttacking();

                            await _attacker.TryAttack();
                        }
                    }
                    else
                    {
                        using CancellationTokenSource playerFollowerTokenSource =
                            CancellationTokenSource.CreateLinkedTokenSource(globalToken);

                        if (cachedPlayerHit.Invoke())
                            MoveToTarget(_playerHit[0].transform, playerFollowerTokenSource.Token).Forget();

                        MoveToPoint(_playerHit[0].transform.position, playerFollowerTokenSource.Token).Forget();

                        whenAnyTasks[0] = UniTask.WaitUntil(cachedPlayerLost, cancellationToken: globalToken);
                        whenAnyTasks[1] = UniTask.WaitUntil(cachedPlayerNear, cancellationToken: globalToken);

                        await UniTask.WhenAny(whenAnyTasks);

                        for (int i = 0; i < whenAnyTasks.Length; i++)
                            whenAnyTasks[i] = default;

                        playerFollowerTokenSource.Cancel();
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
#if UNITY_EDITOR
                Debug.Log($"Operation canceled: {ex}");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                Clean();
            }
        }
        
        private async UniTaskVoid Patrolling(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested is false)
            {
                await MoveToPoint(_wayPoints[_wayPointIndex].transform.position, cancellationToken);
                
                _wayPointIndex++;
                
                if (_wayPointIndex >= _wayPoints.Length)
                    _wayPointIndex = 0;
            }
        }

        private async UniTaskVoid DetectingPlayer(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested is false)
            {
                int hits = Physics2D.RaycastNonAlloc(_transform.position, _transform.right, _playerHit, _sightDistance, _playerMask);

#if UNITY_EDITOR
                Debug.DrawRay(_transform.position, _transform.right * _sightDistance, Color.red);
#endif

                if (hits is 0)
                    _playerHit[0] = default;
                
                await UniTask.WaitForFixedUpdate();
            }
        }

        private UniTask MoveToPoint(Vector2 point, CancellationToken cancellationToken) => 
            Move(() => point, cancellationToken);

        private UniTask MoveToTarget(Transform target, CancellationToken cancellationToken) => 
            target is null ? UniTask.CompletedTask : Move(() => target.position, cancellationToken);

        private async UniTask Move(Func<Vector2> targetProvider, CancellationToken cancellationToken)
        {
            const byte NumberAboveZero = 1;
            
            _animationsSwitcher.SetSpeed(NumberAboveZero);

            while (cancellationToken.IsCancellationRequested is false)
            {
                Vector2 targetPosition = targetProvider();

                if ((targetPosition - _rigidbody2D.position).sqrMagnitude <= NearDistanceSqr)
                    break;

                _rigidbody2D.linearVelocityX = (targetPosition - _rigidbody2D.position).normalized.x * _speed;
                _rigidbody2D.transform.FlipX(_rigidbody2D.linearVelocity.x);

                await UniTask.WaitForFixedUpdate();
            }
        }
        
        private bool OnPlayerHit() => 
            _playerHit[0].collider is not null;
        
        private bool OnPlayerLost() => 
            OnPlayerHit() is false;
        
        private bool OnPlayerNear()
        {
            if (OnPlayerHit() is false)
                return false;

            float sqrDistance = (_playerHit[0].transform.position - _transform.position).sqrMagnitude;
            return sqrDistance <= NearDistanceSqr;
        }

        private void Clean()
        {
            if (_isCleaned)
                return;
            
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            
            _isCleaned = true;
        }
        
        private void OnDie()
        {
            gameObject.SetActive(false);
            
            Clean();
        }
    }
}
