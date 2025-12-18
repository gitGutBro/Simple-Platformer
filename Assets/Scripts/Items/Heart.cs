using UnityEngine;
using Characters;

namespace Items
{
    internal class Heart : MonoBehaviour
    {
        [SerializeField] private int _healAmount;
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out IHealable healable) is false) 
                return;
            
            healable.Heal(_healAmount);
            gameObject.SetActive(false);
        }
    }
}