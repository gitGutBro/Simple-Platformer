namespace _Project.Logic.Characters
{
    internal interface IAttackAnimator
    {
        float AttackAnimationTimeInMilliseconds { get; }
        public void SetAttacking();
    }
}