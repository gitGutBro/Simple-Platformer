namespace _Project.Logic.Characters
{
    internal interface IHealable : IHealthHaver
    {
        public void Heal(int amount);
    }
}