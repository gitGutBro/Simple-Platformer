using UnityEngine;

namespace _Project.Logic.Configs.Data
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    internal class EnemyData : ScriptableObject, INavigationData, IAnimationData
    {
        [SerializeField] private AnimationClip _attackAnimation;

        [field: SerializeField] public float NearDistance { get; private set; }
        [field: SerializeField] public byte DetectionRadius { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }
        
        public float NearDistanceSqr => NearDistance * NearDistance;
        public float AttackAnimationLengthInSeconds => _attackAnimation.length;
    }
}
