using UnityEngine;
using _Project.Logic.Configs.Data;

namespace _Project.Logic.Characters
{
    internal class TargetDetector
    {
        private readonly Transform _transform;
        private readonly INavigationData _data;
        private readonly RaycastHit2D[] _targetHit = new RaycastHit2D[1];

        public TargetDetector(Transform transform, INavigationData data)
        {
            _transform = transform;
            _data = data;
        }

        public void DetectingTarget()
        {
            int hits = Physics2D.RaycastNonAlloc(_transform.position, _transform.right, _targetHit, _data.DetectionRadius, _data.TargetLayer);

#if UNITY_EDITOR
            Debug.DrawRay(_transform.position, _transform.right * _data.DetectionRadius, Color.red);
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
            return sqrDistance <= _data.NearDistanceSqr;
        }
    }
}