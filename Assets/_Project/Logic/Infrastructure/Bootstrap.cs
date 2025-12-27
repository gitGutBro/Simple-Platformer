using UnityEngine;
using _Project.Logic.Health;
using _Project.Logic.Skills;
using _Project.Logic.Characters;

namespace _Project.Logic.Infrastructure
{
    internal class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private HealthView _playerHealthView;
        [Space]
        [SerializeField] private Enemy _enemy;
        [SerializeField] private HealthView _enemyHealthView;
        [Space]
        [SerializeField] private VampireSkill _vampireSkill;

        private void Awake()
        {
            _playerHealthView.Init(_player);
            _enemyHealthView.Init(_enemy);
            _vampireSkill.Init(_player);
        }
    }
}