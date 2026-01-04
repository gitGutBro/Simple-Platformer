using UnityEngine;

namespace _Project.Logic.Configs.Data
{
    internal interface IJumpData
    {
        float JumpForce { get; }
        float JumpCutMultiplier { get; }
        float GroundCheckRadius { get; }
        LayerMask GroundLayer { get; }
    }
}