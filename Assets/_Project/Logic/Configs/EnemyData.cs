using UnityEngine;

namespace _Project.Logic.Configs
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    internal class EnemyData : ScriptableObject
    {
        [SerializeField] private float _nearDistance;
        
        [field: SerializeField] public byte SightDistance { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        
        public float NearDistanceSqr => _nearDistance * _nearDistance;
    }
}
