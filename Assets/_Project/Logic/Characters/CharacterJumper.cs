using System;
using UnityEngine;
using _Project.Logic.Configs;

namespace _Project.Logic.Characters
{
    [Serializable]
    internal class CharacterJumper
    {
        [SerializeField] private Transform _groundChecker;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        private bool _isJumpRequested;
        private bool _isJumpCutRequested;

        private IJumpData _data;

        public void Init(IJumpData data) => 
            _data = data;

        public bool IsGrounded { get; private set; }

        public void FixedUpdate()
        {
            IsGrounded = Physics2D.OverlapCircle(_groundChecker.position, _data.GroundCheckRadius, _data.GroundLayer);

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

        public void OnJumpPressed()
        {
            if (IsGrounded)
                _isJumpRequested = true;
        }

        public void OnJumpReleased()
        {
            if (_rigidbody2D.linearVelocityY > 0f)
                _isJumpCutRequested = true;
        }

        public bool OnIsGrounded() => 
            IsGrounded;
    }
}