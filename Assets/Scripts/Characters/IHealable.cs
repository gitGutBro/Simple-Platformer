using Health;

namespace Characters
{
    internal interface IHealable
    {
        HealthModel Health { get; }
        public void Heal(int amount);
    }
}