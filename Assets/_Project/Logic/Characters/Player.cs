using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Logic.Common;
using _Project.Logic.Health;
using _Project.Logic.Skills;
using _Project.Logic.Configs;
using _Project.Logic.InputSystem;

namespace _Project.Logic.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    internal class Player : MonoBehaviour, IDamagable, IHealable, IItemConsumer
    {
        [SerializeField] private PlayerData _data;
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _groundChecker;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Attacker _attacker;
        [SerializeField] private VampireSkill _vampireSkill;
        [SerializeField] private AnimationsCharacterSwitcher _animationsSwitcher;
        
        [field: SerializeField] public HealthModel Health { get; private set; }
        
        public Wallet Wallet { get; private set; }

        private bool _isAttacking;
        private bool _isGrounded;
        private bool _isJumpRequested;
        private bool _isJumpCutRequested;
        
        private float _horizontalInput;
        
        private IGameplayInputSystem _input;
        
        private bool CanAttack => _isAttacking is false && _attacker.OnCooldown is false && _isGrounded;

        private void Awake()
        {
            _input = new GameplayNewInputSystem();
            _vampireSkill.Init(this);
            
            Wallet = new Wallet();
            Health.Died += OnDie;

            _input.MoveChanged += OnMoveChanged;
            _input.JumpPressed += OnJumpPressed;
            _input.JumpReleased += OnJumpReleased;
            _input.AttackPressed += OnAttackPressed;
            _input.VampirePressed += OnVampirePressed;
        }

        private void OnValidate() => 
            Health.OnValidate();

        private void Update()
        {
            _transform.FlipX(_horizontalInput);
            
            _animationsSwitcher.SetSpeed(_horizontalInput);
            _animationsSwitcher.SetGrounded(_isGrounded);
        }

        private void FixedUpdate()
        {
            _isGrounded = Physics2D.OverlapCircle(_groundChecker.position, _data.GroundCheckRadius, _data.GroundLayer);
            _rigidbody2D.linearVelocityX = _horizontalInput * _data.Speed;

            if (_isJumpRequested)
            {
                _rigidbody2D.linearVelocityY = _data.JumpForce;
                _isJumpRequested = false;
            }

            if (_isJumpCutRequested && _rigidbody2D.linearVelocityY > 0f)
            {
                _rigidbody2D.linearVelocityY *= _data.JumpCutMultiplier;
                _isJumpCutRequested = false;
            }
        }

        private void OnDestroy()
        {
            Health.Died -= OnDie;
            
            _input.MoveChanged -= OnMoveChanged;
            _input.JumpPressed -= OnJumpPressed;
            _input.JumpReleased -= OnJumpReleased;
            _input.AttackPressed -= OnAttackPressed;
            _input.VampirePressed -= OnVampirePressed;
            
            _input?.Dispose();
            _input = null;
        }

        public void TakeDamage(int amount) => 
            Health.Decrease(amount);
        
        public void Heal(int amount) =>
            Health.Increase(amount);
        
        private async UniTaskVoid HandleAttack()
        {
            await _attacker.TryAttack();
            _isAttacking = false;
        }

        private void OnAttackPressed()
        {
            if (CanAttack is false)
                return;
            
            _isAttacking = true;
            _animationsSwitcher.SetAttacking();
            HandleAttack().Forget();
        }
        
        private void OnDie() => 
            gameObject.SetActive(false);
        
        private void OnMoveChanged(float x) => 
            _horizontalInput = x;

        private void OnJumpPressed()
        {
            if (_isGrounded)
                _isJumpRequested = true;
        }

        private void OnJumpReleased()
        {
            if (_rigidbody2D.linearVelocityY > 0f)
                _isJumpCutRequested = true;
        }

        private void OnVampirePressed() => 
            _vampireSkill.TryActivate();
    }
}