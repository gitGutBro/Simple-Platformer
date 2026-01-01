namespace _Project.Logic.Characters
{
    internal interface IChaseContext
    {
        EnemyMoverX MoverX { get; }
        TargetDetector TargetDetector { get; }
    }
}