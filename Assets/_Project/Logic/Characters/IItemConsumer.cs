namespace _Project.Logic.Characters
{
    internal interface IItemConsumer : IHealthHaver
    {
        Wallet Wallet { get; }
    }
}