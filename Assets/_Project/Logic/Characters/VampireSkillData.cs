using UnityEngine;

namespace _Project.Logic.Characters
{
    [CreateAssetMenu(fileName = "VampireSkillData", menuName = "Scriptable Objects/VampireSkillData")]
    internal class VampireSkillData : ScriptableObject
    {
        [field:SerializeField] public int DamageInHalfSecond { get; private set; }
        [field:SerializeField] public float CooldownInSeconds { get; private set; }
        [field:SerializeField] public float ActiveTimeInSeconds  { get; private set; }
    }
}