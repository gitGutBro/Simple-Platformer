using UnityEngine;

namespace _Project.Logic.Configs.Data
{
    internal interface INavigationData : IMoveData
    {
        float NearDistance { get; }
        byte DetectionRadius { get; }
        LayerMask TargetLayer { get; }
        float NearDistanceSqr => NearDistance * NearDistance;
    }
}