namespace _Project.Logic.Characters
{
    internal interface IHealable : IHealthHaver
    {
        public int Heal(int amount);
    }
}