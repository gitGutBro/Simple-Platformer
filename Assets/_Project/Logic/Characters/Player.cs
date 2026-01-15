using UnityEngine;
using Zenject;
using _Project.Logic.Health;
using _Project.Logic.InputSystem;
using _Project.Logic.Configs.Data;

namespace _Project.Logic.Characters
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    internal class Player : MonoBehaviour, IDamagable, IHealable, IItemConsumer
    {
        [SerializeField] private PlayerData _data;
        [SerializeField] private Attacker _attacker;
        [SerializeField] private VampireSkill _vampireSkill;
        [SerializeField] private CharacterJumper _jumper;
        [SerializeField] private PlayerMoverX _moverX;
        [SerializeField] private AnimationsCharacterSwitcher _animationsSwitcher;

        [field: SerializeField] public HealthModel Health { get; private set; }

        private IGameplayInputSystem _inputSystem;

        public Wallet Wallet { get; private set; }

        [Inject]
        public void Construct(IGameplayInputSystem inputSystem, Wallet wallet)
        {
            _inputSystem = inputSystem;
            Wallet = wallet;
        }
        
        private void Start()
        {
            _animationsSwitcher.Init(_data);
            _attacker.Init(_animationsSwitcher, _jumper.OnIsGrounded);

            _vampireSkill.Init(this);
            _jumper.Init(_data);
            _moverX.Init(_data);

            Health.Died += OnDie;

            _inputSystem.MoveChanged += _moverX.OnMoveChanged;
            _inputSystem.JumpPressed += _jumper.OnJumpPressed;
            _inputSystem.JumpReleased += _jumper.OnJumpReleased;
            _inputSystem.AttackPressed += _attacker.OnAttackPressed;
            _inputSystem.VampirePressed += _vampireSkill.OnVampirePressed;
        }

        private void OnValidate() => 
            Health.OnValidate();

        private void Update()
        {
            _moverX.Update();

            _animationsSwitcher.SetGrounded(_jumper.IsGrounded);
            _animationsSwitcher.SetSpeed(_moverX.HorizontalInput);
        }

        private void FixedUpdate()
        {
            _jumper.FixedUpdate();
            _moverX.FixedUpdate();
        }

        private void OnDestroy()
        {
            Health.Died -= OnDie;

            if (_inputSystem is not null)
            {
                _inputSystem.MoveChanged -= _moverX.OnMoveChanged;
                _inputSystem.JumpPressed -= _jumper.OnJumpPressed;
                _inputSystem.JumpReleased -= _jumper.OnJumpReleased;
                _inputSystem.AttackPressed -= _attacker.OnAttackPressed;
                _inputSystem.VampirePressed -= _vampireSkill.OnVampirePressed;
            }

            _inputSystem = null;
        }

        public int TakeDamage(int amount) => 
            Health.Decrease(amount);
        
        public int Heal(int amount) =>
            Health.Increase(amount);
        
        private void OnDie() => 
            gameObject.SetActive(false);        
    }
}