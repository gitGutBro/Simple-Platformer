using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Logic.Common;
using _Project.Logic.Health;
using _Project.Logic.Configs;

namespace _Project.Logic.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    internal class Player : MonoBehaviour, IDamagable, IHealable, IHealthHaver
    {
        private const string Horizontal = nameof(Horizontal);

        [SerializeField] private PlayerData _data;
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _groundChecker;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Attacker _attacker;
        [SerializeField] private AnimationsCharacterSwitcher _animationsSwitcher;
        
        [field:SerializeField] public HealthModel Health { get; private set; }

        private bool _isAttacking;
        private bool _isGrounded;
        private bool _isJumpRequested;
        private bool _isJumpCutRequested;
        private float _horizontalInput;

        private bool CanAttack => Input.GetKeyDown(KeyCode.E) && _isAttacking is false &&
                                  _attacker.OnCooldown is false && _isGrounded;

        private void Awake()
        {
            Health.Died += OnDie;
        }

        private void OnValidate() => 
            Health.OnValidate();

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw(Horizontal);

            if (CanAttack)
            {
                _isAttacking = true;
                _animationsSwitcher.SetAttacking();
                HandleAttack().Forget();
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
                _isJumpRequested = true;

            if (Input.GetButtonUp("Jump") && _rigidbody2D.linearVelocityY > 0f)
                _isJumpCutRequested = true;
            
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
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _data.JumpForce);
                _isJumpRequested = false;
            }

            if (_isJumpCutRequested && _rigidbody2D.linearVelocityY > 0f)
            {
                _rigidbody2D.linearVelocity = new Vector2
                    (_rigidbody2D.linearVelocityX, _rigidbody2D.linearVelocityY * _data.JumpCutMultiplier);
                
                _isJumpCutRequested = false;
            }
        }

        private void OnDestroy() => 
            Health.Died -= OnDie;

        public void TakeDamage(int amount) => 
            Health.Decrease(amount);
        
        public void Heal(int amount) =>
            Health.Increase(amount);

        private async UniTaskVoid HandleAttack()
        {
            await _attacker.TryAttack();
            _isAttacking = false;
        }

        private void OnDie() => 
            gameObject.SetActive(false);
    }
}