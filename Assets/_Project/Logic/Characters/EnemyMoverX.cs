using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Logic.Common;
using _Project.Logic.Configs;

namespace _Project.Logic.Characters
{
    [Serializable]
    internal class EnemyMoverX
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Transform[] _wayPoints;

        private int _wayPointIndex = 0;
        private INavigationData _data;

        public event Action Moved;
        public event Action Stoped;

        public void Init(INavigationData data) =>
            _data = data;

        public void Stop()
        {
            _rigidbody2D.linearVelocityX = 0;
            Stoped?.Invoke();
        }

        public async UniTaskVoid Patrolling(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested is false)
            {
                await MoveToPoint(_wayPoints[_wayPointIndex].transform.position, cancellationToken);

                _wayPointIndex++;

                if (_wayPointIndex >= _wayPoints.Length)
                    _wayPointIndex = 0;
            }
        }

        public UniTask MoveToPoint(Vector2 point, CancellationToken cancellationToken) =>
            Move(() => point, cancellationToken);

        public UniTask MoveToTarget(Transform target, CancellationToken cancellationToken) =>
            target == null ? UniTask.CompletedTask : Move(() => target.position, cancellationToken);

        private async UniTask Move(Func<Vector2> targetProvider, CancellationToken cancellationToken)
        {
            Moved?.Invoke();

            while (cancellationToken.IsCancellationRequested is false)
            {
                Vector2 targetPosition = targetProvider();

                if ((targetPosition - _rigidbody2D.position).sqrMagnitude <= _data.NearDistanceSqr)
                    break;

                _rigidbody2D.linearVelocityX = (targetPosition - _rigidbody2D.position).normalized.x * _data.Speed;
                _rigidbody2D.transform.FlipX(_rigidbody2D.linearVelocity.x);

                await UniTask.WaitForFixedUpdate();
            }
        }
    }
}