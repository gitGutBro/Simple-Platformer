using Cysharp.Threading.Tasks;
using UnityEngine;
using Common;
using Health;

namespace Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    internal class Player : MonoBehaviour, IDamagable, IHealable
    {
        private const string Horizontal = nameof(Horizontal);

        [SerializeField] private float _speed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _jumpCutMultiplier;
        [SerializeField] private float _groundCheckRadius;
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _groundChecker;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Animator _animator;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private Attacker _attacker;
        
        [field:SerializeField] public HealthModel Health { get; private set; }

        private bool _isAttacking;
        private bool _isGrounded;   
        private bool _isJumpRequested;
        private bool _isJumpCutRequested;
        private float _horizontalInput;
        private AnimationsCharacterSwitcher _animationsSwitcher;

        private bool CanAttack => Input.GetKeyDown(KeyCode.E) && _isAttacking is false &&
                                  _attacker.OnCooldown is false && _isGrounded;

        private void Awake()
        {
            _animationsSwitcher = new AnimationsCharacterSwitcher(_animator);
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
            _isGrounded = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckRadius, _groundLayer);
            _rigidbody2D.linearVelocityX = _horizontalInput * _speed;

            if (_isJumpRequested)
            {
                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpForce);
                _isJumpRequested = false;
            }

            if (_isJumpCutRequested && _rigidbody2D.linearVelocityY > 0f)
            {
                _rigidbody2D.linearVelocity = new Vector2
                    (_rigidbody2D.linearVelocityX, _rigidbody2D.linearVelocityY * _jumpCutMultiplier);
                
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