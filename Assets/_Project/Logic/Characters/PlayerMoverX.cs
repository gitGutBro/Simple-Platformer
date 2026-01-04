using System;
using UnityEngine;
using _Project.Logic.Common;
using _Project.Logic.Configs.Data;

namespace _Project.Logic.Characters
{
    [Serializable]
    internal class PlayerMoverX
    {
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        private IMoveData _data;

        public float HorizontalInput { get; private set; }

        public void Update() => 
            _characterTransform.FlipX(HorizontalInput);

        public void FixedUpdate() =>
            _rigidbody2D.linearVelocityX = HorizontalInput * _data.Speed;

        public void Init(IMoveData data) =>
            _data = data;

        public void OnMoveChanged(float x) =>
            HorizontalInput = x;
    }
}