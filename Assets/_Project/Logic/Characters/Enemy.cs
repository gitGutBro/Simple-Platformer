using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Logic.Health;
using _Project.Logic.Configs.Data;
using _Project.Logic.Infrastructure.StateMachine;

namespace _Project.Logic.Characters
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    internal class Enemy : MonoBehaviour, IDamagable, IChaseContext
    {
        [SerializeField] private Attacker _attacker;
        [SerializeField] private EnemyData _data;
        [SerializeField] private AnimationsCharacterSwitcher _animationsSwitcher;

        [field: SerializeField] public HealthModel Health { get; private set; }
        [field: SerializeField] public EnemyMoverX MoverX { get; private set; }
        [field: SerializeField] public TargetDetector TargetDetector { get; private set; }

        private StateMachine _stateMachine;

        private void Awake()
        {
            TargetDetector = new TargetDetector(transform, _data);

            _animationsSwitcher.Init(_data);
            _attacker.Init(_animationsSwitcher);
            MoverX.Init(_data);

            Health.Died += OnDie;
            MoverX.Moved += OnMoved;
            MoverX.Stoped += OnStoped;

            _stateMachine = new StateMachineBuilder()
                .AddState(() => new PatrolState(TargetDetector.IsTargetHit, MoverX.Patrolling))
                .AddState(() => new ChaseState(this))
                .AddState(() => new AttackState(_attacker.TryAttack,() => _attacker.OnCooldown, TargetDetector))
                .Build();
        }

        private void Start() =>
            _stateMachine.Enter<PatrolState>().Forget();

        private void OnValidate() => 
            Health.OnValidate();

        private void FixedUpdate() => 
            TargetDetector.DetectingTarget();

        private void OnDestroy()
        {
            Health.Died -= OnDie;
            MoverX.Moved -= OnMoved;
            MoverX.Stoped -= OnStoped;

            _stateMachine.Exit().Forget();
        }
        
        public void TakeDamage(int amount) => 
            Health.Decrease(amount);

        private void OnDie()
        {
            gameObject.SetActive(false);
            _stateMachine.Exit().Forget();
        }

        private void OnMoved() =>
            _animationsSwitcher.SetSpeed(1);

        private void OnStoped() =>
            _animationsSwitcher.SetSpeed(0);
    }
}