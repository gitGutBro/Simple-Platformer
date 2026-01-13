using System;
using UnityEngine;
using _Project.Logic.Characters;

namespace _Project.Logic.Items
{
    internal class Coin : MonoBehaviour, IConsumable
    {
        public event Action Consumed;

        public void Consume(IItemConsumer consumer)
        {
            consumer.Wallet.AddCoin();
            gameObject.SetActive(false);
            Consumed?.Invoke();
        }

        public void Respawn(Transform newPosition)
        {
            transform.position = newPosition.position;
            gameObject.SetActive(true);
        }
    }
}