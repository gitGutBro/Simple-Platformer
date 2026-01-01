using UnityEngine;

namespace _Project.Logic.Characters
{
    internal class TargetDetector
    {
        private readonly float _nearDistance;
        private readonly float _detectionRadius;
        private readonly Transform _transform;
        private readonly LayerMask _targetLayer;
        private readonly RaycastHit2D[] _targetHit = new RaycastHit2D[1];

        private float SqrNearDistance => _nearDistance * _nearDistance;

        public TargetDetector(float nearRadius, float detectionRadius, LayerMask targetLayer, Transform transform)
        {
            _nearDistance = nearRadius;
            _detectionRadius = detectionRadius;
            _targetLayer = targetLayer;
            _transform = transform;
        }

        public void DetectingTarget()
        {
            int hits = Physics2D.RaycastNonAlloc(_transform.position, _transform.right, _targetHit, _detectionRadius, _targetLayer);

#if UNITY_EDITOR
            Debug.DrawRay(_transform.position, _transform.right * _detectionRadius, Color.red);
#endif

            if (hits is 0)
                _targetHit[0] = default;
        }

        public Transform GetTargetTransfrom() => 
            _targetHit[0].transform;

        public bool IsTargetHit() =>
            _targetHit[0].collider != null;

        public bool IsTargetLost() =>
            IsTargetHit() is false;

        public bool IsTargetNear()
        {
            if (IsTargetHit() is false)
                return false;

            float sqrDistance = (_targetHit[0].transform.position - _transform.position).sqrMagnitude;
            return sqrDistance <= SqrNearDistance;
        }
    }
}