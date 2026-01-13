using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Logic.Characters
{
    internal class VampireCooldownBar : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private Transform _transform;

        private void Awake() => 
            UpdateCooldown(0f, 0f);

        private void LateUpdate() =>
            _transform.rotation = Quaternion.identity;

        public void UpdateCooldown(float cooldownNormalized, float cooldownInSeconds)
        {
            _image.fillAmount = cooldownNormalized;
            _value.text = cooldownNormalized > 0 ? $"{cooldownInSeconds:F1}s" : string.Empty;
        }
    }
}