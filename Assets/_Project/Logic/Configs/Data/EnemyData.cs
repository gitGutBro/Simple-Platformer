using UnityEngine;

namespace _Project.Logic.Configs.Data
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    internal class EnemyData : ScriptableObject, INavigationData
    {
        [field: SerializeField] public float NearDistance { get; private set; }
        [field: SerializeField] public byte SightDistance { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        
        public float NearDistanceSqr => NearDistance * NearDistance;
    }
}
