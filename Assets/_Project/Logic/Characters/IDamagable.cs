namespace _Project.Logic.Characters
{
    internal interface IDamagable : IHealthHaver
    {
        public void TakeDamage(int amount);
    }
}