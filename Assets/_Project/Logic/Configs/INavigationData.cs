namespace _Project.Logic.Configs
{
    internal interface INavigationData : IMoveData
    {
        float NearDistance { get; }
        byte SightDistance { get; }
        float NearDistanceSqr => NearDistance * NearDistance;
    }
}