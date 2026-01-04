namespace _Project.Logic.Characters
{
    internal interface IAttackAnimator
    {
        float AttackAnimationTimeInSeconds { get; }
        public void SetAttacking();
    }
}