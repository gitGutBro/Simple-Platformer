using Health;

namespace Characters
{
    internal interface IDamagable
    {
        HealthModel Health { get; }
        public void TakeDamage(int amount);
    }
}