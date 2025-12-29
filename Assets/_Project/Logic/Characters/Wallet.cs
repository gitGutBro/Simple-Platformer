using UnityEngine;

namespace _Project.Logic.Characters
{
    internal class Wallet
    {
        private uint _countCoins;

        public void AddCoin()
        {
            _countCoins++;

#if UNITY_EDITOR
            Debug.Log($"Add coin. Count coins: {_countCoins}");
#endif
        }
    }
}