using _Project.Logic.Characters;

namespace _Project.Logic.Items
{
    internal interface IHealConsumable
    {
        void Consume(IHealthHaver itemConsumer);
    }
}