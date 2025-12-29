using UnityEngine;
using _Project.Logic.Characters;

namespace _Project.Logic.Items
{
    internal class Coin : MonoBehaviour, IConsumable
    {
        public void Consume(IItemConsumer consumer)
        {
            consumer.Wallet.AddCoin();
            Destroy(gameObject);
        }
    }
}