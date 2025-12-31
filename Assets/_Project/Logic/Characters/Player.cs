using UnityEngine;
using _Project.Logic.Health;
using _Project.Logic.Configs;
using _Project.Logic.InputSystem;

namespace _Project.Logic.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    internal class Player : MonoBehaviour, IDamagable, IHealable, IItemConsumer
    {
        [SerializeField] private PlayerData _data;
        [SerializeField] private Attacker _attacker;
        [SerializeField] private VampireSkill _vampireSkill;
        [SerializeField] private CharacterJumper _jumper;
        [SerializeField] private CharacterMoverX _moverX;
        [SerializeField] private AnimationsCharacterSwitcher _animationsSwitcher;

        [field: SerializeField] public HealthModel Health { get; private set; }

        private IGameplayInputSystem _input;
 
        public Wallet Wallet { get; private set; }

        
        private void Awake()
        {
            _input = new GameplayNewInputSystem();

            _attacker.Init(_animationsSwitcher);
            _attacker.Grounded += _jumper.OnIsGrounded;

            _vampireSkill.Init(this);
            _jumper.Init(_data);
            _moverX.Init(_data);

            Wallet = new Wallet();
            Health.Died += OnDie;

            _input.MoveChanged += _moverX.OnMoveChanged;
            _input.JumpPressed += _jumper.OnJumpPressed;
            _input.JumpReleased += _jumper.OnJumpReleased;
            _input.AttackPressed += _attacker.OnAttackPressed;
            _input.VampirePressed += _vampireSkill.OnVampirePressed;
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
            _attacker.Grounded -= _jumper.OnIsGrounded;

            Health.Died -= OnDie;
            
            _input.MoveChanged -= _moverX.OnMoveChanged;
            _input.JumpPressed -= _jumper.OnJumpPressed;
            _input.JumpReleased -= _jumper.OnJumpReleased;
            _input.AttackPressed -= _attacker.OnAttackPressed;
            _input.VampirePressed -= _vampireSkill.OnVampirePressed;
            
            _input?.Dispose();
            _input = null;
        }

        public void TakeDamage(int amount) => 
            Health.Decrease(amount);
        
        public void Heal(int amount) =>
            Health.Increase(amount);
        
        private void OnDie() => 
            gameObject.SetActive(false);        
    }
}