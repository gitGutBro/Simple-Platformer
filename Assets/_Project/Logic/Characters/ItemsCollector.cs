using UnityEngine;
using _Project.Logic.Items;

namespace _Project.Logic.Characters
{
    internal class ItemsCollector : MonoBehaviour
    {
        private IItemConsumer _consumer;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent(out IConsumable item))
                item.Consume(_consumer);
        }
        
        public void Init(IItemConsumer consumer) =>
            _consumer = consumer;
    }
}