namespace _Project.Logic.Configs.Data
{
    internal interface INavigationData : IMoveData
    {
        float NearDistance { get; }
        byte SightDistance { get; }
        float NearDistanceSqr => NearDistance * NearDistance;
    }
}