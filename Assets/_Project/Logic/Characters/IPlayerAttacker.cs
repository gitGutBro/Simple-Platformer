using System;

namespace _Project.Logic.Characters
{
    internal interface IPlayerAttacker
    {
        public void Init(IAttackAnimator animator, Func<bool> grounded);
        public void OnAttackPressed();
    }
}