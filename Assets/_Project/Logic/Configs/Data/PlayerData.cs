using UnityEngine;

namespace _Project.Logic.Configs.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
    internal class PlayerData : ScriptableObject, IJumpData, IMoveData
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public float JumpCutMultiplier { get; private set; }
        [field: SerializeField] public float GroundCheckRadius { get; private set; }
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }
    }
}
