using UnityEngine;
using _Project.Logic.Characters;

namespace _Project.Logic.Items
{
    internal class Heart : MonoBehaviour, IConsumable
    {
        [SerializeField] private int _healAmount;

        public void Consume(IItemConsumer itemConsumer)
        {
            itemConsumer.Health.Increase(_healAmount);
            Destroy(gameObject);
        }
    }
}