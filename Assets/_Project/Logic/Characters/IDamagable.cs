namespace _Project.Logic.Characters
{
    internal interface IDamagable : IHealthHaver
    {
        public int TakeDamage(int amount);
    }
}