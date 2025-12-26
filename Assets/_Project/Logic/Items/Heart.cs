using _Project.Logic.Characters;
using UnityEngine;

namespace _Project.Logic.Items
{
    internal class Heart : MonoBehaviour, IHealConsumable
    {
        [SerializeField] private int _healAmount;

        public void Consume(IHealthHaver itemConsumer) => 
            itemConsumer.Health.Increase(_healAmount);
    }
}