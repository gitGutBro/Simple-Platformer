using System;
using UnityEngine;

namespace Health
{
    [Serializable]
    internal class HealthModel
    {
        private const int Min = 0;

        [SerializeField][Min(Min)] private int _current;

        [field: SerializeField] public int Max { get; private set; }

        public HealthModel() =>
            _current = Max;

        public event Action Died;
        public event Action<int, int> Changed;

        public int Current => _current;

        public void OnValidate()
        {
            if (_current > Max)
                _current = Max;
        }

        public void Increase(int heal)
        {
            if (heal <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError($"Value heal is zero or less! {GetType()}");
#endif
                return;
            }

            _current = Mathf.Clamp(_current + heal, Min, Max);

            Changed?.Invoke(_current, Max);
        }

        public void Decrease(int damage)
        {
            if (damage <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError($"Value damage is zero or less! {GetType()}");
#endif
                return;
            }    

            _current = Mathf.Clamp(_current - damage, Min, Max);

            Changed?.Invoke(_current, Max);

            if (_current <= Min)
                Died?.Invoke();
        }
    }
}