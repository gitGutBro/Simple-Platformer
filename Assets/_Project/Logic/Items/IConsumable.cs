using _Project.Logic.Characters;

namespace _Project.Logic.Items
{
    internal interface IConsumable
    {
        void Consume(IItemConsumer consumer);
    }
}