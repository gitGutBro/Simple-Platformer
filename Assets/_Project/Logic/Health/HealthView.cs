using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Project.Logic.Characters;

namespace _Project.Logic.Health
{
    [RequireComponent(typeof(Image))]
    internal class HealthView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private Transform _transform;
        
        private IDamagable _damagable;

        private void LateUpdate() => 
            _transform.rotation = Quaternion.identity;

        private void OnDestroy() => 
            _damagable.Health.Changed -= OnHealthChanged;
        
        public void Init(IDamagable damagable)
        {
            _damagable = damagable;
            _damagable.Health.Changed += OnHealthChanged;
            
            OnHealthChanged(_damagable.Health.Current, _damagable.Health.Max);
        }
        
        private void OnHealthChanged(int health, int max)
        {
            _value.text = $"{health:F0}/{max:F0}";

            if (max == 0)
            {
#if UNITY_EDITOR
                Debug.LogError($"Value {_damagable.Health.Max} is zero! {GetType()}");
#endif
                return;
            }

            _image.fillAmount = (float)health / _damagable.Health.Max;
        }
    }
}